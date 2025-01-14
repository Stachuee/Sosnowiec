using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Light : PoweredInteractable
{
    Light2D myLight;

    bool turnOn;

    [SerializeField] bool inverted;

    protected override void Awake()
    {
        base.Awake();
        myLight = transform.GetComponent<Light2D>();
        PowerOn(false, "");
        AddAction(TurnOnOffByComputer);
    }

    public void TurnOnOffByComputer(PlayerController player, UseType type)
    {
        turnOn = !turnOn;
    }
    override public void PowerOn(bool on, string sectorName)
    {
        //if (!turnOn) return;

        if(on)
        {
            myLight.enabled = true ^ inverted;
        }
        else
        {
            myLight.enabled = false ^ inverted;
        }
    }
}
