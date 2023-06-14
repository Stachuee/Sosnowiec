using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusMonitor : InteractableBase
{
    PlayerController activePlayer;

    [SerializeField] AudioSource openCraftingSFX;

    void Start()
    {
        AddAction(OpenStatus);
    }

    public void OpenStatus(PlayerController player, UseType type)
    {
        openCraftingSFX.Play();
        activePlayer = player;
        player.LockInAction(CloseInterface);
        player.uiController.upgradeGuide.Open(true);
    }

    public void CloseInterface()
    {
        activePlayer.UnlockInAnimation();
        activePlayer.uiController.upgradeGuide.Open(false);
    }
}
