using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRInput;

public class RingMenu : MonoBehaviour
{
    public static RingMenu rightMenu, leftMenu;

    public float radius;
    private int itemCount;
    public Material ringMaterial;
    public RingMenuItem[] items;
    private RingMenuItemBR[] itemBrs;
    private Vector2[] directions;
    private int currentlySeleted = -1;
    public OVRInput.Button openButton;
    public Axis2D stick;
    public SideLR Side;
    private bool isOpen = false;


    public float OpenTime;
    private float TIMER;
    private bool anim = false;
    Vector3 beginState, endState;
    private GameObject contents;
    public bool test = false;

    // Start is called before the first frame update
    void Start()
    {
        if (Side == SideLR.right)
        {
            rightMenu = this;
        }
        else
        {
            leftMenu = this;
        }
        SetupMenu();
        transform.localScale = Vector3.zero;
        contents.SetActive(false);
        if (test)
        {
            Open();
            currentlySeleted = 0;
            items[0].modifier.OnSelectedStart();
            items[0].modifier.OnSelectedUpdate();
            items[0].modifier.OnSelectedEnd();
            currentlySeleted = 1;
            items[1].modifier.OnSelectedStart();
            items[1].modifier.OnSelectedUpdate();
            items[1].modifier.OnSelectedEnd();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(openButton))
        {
            Open();
        }
        else if (OVRInput.GetUp(openButton))
        {
            Close();
        }

        if (isOpen && OVRInput.Get(stick).magnitude > 0.1f)
        {

            int idx = ClosestItem(OVRInput.Get(stick));
            Debug.Log("inedx is " + idx);

            if (idx != currentlySeleted)
            {
                if (currentlySeleted != -1)
                {
                    itemBrs[currentlySeleted].Deselect();
                    Debug.Log("deselected last");
                    items[currentlySeleted].modifier.OnSelectedEnd();
                    Debug.Log("selection ended last");
                }
                currentlySeleted = idx;
                itemBrs[currentlySeleted].Select();
                Debug.Log("selected " + items[currentlySeleted]);
                items[currentlySeleted].modifier.OnSelectedStart();
            }
        }
        if(currentlySeleted != -1 && !isOpen)
        {
            items[currentlySeleted].modifier.OnSelectedUpdate();
        }

        if (anim)
        {
            TIMER += Time.deltaTime;
            transform.localScale = Vector3.Lerp(beginState, endState, TIMER / OpenTime);
            if (TIMER > OpenTime)
            {
                anim = false;

                if(transform.localScale == Vector3.zero)
                {
                    contents.SetActive(false);

                }
            }
        }
    }

    public void SetupMenu()
    {
        currentlySeleted = -1;
        Vector3 scale = transform.localScale;
        if (contents != null)
        {
            Destroy(contents);
        }
        transform.localScale = Vector3.one;
        contents = new GameObject("contents");
        contents.transform.position = transform.position;
        contents.transform.parent = this.transform;

        itemCount = items.Length;
        itemBrs = new RingMenuItemBR[itemCount];
        directions = new Vector2[itemCount];

        for (int i = 0; i < itemCount; i++)
        {
            itemBrs[i] = MakeItemMesh(i);
            directions[i] = GetItemDirecion(i);
            Instantiate(items[i], GetItemPos(i), transform.rotation, contents.transform);
        }
        transform.localScale = scale;
    }

    public void Open()
    {
        isOpen = true;
        contents.SetActive(true);

        anim = true;
        TIMER = 0;
        beginState = transform.localScale;
        endState = Vector3.one;
    }

    public void Close()
    {
        isOpen = false;

        anim = true;
        TIMER = 0;
        beginState = transform.localScale;
        endState = Vector3.zero;
    }


    private void OnDrawGizmos()
    {
        if (items.Length > 0)
        {
            itemCount = items.Length;
            DrawCircle(100);
            DrawCircle(itemCount, false);
        }

    }

    private void DrawCircle(int steps, bool lines = true)
    {
        Vector3 rV = new Vector3(radius, 0, 0);
        Quaternion step = Quaternion.AngleAxis(360 / steps, new Vector3(0, 0, 1));
        Vector3 oldV = (transform.position + rV);
        for (int i = 0; i < steps; i++)
        {
            rV = (step * rV);
            Vector3 newV = (transform.position + transform.rotation * rV);
            if (lines)
            {
                Gizmos.DrawLine(oldV, newV);
            }
            else
            {
                Gizmos.DrawSphere(oldV, 0.02f);
            }
            oldV = newV;
        }
    }

    private float Circum()
    {
        return radius * 2 * Mathf.PI; 
    }

    private Vector3 GetItemPos(float itemNum, float width = 0f)
    {
        Vector3 rV = new Vector3(radius +width, 0, 0);
        Quaternion step = Quaternion.AngleAxis((360 / itemCount)* itemNum, new Vector3(0, 0, 1));
        return transform.position + transform.rotation * (step * rV);
    }

    private Vector2 GetItemDirecion(float itemNum)
    {
        Vector3 rV = new Vector3(radius, 0, 0);
        Vector3 step = Quaternion.AngleAxis((360 / itemCount) * itemNum, new Vector3(0, 0, 1)) * rV;
        return new Vector2(step.x, step.y);
    }

    private RingMenuItemBR MakeItemMesh(int itemNum)
    {
        GameObject go = new GameObject("ItemBackground"+itemCount);
        RingMenuItemBR br = go.AddComponent<RingMenuItemBR>();
        go.transform.parent = contents.transform;
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[3];
        int[] triangles = new int[3];
        vertices[0] = transform.position;
        vertices[1] = GetItemPos(itemNum-0.5f, radius);
        vertices[2] = GetItemPos(itemNum+0.5f, radius);
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        br.mesh = mesh;
        ringMaterial.color = new Color(ringMaterial.color.r, ringMaterial.color.g, ringMaterial.color.b, br.minTrans);
        go.AddComponent<MeshRenderer>().material = ringMaterial;
        br.mat = go.GetComponent<MeshRenderer>().material;
        go.AddComponent<MeshFilter>().mesh = mesh;
        return br;
    }

    public int ClosestItem(Vector2 stickDir)
    {
        var maxDot = -Mathf.Infinity;
        int ret = 0;
        int i = 0;

        foreach (Vector2 dir in directions)
        {
            var t = Vector3.Dot(stickDir, dir);
            if (t > maxDot)
            {
                ret = i;
                maxDot = t;
            }
            i++;
        }

        return ret;
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    public void EndCurrentItem()
    {
        if (currentlySeleted != -1)
        {
            items[currentlySeleted].modifier.OnSelectedEnd();
        }
    }

    public static RingMenu GetRingMenu(SideLR side)
    {
        if(side == SideLR.right)
        {
            return rightMenu;
        }
        else
        {
            return leftMenu;
        }
    }

}

public class RingMenuItemBR : MonoBehaviour
{
    public Material mat;
    public Mesh mesh;
    public float minTrans = 0.4f, maxTrans = 0.7f;
    private float currentMin, currentMax;
    private float TIMER, selectTime = 0.2f;
    private bool anim = false;

    private void Update()
    {
        if (anim)
        {
            TIMER += Time.deltaTime;
            float alpha =  Mathf.Lerp(currentMin, currentMax, TIMER / selectTime);
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
            if (TIMER > selectTime)
            {
                anim = false;
            }
        }
    }

    public void Select()
    {
        anim = true;
        TIMER = 0;
        currentMin = mat.color.a;
        currentMax = maxTrans;
    }

    public void Deselect()
    {
        anim = true;
        TIMER = 0;
        currentMin = mat.color.a;
        currentMax = minTrans;
    }
}

public enum SideLR{
    left, right
}
