using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple Teleportation via Tumbstick up and turning via Thumbstick left/right
/// </summary>
public class Teleport : MonoBehaviour
{
    //the player to be teleported
    public GameObject player;

    //the indicator that shows wehere to teleport
    public GameObject teleportVisual;

    //active when thumbstick up is held
    private bool tp = false;

    //the thumbstik to turn and teleport
    public OVRInput.Axis2D turn;
    //threshold for hen turn or teleport is triggered
    public float turnThreshold = 0.5f, teleportthreshold = 0.6f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //only works when the left Ringmenu is closed
        if (!RingMenu.leftMenu.IsOpen())
        {
            //if teleport button is held
            if (Mathf.Abs(OVRInput.Get(turn).y) > teleportthreshold)
            {
                if (tp)
                {
                    //update the visual indicator
                    teleportVisual.transform.position = RayPointer.left.GetPoint();
                }
                else
                {
                    //set tp mode to true (to update visual)
                    tp = true;
                    teleportVisual.SetActive(true);
                }
            }
            else
            {
                //if button not held anymore and was in tp mode
                if (tp)
                {
                    //teleport player an deactivate indicator 
                    player.transform.position = RayPointer.left.GetPoint();
                    teleportVisual.SetActive(false);
                    tp = false;
                }
                else
                {
                    //if not teleportincg you can continuousely turn left or right
                    if (Mathf.Abs(OVRInput.Get(turn).x) > turnThreshold)
                    {
                        player.transform.Rotate(new Vector3(0, OVRInput.Get(turn).x, 0));
                    }
                }
            }

        }
    }
}
