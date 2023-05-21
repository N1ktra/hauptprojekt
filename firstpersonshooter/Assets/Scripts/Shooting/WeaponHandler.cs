using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private List<Weapon> weapons = new List<Weapon>();
    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private GameObject weaponHolder;

    private KeyCode[] keyCodes;
    // Start is called before the first frame update
    private void Start()
    {
        keyCodes = new KeyCode[] {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9
        };
        foreach(Weapon weapon in weapons)
        {
            weapon.transform.gameObject.SetActive(weapon == currentWeapon);
        }
    }

    private void Update()
    {
        if (currentWeapon == null) return;

        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                setCurrentWeapon(i);
            }
        }

        if ((currentWeapon.isAutomatic && Input.GetMouseButton(0)) || Input.GetMouseButtonDown(0))
        {
            currentWeapon.Shoot(Input.GetMouseButtonDown(0));
        }
    }

    /// <summary>
    /// F�gt eine Waffe hinzu, setzt sie jedoch nicht als aktuelle
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
    /// Gibt die aktuell ausger�stete Waffe zur�ck
    /// </summary>
    /// <returns></returns>
    public Weapon getCurrentWeapon()
    {
        return currentWeapon;
    }

    /// <summary>
    /// Setzt die aktuell ausger�stete Waffe
    /// </summary>
    /// <param name="index">Der Index, an dem die Waffe in der Liste steht</param>
    public void setCurrentWeapon(int index)
    {
        if (weapons.Count < index + 1) return;
        currentWeapon.gameObject.SetActive(false);
        weapons[index].gameObject.SetActive(true);
        currentWeapon = weapons[index];
    }
}
