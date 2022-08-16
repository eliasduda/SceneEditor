using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum containing the different sahapes that can be drawn
/// </summary>
public enum DrawShape
{
    Floor, Wall, Sphere
}

/// <summary>
/// Moifier that allows to draw a specified shape by setting multiple points with the Raypointer by pressing the trigger
/// </summary>
public class Draw : Modifier
{
    public DrawShape shape;
    private int points = 0;
    private Vector3[] p = new Vector3[4];
    private GameObject currentShape;
    private SideLR currentSide;
    public GameObject defaultShape;
    public float lineThickness;
    public override void OnSelectedEnd()
    {
        FinishShape();
        RayPointer.left.OnTriggerPressed.RemoveListener(AddPoint);
        RayPointer.right.OnTriggerPressed.RemoveListener(AddPoint);
    }

    public override void OnSelectedStart()
    {
        RayPointer.left.OnTriggerPressed.AddListener(AddPoint);
        RayPointer.right.OnTriggerPressed.AddListener(AddPoint);
        RayPointer.right.CastFloor();
        RayPointer.left.CastFloor();
    }

    public override void OnSelectedUpdate()
    {
        //update the shape with the current "pointed at" point of the Raypointer
        //point index and shape determine how the shape is modified
        if (points == 1)
        {
            Vector3 endPosition = RayPointer.GetPointer(currentSide).GetPoint();

            Vector3 x = (endPosition - currentShape.transform.position);
            float width = x.magnitude;
            float rotation = Vector3.Angle(Vector3.right, new Vector3(x.x, 0, x.z));
            if (endPosition.z > currentShape.transform.position.z)
            {
                rotation *= -1;
            }
            if (shape == DrawShape.Sphere)
            {
                currentShape.transform.localScale = new Vector3(width*2, width*2, width*2);
            }
            else
            {
                currentShape.transform.localScale = new Vector3(width, currentShape.transform.localScale.y, lineThickness);
                currentShape.transform.rotation = Quaternion.Euler(0, rotation, 0);
            }

            p[points] = endPosition;
        }
        else if (points == 2)
        {
            if (shape == DrawShape.Sphere)
            {
                FinishShape();
            }
            Vector3 endPosition = RayPointer.GetPointer(currentSide).GetPoint();
            Vector3 depthDir = Vector3.Cross((p[points-1] - currentShape.transform.position), Vector3.up).normalized;
            Vector3 cPosDir = endPosition - p[points-1];
            Vector3 dir = Vector3.Dot(cPosDir, depthDir) * depthDir;
            float depth = dir.magnitude;
            if(Vector3.Dot(cPosDir, depthDir) < 0)
            {
                depth *= -1;
            }
            p[points] = p[points - 1] + depthDir * depth;
            currentShape.transform.localScale = new Vector3(currentShape.transform.localScale.x, currentShape.transform.localScale.y, depth);
        }
        else if (points == 3)
        {
            if (shape == DrawShape.Floor)
            {
                FinishShape();
            }
            Vector3 pPoint = p[points - 1] + (Vector3.up) * RayPointer.GetPointer(currentSide).transform.position.y;
            Vector3 lPoint = RayPointer.GetPointer(currentSide).transform.position;
            Vector3 pTol = lPoint - pPoint;
            Vector3 pNormal = pTol.normalized;
            Vector3 lNormal = RayPointer.GetPointer(currentSide).transform.TransformDirection(Vector3.forward);
            float a = Vector3.Dot((pPoint - lPoint), pNormal);
            float b = Vector3.Dot(lNormal, pNormal);
            Vector3 intersec = (a / b) * lNormal + lPoint;

            float ak = (pPoint - transform.position).magnitude;
            float angle = Mathf.Rad2Deg *RayPointer.GetPointer(currentSide).transform.rotation.x;
            float ht = ak / (Mathf.Sin(angle));
            float height = Mathf.Sqrt(-Mathf.Pow(ak, 2) + Mathf.Pow(ht, 2));

            currentShape.transform.localScale = new Vector3(currentShape.transform.localScale.x, intersec.y, currentShape.transform.localScale.z);

        }
        else if (points == 4)
        {
            FinishShape();
        }
    }

    //add a new point to the shape
    public void AddPoint(SideLR side, Vector3 point)
    {
        //if first point
        if(points == 0)
        {
            currentSide = side;

            //instantiate the shape at the points position
            p[points] = point;
            currentShape = Instantiate(defaultShape);
            currentShape.layer = 2;
            currentShape.transform.position = point;
            currentShape.transform.localScale = new Vector3(lineThickness, lineThickness, lineThickness);
        }
        if (side == currentSide)
        {
            points++; 
        }
    }

    //finish the current shape (new shape can be started)
    public void FinishShape()
    {
        if (currentShape != null)
        {
            currentShape.layer = 0;
            currentShape = null;
        }
        points = 0;
    }

}
