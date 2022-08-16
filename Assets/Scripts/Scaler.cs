using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller that scales the player up or down
/// </summary>
public class Scaler : MonoBehaviour
{
    //Buttons for upscale(zoomout) or downscale(zoomin)
    public OVRInput.Button zoomIn, zoomOut;
    //the player to scale
    public GameObject player;
    //current scale and minimum scale
    private float scale, minScale;


    // Start is called before the first frame update
    void Start()
    {
        //set current playerscale as minscale
        minScale = player.transform.localScale.x;
        scale = minScale;
    }

    // Update is called once per frame
    void Update()
    {
        //if buttons are pressed upscale times 2 or downscale times 0.5
        if (OVRInput.GetDown(zoomIn))
        {
            scale *= 2;
            player.transform.localScale = new Vector3(scale, scale, scale);
        }else if (OVRInput.GetDown(zoomOut))
        {
            if (scale * 0.5 > minScale)//not smaller than minScale
            {
                scale *= 0.5f;
            }
            player.transform.localScale = new Vector3(scale, scale, scale);

        }
    }
}
