using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    public WeaponHandler weaponHandler;
    public PlayerStats playerStats;

    [Header("Hitmarker")]
    public RawImage hitmarker;
    public Color StandardHitColor;
    public Color OnEnemyHitColor;

    [Header("Interface")]
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
        {
            Enemy enemy;
            if (e.EnemyHit(out enemy))
                hitmarker.color = OnEnemyHitColor;
            else
                hitmarker.color = StandardHitColor;
            StartCoroutine(showGameObject(hitmarker.gameObject, .1f));
        }
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
