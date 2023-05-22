using DG.Tweening;
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
                switchWeapon(i);
            }
        }

        if (currentWeapon == null) return;

        if ((currentWeapon.isAutomatic && Input.GetMouseButton(0)) || Input.GetMouseButtonDown(0))
        {
            currentWeapon.Attack(Input.GetMouseButtonDown(0));
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
        if (currentWeapon == weapon)
            switchWeapon(0);
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
    public int getCurrentWeaponIndex()
    {
        return weapons.IndexOf(currentWeapon);
    }

    /// <summary>
    /// Setzt die aktuell ausgerüstete Waffe
    /// </summary>
    /// <param name="index">Der Index, an dem die Waffe in der Liste steht</param>
    public void switchWeapon(int index)
    {
        if (weapons.Count < index + 1 || getCurrentWeaponIndex() == index) return;

        Weapon oldWeapon = currentWeapon;
        Weapon newWeapon = weapons[index];
        currentWeapon = null;

        Sequence weaponSwitchStart = DOTween.Sequence();
        weaponSwitchStart.Append(oldWeapon.transform.DOLocalMove(new Vector3(0,-1f, -.5f), .5f));
        weaponSwitchStart.Join(oldWeapon.transform.DOLocalRotate(new Vector3(90, 0, 0), .5f));

        weaponSwitchStart.OnComplete(() =>
        {
            oldWeapon.gameObject.SetActive(false);
            weapons[index].transform.rotation = oldWeapon.transform.rotation;
            weapons[index].transform.localPosition = oldWeapon.transform.localPosition;
            weapons[index].gameObject.SetActive(true);

            Sequence weaponSwitchEnd = DOTween.Sequence();
            weaponSwitchEnd.Append(newWeapon.transform.DOLocalMove(new Vector3(0, 0, 0), .5f));
            weaponSwitchEnd.Join(newWeapon.transform.DOLocalRotate(new Vector3(0, 0, 0), .5f));

            weaponSwitchEnd.OnComplete(() =>
            {
                currentWeapon = newWeapon;
            });
        });
    }
}
