using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RayPointer : MonoBehaviour
{
    public static RayPointer left, right;

    public LineRenderer lR;
    public GameObject cursor;
    public SideLR side;
    public OVRInput.Button triggerButton;
    public OVRInput.Touch triggerTouch;
    public OVRInput.Button grabButton;
    public OVRInput.Axis2D stick;

    public UnityEvent<SideLR, Vector3> OnTriggerPressed;
    public UnityEvent<SideLR> OnTriggerTouched;
    public UnityEvent<GameObject> OnPointAt;
    private Vector3 point;
    private GameObject selected;
    private int layerMask;

    private void Awake()
    {
        if (side == SideLR.right)
        {
            right = this;
        }
        else
        {
            left = this;
        }
        CastFloor();
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!RingMenu.GetRingMenu(side).IsOpen())
        {
            lR.enabled = true;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                lR.SetPosition(0, transform.position);
                lR.SetPosition(1, hit.point);

                cursor.SetActive(true);
                cursor.transform.position = hit.point;
                cursor.transform.LookAt(cursor.transform.position + hit.normal);

                point = hit.point;

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
                selected = null;
                OnPointAt.Invoke(null);

                cursor.SetActive(false);

                lR.SetPosition(0, transform.position);
                lR.SetPosition(1, transform.position + transform.TransformDirection(Vector3.forward) * 100000f);
            }

            if (OVRInput.GetDown(triggerButton))
            {
                Debug.Log("Before Pressed trigger");
                OnTriggerPressed.Invoke(side, point);
                Debug.Log("Pressed trigger");
            }

            if (OVRInput.GetDown(triggerTouch))
            {
                OnTriggerTouched.Invoke(side);
            }

        }
        else
        {
            lR.enabled = false;
            cursor.SetActive(false);
        }
    }

    public void CastNormal()
    {
        layerMask = 1 << 2;
        layerMask = ~layerMask;
    }

    public void CastFloor()
    {
        layerMask = 1 << 3;
    }

    public Vector3 GetPoint()
    {
        return point;
    }

    public Vector2 GetAxis()
    {
        return OVRInput.Get(stick);
    }

    public GameObject GetSceneAsset()
    {
        return selected;
    }

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
