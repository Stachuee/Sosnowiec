using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


[System.Serializable]
public struct PlayerInfo
{
    public float hp;
    public float maxHp;

    public float speed;
    public float throwStrength;

    public DamageTypeResistance damageResistance;
}

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour, ITakeDamage
{
    [SerializeField][Header("Player info")]
    public PlayerInfo playerInfo;

    [Header("Movment")]
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
    Vector3 m_Velocity = Vector3.zero;
    [SerializeField] bool topPlayer;

    Vector2 moveDirection = Vector2.zero;
    public Vector2 currentAimDirection {get; private set;}
    Vector2 desiredAimDirection = Vector2.zero;
    bool keyboard;

    float jumping;

    Rigidbody2D myBody;
    PlayerInput myinput;
    public GunController gunController;
    public EquipmentController equipmentController;
    //public UiController uiController;

    [SerializeField]
    Camera cam;

    List<IInteractable> inRange = new List<IInteractable>();

    [SerializeField]
    int maxItemsHeld = 3;
    List<ItemSO> heldItems = new List<ItemSO>();


    public bool LockInAnimation
    {
        get
        {
            return lockedInAnimation;
        }
        set
        {
            lockedInAnimation = value;
        }
    }
    UnityAction callbackWhenUnlocking;
    bool lockedInAnimation;

    private void Awake()
    {
        myBody = transform.GetComponent<Rigidbody2D>();
        myinput = transform.GetComponent<PlayerInput>();
        gunController = transform.GetComponent<GunController>();
        equipmentController = transform.GetComponent<EquipmentController>();
        keyboard = myinput.currentControlScheme == "Keyboard" ? true : false; // Check if player uses keyboard or controller

        playerInfo = new PlayerInfo() { hp = 100, maxHp = 100, speed = 5, throwStrength = 500};
    }

    private void Update() 
    {
        if (!LockInAnimation) myBody.velocity = Vector3.SmoothDamp(myBody.velocity, new Vector2(moveDirection.x * playerInfo.speed, myBody.velocity.y), ref m_Velocity, m_MovementSmoothing);
        else myBody.velocity = new Vector2(0, myBody.velocity.y);

        if (keyboard) //refactor this pls
        {
            Vector2 mouse = Mouse.current.position.ReadValue();
            Vector2 mousePos = cam.ScreenToWorldPoint(mouse);
            currentAimDirection = new Vector2((mouse.x - Screen.width/2) / (Screen.width / 2), (mouse.y - Screen.height * 3/4) / (Screen.height * 3 / 4));
        }
        else
        {
            Vector2 vel = Vector2.zero;
            currentAimDirection = Vector2.SmoothDamp(currentAimDirection, desiredAimDirection, ref vel, 0.02f);
        }
    }


    [SerializeField]
    GameObject itemPrefab;

    void DropHolding()
    {
        if (heldItems.Count == 0) return;
        Item itemDropped = Instantiate(itemPrefab, transform.position, Quaternion.identity).GetComponentInChildren<Item>();
        itemDropped.Innit(heldItems[heldItems.Count - 1]);
        heldItems.RemoveAt(heldItems.Count - 1);

        itemDropped.GetComponentInParent<Rigidbody2D>().AddForce(currentAimDirection.normalized * playerInfo.throwStrength);
    }

    public bool PickUp(ItemSO item)
    {
        if (heldItems.Count >= maxItemsHeld) return false;
        else
        {
            heldItems.Add(item);
            return true;
        }
    }

    #region InputRegion
    public void OnMove(InputAction.CallbackContext context)
    {   
        moveDirection = context.ReadValue<Vector2>();
        SendMovmentControll(moveDirection);
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (lockedInAnimation)
        {
            callbackWhenUnlocking.Invoke();
            LockInAnimation = false;
        }
        if (context.ReadValue<float>() > 0.9f)
        {
            SendbackControll();
            DropHolding();
        }
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (context.ReadValue<float>() > 0.9f)
        {
            SendUseControll();
            if (inRange.Count > 0)
                inRange[0].Use(this);
        } // use one
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumping = context.ReadValue<float>();
        //myBody.AddForce(new Vector2(0, 200));
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (context.ReadValue<float>() > 0.9f) gunController.ShootGun();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        desiredAimDirection = context.ReadValue<Vector2>();
        desiredAimDirection = desiredAimDirection.magnitude > 1 ? desiredAimDirection.normalized : desiredAimDirection;
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (context.ReadValue<float>() > 0.9f) equipmentController.UseEquipment();
    }
    #endregion

    #region UseRegion
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Interactable")
        {
            inRange.Add(collision.transform.GetComponent<IInteractable>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Interactable")
        {
            inRange.Remove(collision.transform.GetComponent<IInteractable>());
        }
    }


    #endregion

    #region interfaces
    public float TakeDamage(float damage, DamageType type)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region ForwardControll

    List<IControllSubscriberMovment> movmentSubscribers = new List<IControllSubscriberMovment>();
    public void AddMovmentSubscriber(IControllSubscriberMovment subscriberMovment)
    {
        movmentSubscribers.Add(subscriberMovment);
    }
    public void RemoveMovmentSubscriber(IControllSubscriberMovment subscriberMovment)
    {
        movmentSubscribers.Remove(subscriberMovment);
    }
    void SendMovmentControll(Vector2 context)
    {
        movmentSubscribers.ForEach(x => x.ForwardCommandMovment(context));
    }

    List<IControllSubscriberUes> iControllSubscriberUse = new List<IControllSubscriberUes>();
    public void AddUseSubscriber(IControllSubscriberUes subscriberMovment)
    {
        iControllSubscriberUse.Add(subscriberMovment);
    }
    public void RemoveUseSubscriber(IControllSubscriberUes subscriberMovment)
    {
        iControllSubscriberUse.Remove(subscriberMovment);
    }
    void SendUseControll()
    {
        iControllSubscriberUse.ForEach(x => x.ForwardCommandUse());
    }

    List<IControllSubscriberBack> iControllSubscriberBack = new List<IControllSubscriberBack>();
    public void AddBackSubscriber(IControllSubscriberBack subscriberMovment)
    {
        iControllSubscriberBack.Add(subscriberMovment);
    }
    public void RemoveBackSubscriber(IControllSubscriberBack subscriberMovment)
    {
        iControllSubscriberBack.Remove(subscriberMovment);
    }
    void SendbackControll()
    {
        iControllSubscriberBack.ForEach(x => x.ForwardCommandBack());
    }

    #endregion

    #region ExternalControll

    public ItemSO DepositIngredient()
    {
        if (heldItems.Count > 0)
        {
            ItemSO temp = heldItems[heldItems.Count - 1];
            heldItems.Remove(temp);
            return temp;
        }
        else return null;
    }

    public void LockInAction(UnityAction callback)
    {
        if (callbackWhenUnlocking != null) callbackWhenUnlocking.Invoke();
        callbackWhenUnlocking = callback;
        LockInAnimation = true;
    }

    public void UnlockInAnimation()
    {
        LockInAnimation = false;
    }

    #endregion
}
