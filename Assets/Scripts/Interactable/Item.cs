using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Item : InteractableBase, IGetHandInfo
{
    [SerializeField]
    ItemSO itemSO;

    SpriteRenderer myRenderer;


    protected override void Awake()
    {
        base.Awake();
        myRenderer = transform.GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        myRenderer.sprite = itemSO.GetDefaultSprite();
        AddAction(PickUpMatherial); 
    }

    public void Innit(ItemSO item, PlayerController player = null)
    {
        itemSO = item;
        item.Drop(player, this);
    }

    void PickUpMatherial(PlayerController player, UseType type)
    {
        //if (player.PickUp(itemSO)) Destroy(transform.parent.gameObject);
        bool destroy = false;

        if(itemSO.PickUp(player, this, out destroy))
        {
            if (player.PickUp(itemSO))
            {
                Destroy(transform.parent.gameObject);
            }
        }
        else if(destroy)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    public ItemSO GetItem()
    {
        return itemSO;
    }

    public HandInfoContainer GetHandInfo()
    {
        return new HandInfoContainer() { show = true, name = itemSO.GetItemName(), sprite = itemSO.GetIconSprite()};
    }
}
