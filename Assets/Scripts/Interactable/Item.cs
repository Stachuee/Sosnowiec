using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Item : InteractableBase
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

    public void Innit(ItemSO item)
    {
        itemSO = item;
    }

    void PickUpMatherial(PlayerController player)
    {
        if (player.PickUp(itemSO)) Destroy(transform.parent.gameObject);
    }
}
