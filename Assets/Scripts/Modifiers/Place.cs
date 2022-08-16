using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Modifier that allows the Placement of a defined Gameobject at the position of the right Raypointer's point
/// also allows scaleing or rotation via the stick
/// </summary>
public class Place : Modifier
{
    public GameObject obj;
    private GameObject currentObj;
    private float size = 1;
    private SideLR activeSide;
    private float rotation = 0;
    private float threshold = 0.5f;

    public override void OnSelectedEnd()
    {
        if(currentObj != null)
        {
            Destroy(currentObj);
            currentObj = null;
            rotation = 0f;
            threshold = 0f;
        } 
        RayPointer.right.OnTriggerPressed.RemoveListener(PlaceObject);
    }

    public override void OnSelectedStart()
    {
        activeSide = SideLR.right;
        RayPointer.right.OnTriggerPressed.AddListener(PlaceObject);
        RayPointer.right.CastFloor();
    }

    public override void OnSelectedUpdate()
    {
        if(currentObj == null)
        {
            currentObj = Instantiate(obj, RayPointer.GetPointer(activeSide).GetPoint(), Quaternion.identity);
            currentObj.layer = 2;
        }
        else
        {
            currentObj.transform.position = RayPointer.GetPointer(activeSide).GetPoint();
        }

        Vector2 axis = RayPointer.GetPointer(activeSide).GetAxis();
        if (Mathf.Abs(axis.y) > threshold)
        {
            size += axis.y*0.05f;
            if(size < 0.01f)
            {
                size = 0.01f;
            }
        }
        if (Mathf.Abs(axis.x) > threshold)
        {
            rotation += axis.x;
        }
        currentObj.transform.localScale = new Vector3(size, size, size);
        currentObj.transform.rotation = Quaternion.Euler(0, rotation, 0);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TouchedTrigger(SideLR side)
    {
       // RayPointer.GetPointer(activeSide).OnTriggerPressed.RemoveListener(PlaceObject);
       // activeSide = side; 
       // RayPointer.GetPointer(activeSide).OnTriggerPressed.RemoveListener(PlaceObject);
    }

    public void PlaceObject(SideLR side, Vector3 point)
    {
        currentObj.layer = 0;
        currentObj = null;
        rotation = 0f;
        threshold = 0f;
    }
}
