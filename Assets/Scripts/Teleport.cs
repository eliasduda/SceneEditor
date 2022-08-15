using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject player, teleportVisual;
    private bool tp = false;
    public OVRInput.Axis2D turn;
    public float turnThreshold = 0.5f, teleportthreshold = 0.6f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!RingMenu.leftMenu.IsOpen())
        {
            if (Mathf.Abs(OVRInput.Get(turn).y) > teleportthreshold)
            {
                if (tp)
                {
                    teleportVisual.transform.position = RayPointer.left.GetPoint();
                }
                else
                {
                    tp = true;
                    teleportVisual.SetActive(true);
                }
            }
            else
            {
                if (tp)
                {
                    player.transform.position = RayPointer.left.GetPoint();
                    teleportVisual.SetActive(false);
                    tp = false;
                }
                else
                {

                    if (Mathf.Abs(OVRInput.Get(turn).x) > turnThreshold)
                    {
                        player.transform.Rotate(new Vector3(0, OVRInput.Get(turn).x, 0));
                    }
                }
            }

        }
    }
}
