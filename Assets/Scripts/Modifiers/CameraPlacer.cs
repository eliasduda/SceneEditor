using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlacer : Modifier
{
    public GameObject defaultCamera;
    public MeshRenderer plane;
    public Vector2Int textureResolution;
    private GameObject currentCam;
    private float currentFOV = 60;

    public override void OnSelectedEnd()
    {
        if (currentCam != null)
        {
            Destroy(currentCam);
        }
        RayPointer.right.OnTriggerPressed.RemoveListener(AddPoint);
    }

    public override void OnSelectedStart()
    {
        RayPointer.right.OnTriggerPressed.AddListener(AddPoint);
        SetupCam();

    }

    public override void OnSelectedUpdate()
    {
        if (currentCam != null)
        {
            currentCam.transform.position = RayPointer.right.transform.position;
            currentCam.transform.rotation = RayPointer.right.transform.rotation;

            Vector2 axis = RayPointer.right.GetAxis();
            if (currentFOV > 0 && currentFOV < 180)
            {
                currentFOV += axis.y;
            }
            currentCam.GetComponent<Camera>().fieldOfView = currentFOV;
        }
        else
        {
            SetupCam();
        }
    }

    public void AddPoint(SideLR side, Vector3 point)
    {
        if (currentCam != null)
        {
            currentCam = null;
        }
    }

    public void SetupCam()
    {
        currentCam = Instantiate(defaultCamera, RayPointer.right.transform.position, RayPointer.right.transform.rotation);
        plane = currentCam.GetComponentInChildren<MeshRenderer>();
        RenderTexture rt = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        rt.Create();
        currentCam.GetComponent<Camera>().targetTexture = rt;
        plane.material.mainTexture = rt;
    }

}
