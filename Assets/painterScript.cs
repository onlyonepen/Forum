// Write black pixels onto the GameObject that is located
// by the script. The script is attached to the camera.
// Determine where the collider hits and modify the texture at that point.
//
// Note that the MeshCollider on the GameObject must have Convex turned off. This allows
// concave GameObjects to be included in collision in this example.
//
// Also to allow the texture to be updated by mouse button clicks it must have the Read/Write
// Enabled option set to true in its Advanced import settings.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARCore;

public class painterScript : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private GameObject Model;
    public int BrushSize = 10;
    public float Transparentcy = 1;
    public LayerMask TargetObject;
    private int height;
    public Color32[] colors;
    private Ray TouchRay;

    RaycastHit hit;


    public bool getStartingColorFromMaterial;
    public FlexibleColorPicker fcp;
    public Color CurrentColor;


    void Start()
    {
        fcp.onColorChange.AddListener(OnChangeColor);


        Transform trainsform = Model.transform;
        //get zombie model
        foreach (Transform childTransform in trainsform)
        {
            if (childTransform.gameObject.GetComponent<SkinnedMeshRenderer>() != null && childTransform != trainsform)
            {
                childTransform.transform.position = trainsform.position;
                //Go through each child gameObject and see if have SkinnedMeshRenderer
                //if it does, add a meshcollider using the mesh from the SkinnedMeshRenderer

                MeshCollider collider = childTransform.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
                SkinnedMeshRenderer skinnedMeshRenderer = childTransform.gameObject.GetComponent<SkinnedMeshRenderer>();
                collider.sharedMesh = skinnedMeshRenderer.sharedMesh;
            }

            //For further development, change texture to be writable (by cloning texture to seperate one, for reverting and saving also)
            if (childTransform.gameObject.GetComponent<Renderer>() != null)
            {
                Dictionary<Texture2D, Texture2D> textureDict = new Dictionary<Texture2D, Texture2D>();
                Material material = childTransform.gameObject.GetComponent<Renderer>().material;
                Texture2D mainTexture = (Texture2D)material.mainTexture;
                List<Texture2D> dictKeys = new List<Texture2D>(textureDict.Keys);
                if (dictKeys.Contains(mainTexture))
                {
                    Debug.Log("repeats");
                }
                else
                {
                    Texture2D clone = new Texture2D(mainTexture.width, mainTexture.height);
                    clone.SetPixels(mainTexture.GetPixels());
                    clone.Apply();
                    textureDict.Add(mainTexture, clone);
                    material.SetTexture("_MainTex", clone);
                }

            }

        }

    }



    public void Update()
    {
        height = BrushSize;
        colors = new Color32[BrushSize * height];
        //Generate Brush
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < BrushSize; j++)
            {
                colors[i * BrushSize + j] = CurrentColor;
            }
        }

        if (getStartingColorFromMaterial)
        {
            fcp.color = CurrentColor;
        }

        if (!Input.GetKey(KeyCode.Mouse0))
        { 
            return;
        }


        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            TouchRay = cam.ScreenPointToRay(touch.position);
        }

        //if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
        //{
        //    Debug.Log("Not hit");
        //    return;
        //}

        if (!Physics.Raycast(TouchRay, out hit))
        {
            Debug.Log("Not hit");
            return;
        }

        Renderer rend = hit.transform.GetComponent<Renderer>();

        MeshCollider meshCollider = hit.collider as MeshCollider;

        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
        {
            Debug.Log("Unable to access textures");
            return;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Texture2D tex = rend.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;
            //Color BaseColor = tex.GetPixels((int)pixelUV.x, (int)pixelUV.y);

            //tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, CurrentColor);
            tex.SetPixels32((int)pixelUV.x, (int)pixelUV.y, BrushSize, height, colors);
            tex.Apply();
            Debug.Log("Texture Cordinate : " + pixelUV);
            Debug.Log("Hit Point : " + hit.point);
            Debug.Log("Color : " + CurrentColor);
        }

        

    }

    private void OnChangeColor(Color co)
    {
        CurrentColor = co;
    }
}