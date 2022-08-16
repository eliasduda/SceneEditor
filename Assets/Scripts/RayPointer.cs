using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simple VR ray pointer controller
/// </summary>
public class RayPointer : MonoBehaviour
{
    //access both static controllers
    public static RayPointer left, right;

    //renders the rey
    public LineRenderer lR;
    //cursor at the end of the ray
    public GameObject cursor;
    //defines the sie of this controller
    public SideLR side;
    
    //Buttons and Sticks
    public OVRInput.Button triggerButton;
    public OVRInput.Button grabButton;
    public OVRInput.Axis2D stick;

    //Event thrown when Trigger gets Pressed
    public UnityEvent<SideLR, Vector3> OnTriggerPressed;
    //Event gets thrown when Pointing at a SceneAsset object
    public UnityEvent<GameObject> OnPointAt;
    //the point where the ray landed last
    private Vector3 point;
    //the SceneAsset that was last selected
    private GameObject selected;
    //the current layermask for the raycast
    private int layerMask;

    private void Awake()
    {
        //define static controller depending on side
        if (side == SideLR.right)
        {
            right = this;
        }
        else
        {
            left = this;
        }
        //sets layermask to ast only on the floorplane
        CastFloor();
    }

    // Update is called once per frame
    void Update()
    {
        //only works when he Ringmenu on that side is not open 
        if (!RingMenu.GetRingMenu(side).IsOpen())
        {
            //enables visuals
            lR.enabled = true;
            RaycastHit hit;

            //cast ray
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                //update visuals (cursor+lineRenderer)
                lR.SetPosition(0, transform.position);
                lR.SetPosition(1, hit.point);

                cursor.SetActive(true);
                cursor.transform.position = hit.point;
                cursor.transform.LookAt(cursor.transform.position + hit.normal);

                point = hit.point; // save hit point

                //check for hits with SceneAssets
                if (hit.collider.gameObject.GetComponent<SceneAsset>())
                {
                    if(hit.collider.gameObject.GetComponent<SceneAsset>() != selected)
                    {
                        selected = hit.collider.gameObject.GetComponent<SceneAsset>().gameObject;
                        OnPointAt.Invoke(selected.gameObject);
                    }
                }
                else
                {
                    selected = null;
                    OnPointAt.Invoke(null);
                }

            }
            else
            {
                //deactivate visuals when nothing is hit
                selected = null;
                OnPointAt.Invoke(null);

                cursor.SetActive(false);

                lR.SetPosition(0, transform.position);
                lR.SetPosition(1, transform.position + transform.TransformDirection(Vector3.forward) * 100000f);
            }

            //throw Event when trigger is pressed
            if (OVRInput.GetDown(triggerButton))
            {
                Debug.Log("Before Pressed trigger");
                OnTriggerPressed.Invoke(side, point);
                Debug.Log("Pressed trigger");
            }


        }
        else
        {
            //deactivate visuals when menu is open
            lR.enabled = false;
            cursor.SetActive(false);
        }
    }

    //cast to every layer (exept ignorRayCast layer)
    public void CastNormal()
    {
        layerMask = 1 << 2;
        layerMask = ~layerMask;
    }

    //only casts to the floor layer
    public void CastFloor()
    {
        layerMask = 1 << 3;
    }

    //get last hit point
    public Vector3 GetPoint()
    {
        return point;
    }

    //Get Axis of the Stick
    public Vector2 GetAxis()
    {
        return OVRInput.Get(stick);
    }

    //Get last hit SceneAsset
    public GameObject GetSceneAsset()
    {
        return selected;
    }

    //Get the static controller of a given side
    public static RayPointer GetPointer(SideLR side)
    {
        if(side == SideLR.right)
        {
            return right;
        }
        else
        {
            return left;
        }
    }
}
