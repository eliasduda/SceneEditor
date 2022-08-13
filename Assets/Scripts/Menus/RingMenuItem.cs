using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RingMenuItem : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject icon;
    public Modifier modifier;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube(transform.position, Vector3.one);
    }
}
