using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PowerDeposit : InteractableBase
{

    List<ItemSO> acceptedItems;

    [SerializeField]
    ItemSO inDeposit;
    [SerializeField]
    GameObject itemPrefab;
    [SerializeField]
    SpriteRenderer powerCellRenderer;

    FuseBox fuseBox;

    bool firstTime = true;

    protected override void Awake()
    {
        base.Awake();
        fuseBox = transform.GetComponentInParent<FuseBox>();
        AddAction(DepositBattery);

        acceptedItems = new List<ItemSO>();
    }

    public void DepositBattery(PlayerController player)
    {
        if(inDeposit == null)
        {
            ItemSO deposited = player.CheckIfHoldingAnyAndDeposit<PowerCoreItem>();
            if (deposited != null)
            {
                inDeposit = deposited;
                powerCellRenderer.sprite = deposited.GetIconSprite();
                fuseBox.PlugIn((deposited as PowerCoreItem).GetPowerLevel());
                if(firstTime)
                {
                    ProgressStageController.instance.StartGame();
                    firstTime = false;
                }
            }
        }
        else
        {
            GameObject temp = Instantiate(itemPrefab, transform.position, Quaternion.identity);
            temp.GetComponentInChildren<Item>().Innit(inDeposit);
            inDeposit = null;
            powerCellRenderer.sprite = null;
            fuseBox.PlugIn(0);
        }
    }


    public override bool IsUsable(PlayerController player)
    {
        return inDeposit != null || player.CheckIfHoldingAny<PowerCoreItem>();
    }
}
