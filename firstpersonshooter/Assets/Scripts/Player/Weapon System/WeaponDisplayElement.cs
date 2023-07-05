using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDisplayElement : MonoBehaviour
{

    public RawImage symbol;
    public TextMeshProUGUI Text;

    public void Init(string text, Texture symbol)
    {
        this.symbol.texture = symbol;
        Text.text = text;
    }
}
