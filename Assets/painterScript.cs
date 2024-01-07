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
using TMPro;

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

    // For change brush (and eraser) buttons
    [SerializeField]
    private TextMeshProUGUI ModeButtonGUI;

    [SerializeField]
    private int mode;

    // Color Picker
    public bool getStartingColorFromMaterial;
    public FlexibleColorPicker fcp;
    public Color CurrentColor;

    // Took the variable inits outside of the Start() and fixed the processing system so it actually works right
    // Dont change

    private Dictionary<Texture2D, Texture2D> textureDict = new Dictionary<Texture2D, Texture2D>();
    private Dictionary<Texture2D, Texture2D> reversedTextureDict = new Dictionary<Texture2D, Texture2D>();
    private List<Texture2D> dictKeys;
    private string[] brushes = {"Paint", "Erase"};


    void Start()
    {
        fcp.onColorChange.AddListener(OnChangeColor);

        // Process Model
        Transform trainsform = Model.transform;
        //get zombie model
        foreach (Transform childTransform in trainsform){
            if (childTransform.gameObject.GetComponent<SkinnedMeshRenderer>() != null && childTransform != trainsform){
                childTransform.transform.position = trainsform.position;
                //Go through each child gameObject and see if have SkinnedMeshRenderer
                //if it does, add a meshcollider using the mesh from the SkinnedMeshRenderer
                
                MeshCollider collider = childTransform.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
                SkinnedMeshRenderer skinnedMeshRenderer = childTransform.gameObject.GetComponent<SkinnedMeshRenderer>();
                collider.sharedMesh = skinnedMeshRenderer.sharedMesh;
            }
            
            //For further development, change texture to be writable (by cloning texture to seperate one, for reverting and saving also)
            // Note: I think we did the above already

            // Will check if texture was already clone so we don't clone textures more than once
            if(childTransform.gameObject.GetComponent<Renderer>() != null){
                Material material = childTransform.gameObject.GetComponent<Renderer>().material;
                Texture2D mainTexture = (Texture2D)material.mainTexture;
                dictKeys = new List<Texture2D>(textureDict.Keys);
                if (dictKeys.Contains(mainTexture)){
                    material.SetTexture("_MainTex", textureDict[mainTexture]);
                    Debug.Log("repeats");
                }else{
                    Texture2D clone = new Texture2D(mainTexture.width, mainTexture.height);
                    clone.name = mainTexture.name + "clone";
                    clone.SetPixels(mainTexture.GetPixels());
                    clone.Apply();
                    textureDict.Add(mainTexture, clone);
                    material.SetTexture("_MainTex",clone);
                }
                
            }
        }
        dictKeys = new List<Texture2D>(textureDict.Keys);
        // Create reversed dictionary for erase lookup
        foreach (var key in dictKeys){
            reversedTextureDict.Add(textureDict[key], key);
        }

    }



    public void Update()
    {
        if (mode != 1)
        {
            if (getStartingColorFromMaterial)
            {
                fcp.color = CurrentColor;
            }
        }
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            TouchRay = cam.ScreenPointToRay(touch.position);

            if (!Physics.Raycast(TouchRay, out hit))
            {
                Debug.Log("Not hit");
                return;
            }
            ApplyPaintToHitpoint(hit);
        }
        

        /*
        // Code for testing on PC
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Debug.Log("Not hit");
                return;
            }
            ApplyPaintToHitpoint(hit);
        }
        */

        // Original code -> moved to function
        /*
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Debug.Log("Not hit");
                return;
            }

            Texture2D tex = rend.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;

            //Color BaseColor = tex.GetPixels((int)pixelUV.x, (int)pixelUV.y);
            //tex.SetPixel((int)pixelUV.x, (int)pixelUV.y, CurrentColor);

            tex.SetPixels32((int)pixelUV.x, (int)pixelUV.y, BrushSize, height, colors);
            tex.Apply();
        }
        */
    }

    private void ApplyPaintToHitpoint(RaycastHit hit)
    {
        // Get Editing textures
        Renderer rend = hit.transform.GetComponent<Renderer>();

            MeshCollider meshCollider = hit.collider as MeshCollider;

            if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
            {
                Debug.Log("Unable to access textures");
                return;
            }

        Texture2D tex = rend.material.mainTexture as Texture2D;

        // Get texture coords
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;

        // Generate Brush
        colors = new Color32[BrushSize * BrushSize];
        for (int i = 0; i < BrushSize; i++)
        {
            for (int j = 0; j < BrushSize; j++)
            {
                if (mode == 1)
                {
                    colors[i * BrushSize + j] = reversedTextureDict[tex].GetPixel((int) pixelUV.x + j, (int) pixelUV.y + i);
                }
                else
                {
                    colors[i * BrushSize + j] = CurrentColor;
                }
            }  
        }

        // Apply textures
        tex.SetPixels32((int)pixelUV.x, (int)pixelUV.y, BrushSize, BrushSize, colors);
        tex.Apply();
    }

    private void OnChangeColor(Color co)
    {
        CurrentColor = co;
    }

    // Change Brush code
    public void ChangeBrush()
    {
        if (mode == brushes.Length - 1)
        {
            mode = 0;
        }
        else
        {
            mode += 1;
        }
        ModeButtonGUI.text = brushes[mode];
    }
}