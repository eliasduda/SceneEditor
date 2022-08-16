using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 
/// Item for the RingMenu class
/// </summary>
public class RingMenuItem : MonoBehaviour
{
    //Text showing the name of this item
    public TextMeshProUGUI text; 
    //object as icon of this item
    public GameObject icon;
    //modifier implements the functionality of this item
    public Modifier modifier;

}
