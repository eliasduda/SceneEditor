using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Modifier : MonoBehaviour
{

    public abstract void OnSelectedStart();
    public abstract void OnSelectedUpdate();
    public abstract void OnSelectedEnd();

}
