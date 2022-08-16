using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Modifier base class 
/// Has Functions that get called by the ringmenu when an item is selected 
/// every item must have a modifier
/// </summary>
public abstract class Modifier : MonoBehaviour
{

    /// <summary>
    /// Called when the item is selected
    /// </summary>
    public abstract void OnSelectedStart();
    /// <summary>
    /// Called every frame when the item is selected
    /// </summary>
    public abstract void OnSelectedUpdate();
    /// <summary>
    /// Called when the item is not selected anymore
    /// </summary>
    public abstract void OnSelectedEnd();

}
