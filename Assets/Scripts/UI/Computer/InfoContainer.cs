using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ComputerInfoContainer
{
    public string name;
    
    public bool showHp;
    public float hp;
    public float maxHp;
    
    public bool showCharge;
    public bool charged;

    public bool showProgress;
    public float progress;
}


public struct HandInfoContainer
{
    public bool show;

    public string name;
    public Sprite sprite;
}
