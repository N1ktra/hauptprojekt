using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private List<Weapon> weapons = new List<Weapon>();
    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private GameObject weaponHolder;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    private void Update()
    {
        if (currentWeapon == null) return;
        if ((currentWeapon.isAutomatic && Input.GetMouseButton(0)) || Input.GetMouseButtonDown(0))
        {
            currentWeapon.Shoot();
        }
    }

    /// <summary>
    /// Fügt eine Waffe hinzu, setzt sie jedoch nicht als aktuelle
    /// </summary>
    /// <param name="weapon"></param>
    public void addWeapon(Weapon weapon)
    {
        weapon.gameObject.SetActive(false);
        weapons.Add(weapon);
        weapon.transform.parent = transform;
        weapon.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Entfernt eine Waffe
    /// </summary>
    /// <param name="weapon"></param>
    public void removeWeapon(Weapon weapon)
    {
        if(currentWeapon == weapon)
            currentWeapon = weapons[0];
        weapons.Remove(weapon);
        Destroy(weapon);
    }

    /// <summary>
    /// Gibt die aktuell ausgerüstete Waffe zurück
    /// </summary>
    /// <returns></returns>
    public Weapon getCurrentWeapon()
    {
        return currentWeapon;
    }

    /// <summary>
    /// Setzt die aktuell ausgerüstete Waffe
    /// </summary>
    /// <param name="index">Der Index, an dem die Waffe in der Liste steht</param>
    public void setCurrentWeapon(int index)
    {
        for(int i = 0; i < weapons.Count; i++)
        {
            weapons[i].gameObject.SetActive(i == index);
            currentWeapon = weapons[index];
        }
    }
}
