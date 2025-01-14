using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatHUDController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    [SerializeField] Image hpSlider;


    [SerializeField] List<Sprite> equipmentIcons;
    [SerializeField] Image equipmentIcon;
    [SerializeField] TextMeshProUGUI equipmentCount;

    [SerializeField] Image bulletTypeSprite;
    [SerializeField] Text bulletsAmount;

    GunController gunController;

    bool active = true;

    [SerializeField] Vector2 reloadOffset;
    [SerializeField] Vector2 useOffset;
    [SerializeField] Transform reload;
    [SerializeField] Image reloadImage;

    [SerializeField] Transform useSpriteParrent;
    [SerializeField] GameObject useSpritePad;
    [SerializeField] GameObject useSpriteKeyboard;


    private void Start()
    {
        if (playerController.isScientist)
        {
            gameObject.SetActive(false);
            active = false;
        }
        reload.gameObject.SetActive(false);

        gunController = playerController.gunController;
    }
    private void LateUpdate()
    {
        useSpriteParrent.position = (Vector2)playerController.transform.position + useOffset;
        if (!active) return;
        reload.transform.position = (Vector2)playerController.transform.position + reloadOffset;
    }

    private void Update()
    {
        if (!active) return;

        hpSlider.fillAmount = playerController.playerInfo.hp / playerController.playerInfo.maxHp;

        bulletTypeSprite.sprite = gunController.GetCurrentGun().GetAmmoIcon();
        bulletsAmount.text = (gunController.GetCurrentGun().GetAmmoAmount());


    }

    public void UpdateReload(float progress)
    {
        reloadImage.fillAmount = (1 - progress);
        
        if(progress <= 0.05f)
        {
            reload.gameObject.SetActive(false);
        }
        else
        {
            reload.gameObject.SetActive(true);
        }
    }

    public void UpdateEquipment(EquipmentType current)
    {
        equipmentIcon.sprite = equipmentIcons[(int)current];
    }

    public void UpdateEquipmentCount(int count)
    {
        equipmentCount.text = count.ToString();
    }

    public void ShowUse(bool state)
    {
        if(playerController.keyboard)
        {
            useSpriteKeyboard.SetActive(state); 
        }
        else
        {
            useSpritePad.SetActive(state);
        }
    }
}
