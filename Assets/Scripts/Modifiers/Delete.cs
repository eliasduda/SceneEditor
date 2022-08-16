using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Modifier that allows to elete any SceneAsset selected by the right RayPointer
/// </summary>
public class Delete : Modifier
{
    public Color deleteColor;
    private GameObject selected;
    private Color objColor;
    public override void OnSelectedEnd()
    {
        RayPointer.right.OnTriggerPressed.RemoveListener(Select); 
        RayPointer.right.OnPointAt.RemoveListener(Point);
        selected = null;
    }

    public override void OnSelectedStart()
    {
        //listen to the trigger and SceneAsset selectio of the controller
        RayPointer.right.OnTriggerPressed.AddListener(Select);
        RayPointer.right.OnPointAt.AddListener(Point);
        RayPointer.right.CastNormal();
    }

    public override void OnSelectedUpdate()
    {
    }


    public void Point(GameObject obj)
    {
        if (selected != null)
        {
            selected.GetComponent<MeshRenderer>().material.color = objColor;
        }

        if (obj != null)
        {
            selected = obj;
            objColor = selected.GetComponent<MeshRenderer>().material.color;
            selected.GetComponent<MeshRenderer>().material.color = deleteColor;

        }
        else
        {
            selected = null;
        }

    }

    public void Select(SideLR side, Vector3 point)
    {
        Debug.Log("Selected");
        if (selected != null)
        {
            Debug.Log("Destroying object");
            Destroy(selected.gameObject);
            Debug.Log("Destroyed object");

            selected = null;
        }
    }
}
