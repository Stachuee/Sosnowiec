using GD.MinMaxSlider;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBoxUIScript : MonoBehaviour
{
    [SerializeField] List<Fuse> fuses;

    [SerializeField] Vector2 offset;
    [SerializeField] float fuseHeight;

    [SerializeField] GameObject fusePrefab;

    [SerializeField] PlayerController playerController;

    [SerializeField] Transform fuseBoxUI;

    [SerializeField] Transform gaugeArrow;
    [SerializeField] Transform overchargeGaugeArrow;
    FuseBox openFuseBox;

    [SerializeField]
    Vector2 gaugeAngle;

    SwitchType openType;

    bool setUp;


    public void Setup()
    {
        List<MapSegment> segments = SegmentController.segmentController.mapSegments.FindAll(x => x.CreateFuse());
        int segmenCount = segments.Count;
        
        float fuzeOffset = fuseHeight / (segmenCount - 1);
        for (int i = 0; i < segmenCount; i++)
        {
            Fuse temp = Instantiate(fusePrefab, (Vector2)transform.position + offset + new Vector2(0, fuseHeight / 2) - new Vector2(0, fuzeOffset * i), Quaternion.identity, fuseBoxUI).GetComponent<Fuse>();
            temp.SetFuse(segments[i].sectorName, this);
            fuses.Add(temp);
        }
        setUp = true;
    }

    private void Update()
    {
        if(openFuseBox != null)
        {
            float angle = (gaugeAngle.y - gaugeAngle.x) * Mathf.Min(openFuseBox.GetPowerConsumption(), 1f); // max overloaded arrow 
            gaugeArrow.rotation = Quaternion.Euler(0, 0, gaugeAngle.x + angle);
            angle = (gaugeAngle.y - gaugeAngle.x) * ElectricityController.overcharge / ElectricityController.maxOvercharge;
            overchargeGaugeArrow.rotation = Quaternion.Euler(0, 0, gaugeAngle.x + angle);
        }
    }

    public void OpenBox(SwitchType openType, FuseBox boxOpen)
    {
        if (!setUp) Setup();
        this.openType = openType;

        openFuseBox = boxOpen;

        fuses.ForEach(x =>
        {
            x.TurnFuse(boxOpen.GetFuseStatus(x.segmentName));
        });
        fuseBoxUI.gameObject.SetActive(true);
        playerController.uiController.myEventSystem.SetSelectedGameObject(fuses[0].gameObject);
    }
    public void CloseBox()
    {
        fuseBoxUI.gameObject.SetActive(false);
    }

    public void SwitchFuse(string segmentName, bool on)
    {
        openFuseBox.UpdateFuse(segmentName, on);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine((Vector2)transform.position + offset + new Vector2(0, fuseHeight / 2), (Vector2)transform.position + offset - new Vector2(0, fuseHeight / 2));
    }
}
