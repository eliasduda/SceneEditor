using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    public OVRInput.Button zoomIn, zoomOut;
    public GameObject rig;
    private float scale, minScale;
    // Start is called before the first frame update
    void Start()
    {
        minScale = rig.transform.localScale.x;
        scale = minScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(zoomIn))
        {
            scale *= 2;
            rig.transform.localScale = new Vector3(scale, scale, scale);
        }else if (OVRInput.GetDown(zoomOut))
        {
            if (scale * 0.5 > minScale)
            {
                scale *= 0.5f;
            }
            rig.transform.localScale = new Vector3(scale, scale, scale);

        }
    }
}
