using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CureMachineUI : MonoBehaviour
{
    readonly int MAX_ITEMS_REQUIRED = 7;

    [SerializeField] GameObject cureMachine;
    [SerializeField] bool active;
    [SerializeField] Image progressSlider;
    [SerializeField] GameObject noPower;

    [SerializeField] List<Image> supports;
    [SerializeField] List<GameObject> supportsRequired;

    [SerializeField] Transform itemNeededContainder;
    List<Image> items;
    [SerializeField] GameObject itemRequiredPrefab;

    CureMachine cureMachineScript;

    int lastProgressIndex;
    ProgressStageController cureController;
    public void Open(bool on)
    {
        if (on) cureMachine.SetActive(true);
        else cureMachine.SetActive(false);
        active = on;
        noPower.SetActive(!CureMachine.Instance.IsPowered());
        UpdateItemsRequired();
    }

    private void Start()
    {
        cureController = ProgressStageController.instance;
        cureMachineScript = CureMachine.Instance;
        items = new List<Image>();

        for (int i = 0; i < MAX_ITEMS_REQUIRED; i++)
        {
            GameObject temp = Instantiate(itemRequiredPrefab);
            temp.transform.SetParent(itemNeededContainder);
            temp.transform.localScale = Vector3.one;
            temp.SetActive(false);
            items.Add(temp.GetComponent<Image>());
        }
    }

    private void Update()
    {
        if(active)
        {
            progressSlider.fillAmount = ProgressStageController.instance.GetCurrentProgress();
            for(int i = 0; i < supports.Count; i++)
            {
                supports[i].fillAmount = cureMachineScript.GetSupport(i);
            }

            if(lastProgressIndex != cureController.GetCurrentLevel())
            {
                UpdateItemsRequired();
            }
        }
    }

    public void UpdateItemsRequired()
    {
        lastProgressIndex = cureController.GetCurrentLevel();
        int id = 0;
        items.ForEach(item =>
        {
            item.gameObject.SetActive(false);
        });

        supportsRequired.ForEach(supportRequired =>
        {
            supportRequired.gameObject.SetActive(false);
        });

        CureMachine.Instance.GetRequiredSupport().ForEach(item =>
        {
            supportsRequired[(int)item].gameObject.SetActive(true);
        });

        CureMachine.Instance.GetRequiredItems().ForEach(item =>{
            items[id].sprite = item.GetIconSprite();
            items[id].gameObject.SetActive(true);
            id++;
        });
    }

}
