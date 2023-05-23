using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public WeaponHandler weaponHandler;
    public PlayerStats playerStats;

    public RawImage hitmarker;
    public RawImage weaponSymbol;
    public TextMeshProUGUI AmmoText;
    public TextMeshProUGUI HealthText;
    public Slider HealthBar;

    // Start is called before the first frame update
    void Start()
    {
        HealthBar.minValue = 0;
        HealthBar.maxValue = playerStats.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        setAmmoDisplay();
        setHealthDisplay();
    }

    public void SetWeaponSymbol(Texture texture)
    {
        weaponSymbol.texture = texture;
    }

    public void showHitmarker(object sender, AttackEventArgs e)
    {
        if (hitmarker == null) return;
        if (!hitmarker.gameObject.activeSelf)
            StartCoroutine(showGameObject(hitmarker.gameObject, .1f));
    }
    private IEnumerator showGameObject(GameObject obj, float duration)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
    }

    public void setAmmoDisplay()
    {
        Weapon currentWeapon = weaponHandler.getCurrentWeapon();
        if (currentWeapon == null) return;
        if (currentWeapon is Gun)
        {
            Gun gun = (Gun)currentWeapon;
            AmmoText.text = gun.currentAmmo +  " / " + gun.maxAmmo;
        }
        else
        {
            AmmoText.text = "-";
        }
    }

    public void setHealthDisplay()
    {
        HealthText.text = playerStats.currentHealth + " / " + playerStats.maxHealth;
        HealthBar.value = playerStats.currentHealth;
    }

}
