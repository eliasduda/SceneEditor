using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRInput;

/// <summary>
/// A Ringmenu that contsists of RingmenuItems arranged in a circle and RingmenuBrs that form the backround ring itself
/// The items are selected via a stick
/// </summary>
public class RingMenu : MonoBehaviour
{
    //acces to the two static Menus (one per hand)
    public static RingMenu rightMenu, leftMenu;

    public float radius;
    private int itemCount;
    //Material for the Background
    public Material ringMaterial;
    //Items 
    public RingMenuItem[] items;
    //Item Backgrounds
    private RingMenuItemBR[] itemBrs;
    //directoins of the items
    private Vector2[] directions;
    private int currentlySeleted = -1;
    //Button to open the menu
    public OVRInput.Button openButton;
    //stick to select a item
    public Axis2D stick;
    //sie of this menu
    public SideLR Side;
    private bool isOpen = false;
    //segmenst of the ringmenu
    public int ringResolution = 20;


    //time for the open and close animation
    public float OpenTime;
    private float TIMER;
    private bool anim = false;
    Vector3 beginState, endState;
    private GameObject contents;

    // Start is called before the first frame update
    void Start()
    {
        //define static Menus dependent on side
        if (Side == SideLR.right)
        {
            rightMenu = this;
        }
        else
        {
            leftMenu = this;
        }
        //setup menu wih current items
        SetupMenu();
        transform.localScale = Vector3.zero;
        contents.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //handle open and close
        if (OVRInput.GetDown(openButton))
        {
            Open();
        }
        else if (OVRInput.GetUp(openButton))
        {
            Close();
        }

        //handle item selection
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
        //update the currently selected modifier
        if(currentlySeleted != -1 && !isOpen)
        {
            items[currentlySeleted].modifier.OnSelectedUpdate();
        }

        //handle open and close animation
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

    //Sets up the menu with the current items
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

    //starts the open animation
    public void Open()
    {
        isOpen = true;
        contents.SetActive(true);

        anim = true;
        TIMER = 0;
        beginState = transform.localScale;
        endState = Vector3.one;
    }

    //starts the close animation
    public void Close()
    {
        isOpen = false;

        anim = true;
        TIMER = 0;
        beginState = transform.localScale;
        endState = Vector3.zero;
    }


    //debug draw menu gizmos in editor
    private void OnDrawGizmos()
    {
        if (items.Length > 0)
        {
            itemCount = items.Length;
            DrawCircle(100);
            DrawCircle(itemCount, false);
        }

    }

    //Debug draw a circle
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

    //get the position of any item in the menu
    private Vector3 GetItemPos(float itemNum, float width = 0f)
    {
        Vector3 rV = new Vector3(radius +width, 0, 0);
        Quaternion step = Quaternion.AngleAxis((360 / itemCount)* itemNum, new Vector3(0, 0, 1));
        return transform.position + transform.rotation * (step * rV);
    }

    //get the 2d direction where any item is located
    private Vector2 GetItemDirecion(float itemNum)
    {
        Vector3 rV = new Vector3(radius, 0, 0);
        Vector3 step = Quaternion.AngleAxis((360 / itemCount) * itemNum, new Vector3(0, 0, 1)) * rV;
        return new Vector2(step.x, step.y);
    }

    //Create a ring segment mesh for the item background
    private RingMenuItemBR MakeItemMesh(int itemNum)
    {
        GameObject go = new GameObject("ItemBackground"+itemCount);
        RingMenuItemBR br = go.AddComponent<RingMenuItemBR>();
        go.transform.parent = contents.transform;
        Mesh mesh = new Mesh();
        int segments = ringResolution / items.Length;
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];
        vertices[0] = transform.position; 
        vertices[1] = GetItemPos(itemNum - 0.5f, radius);
        float vPos = -0.5f;
        for (int i = 0; i<segments; i++)
        {
            vPos += 1.0f/segments;
            Vector3 pos = GetItemPos(itemNum + vPos, radius);
            vertices[i + 2] = pos;
            triangles[i*3] = 0;
            triangles[i * 3+1] = i + 1;
            triangles[i * 3+2] = i + 2;
        }

        vertices[segments+1] = GetItemPos(itemNum + 0.5f, radius);

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

    //find the item a given direction is pointing at
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

    //deselect the current item
    public void EndCurrentItem()
    {
        if (currentlySeleted != -1)
        {
            items[currentlySeleted].modifier.OnSelectedEnd();
        }
    }

    //gets the static ringmenu of a given side
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

/// <summary>
/// Background containing a ring segment mesh 
/// Handles selection via the animation of the material alpha
/// </summary>
public class RingMenuItemBR : MonoBehaviour
{
    //material of the background
    public Material mat;
    //ring segment mesh
    public Mesh mesh;
    //Transparency when selecte and when not selected
    public float minTrans = 0.4f, maxTrans = 0.7f;
    private float currentMin, currentMax;
    private float TIMER, selectTime = 0.2f;
    private bool anim = false;

    private void Update()
    {
        //hanle select animation 
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

    //select this item
    public void Select()
    {
        anim = true;
        TIMER = 0;
        currentMin = mat.color.a;
        currentMax = maxTrans;
    }

    //deselect thie item
    public void Deselect()
    {
        anim = true;
        TIMER = 0;
        currentMin = mat.color.a;
        currentMax = minTrans;
    }
}

/// <summary>
/// Enum for the controller sides
/// </summary>
public enum SideLR{
    left, right
}
