using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class TextHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _Text;

    public void SetValue(float value)
    {
        _Text.text = $"{Mathf.FloorToInt(value)}";
    }
}
