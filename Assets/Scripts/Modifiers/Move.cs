using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moifier that allos to move any SceneAsset selected by the right RayPointer
/// </summary>
public class Move : Modifier
{
    private GameObject selected;
    private bool isMoving;
    private Color objColor;
    public Color moveColor;

    public override void OnSelectedEnd()
    {
        RayPointer.right.OnTriggerPressed.RemoveListener(Select);
        RayPointer.right.OnPointAt.RemoveListener(Point);
        selected = null;
        isMoving = false;
    }

    public override void OnSelectedStart()
    {
        RayPointer.right.OnTriggerPressed.AddListener(Select);
        RayPointer.right.OnPointAt.AddListener(Point);
        RayPointer.right.CastNormal();
    }

    public override void OnSelectedUpdate()
    {
        if (selected != null && isMoving)
        {
            selected.transform.position = RayPointer.right.GetPoint();
        }
    }

    public void Point(GameObject obj)
    {
        if (!isMoving)
        {
            if (selected != null)
            {
                selected.GetComponent<MeshRenderer>().material.color = objColor;
            }

            if (obj != null)
            {
                selected = obj;
                objColor = selected.GetComponent<MeshRenderer>().material.color;
                selected.GetComponent<MeshRenderer>().material.color = moveColor;

            }
            else
            {
                selected = null;
            }
        }
    }

    public void Select(SideLR side, Vector3 point)
    {
        if (selected != null)
        {
            if (isMoving)
            {
                selected.GetComponent<MeshRenderer>().material.color = objColor;
                selected.layer = 0;
                selected = null;
                RayPointer.right.CastNormal();
                isMoving = false;
            }
            else
            {
                RayPointer.right.CastFloor();
                isMoving = true;
            }
        }
    }
}
