using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHUDController : MonoBehaviour
{
    [SerializeField] GameObject HUD;

    [SerializeField] GameObject doors;
    [SerializeField] GameObject turrets;
    [SerializeField] GameObject printers;


    [SerializeField] EnergyComputerIndicators doorsIndicator;
    [SerializeField] EnergyComputerIndicators printersIndicator;
    [SerializeField] EnergyComputerIndicators turretsIndicator;

    MapSegment currentSegment;
    bool update;
    public void Show(bool state, MapSegment segment = null)
    {
        HUD.SetActive(state);
        update = state;
        currentSegment = segment;
        if (segment != null)
        {
            doorsIndicator.UpdateIndicator(segment.GetPowerStatus(SwitchType.Doors));
            printersIndicator.UpdateIndicator(segment.GetPowerStatus(SwitchType.Printers));
            turretsIndicator.UpdateIndicator(segment.GetPowerStatus(SwitchType.Security));
        }
    }

    private void Update()
    {
        if (update)
        {
            doorsIndicator.UpdateIndicator(currentSegment.GetPowerStatus(SwitchType.Doors));
            printersIndicator.UpdateIndicator(currentSegment.GetPowerStatus(SwitchType.Printers));
            turretsIndicator.UpdateIndicator(currentSegment.GetPowerStatus(SwitchType.Security));
        }
    }
}
