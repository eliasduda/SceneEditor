using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that sets the current position of each cursor to the Floor Grid Material
/// </summary>
public class Cursor : MonoBehaviour
{
    public Material floor;
    public SideLR side;
    private int num;
    // Start is called before the first frame update
    void Start()
    {
        if(side == SideLR.right)
        {
            num = 0;
        }
        else
        {
            num = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        floor.SetVector("_Cursor" + num, transform.position);
    }
}
