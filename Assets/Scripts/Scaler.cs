using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    public OVRInput.Button zoomIn, zoomOut;
    public GameObject rig;
    public float scaleMuliplier;
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
        if (OVRInput.Get(zoomIn) && scale > minScale)
        {
            scale *= 1-scaleMuliplier;
            rig.transform.localScale = new Vector3(scale, scale, scale);
        }else if (OVRInput.Get(zoomOut))
        {
            scale *= 1 + scaleMuliplier;
            rig.transform.localScale = new Vector3(scale, scale, scale);

        }
    }
}
