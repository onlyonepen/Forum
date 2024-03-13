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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.ARCore;
using TMPro;
using UnityEngine.UI;

public class painterScript : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private GameObject Model;
    public int BrushSize = 10;
    public float Transparentcy = 255;
    public LayerMask TargetObject;
    private int height;
    public Color32[] colors;
    private Ray TouchRay;

    RaycastHit hit;
    bool CanPaint = true;

    // For change brush (and eraser) buttons
    [SerializeField]
    private TextMeshProUGUI ModeButtonGUI;

    [SerializeField]
    private int mode;

    // Color Picker
    public bool getStartingColorFromMaterial;
    public FlexibleColorPicker fcp;
    public Color CurrentColor;

    public GameObject ModeSelector;
    public Sprite Painter;
    public Sprite Eraser;
    public GameObject MoverButton;

    // Line drawer
    public int continuesFramesPressed;

    public Texture2D brush;
    public float sizeDecimal;

    // Took the variable inits outside of the Start() and fixed the processing system so it actually works right
    // Dont change

    private Dictionary<Texture2D, Texture2D> textureDict = new Dictionary<Texture2D, Texture2D>();
    private Dictionary<Texture2D, Texture2D> reversedTextureDict = new Dictionary<Texture2D, Texture2D>();
    private List<Texture2D> dictKeys;
    private string[] brushes = { "Paint", "Erase" };
    private float[] brushTemplate;
    public int brushTemplateX;
    public int brushTemplateY;

    void Start()
    {
        fcp.onColorChange.AddListener(OnChangeColor);

        // Process Model
        Transform trainsform = Model.transform;
        //get zombie model
        foreach (Transform childTransform in trainsform) {
            if (childTransform.gameObject.GetComponent<MeshFilter>() != null && childTransform != trainsform) {
                //Go through each child gameObject and see if have SkinnedMeshRenderer
                //if it does, add a meshcollider using the mesh from the SkinnedMeshRenderer

                MeshCollider collider = childTransform.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
                MeshFilter meshFilter = childTransform.gameObject.GetComponent<MeshFilter>();
                collider.sharedMesh = meshFilter.mesh;
            }

            //For further development, change texture to be writable (by cloning texture to seperate one, for reverting and saving also)
            // Note: I think we did the above already

            // Will check if texture was already clone so we don't clone textures more than once
            if (childTransform.gameObject.GetComponent<Renderer>() != null) {
                Material material = childTransform.gameObject.GetComponent<Renderer>().material;
                Texture2D mainTexture = (Texture2D)material.mainTexture;
                dictKeys = new List<Texture2D>(textureDict.Keys);
                if (dictKeys.Contains(mainTexture)) {
                    material.SetTexture("_MainTex", textureDict[mainTexture]);
                    Debug.Log("repeats");
                } else {
                    Texture2D clone = new Texture2D(mainTexture.width, mainTexture.height);
                    clone.name = mainTexture.name + "clone";
                    clone.SetPixels(mainTexture.GetPixels());
                    clone.Apply();
                    textureDict.Add(mainTexture, clone);
                    material.SetTexture("_MainTex", clone);
                }

            }
        }
        dictKeys = new List<Texture2D>(textureDict.Keys);
        // Create reversed dictionary for erase lookup
        foreach (var key in dictKeys) {
            reversedTextureDict.Add(textureDict[key], key);
        }

        PaintSizeManager(50);
    }



    public void Update()
    {
        
        if (CanPaint == true)
        {

            if (Input.touchCount > 0)
            {
                continuesFramesPressed += 1;
                Touch touch = Input.GetTouch(0);
                TouchRay = cam.ScreenPointToRay(touch.position);

                if (!Physics.Raycast(TouchRay, out hit))
                {
                    Debug.Log("Not hit");
                    return;
                }
                ApplyPaintToHitpoint(hit);
            }
            else
            {
                continuesFramesPressed = 0;
            }
        }
        fcp.color = CurrentColor;
        Transparentcy = CurrentColor.a;

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
        Debug.Log(pixelUV);

        // Generate Brush
        colors = new Color32[brushTemplate.Length];
        int offsetx = (brushTemplateX/2);
        int offsety = (brushTemplateY/2);
        for (int i = 0; i < brushTemplateX; i++)
        {
            for (int j = 0; j < brushTemplateY; j++)
            {
                int applyPoint = j * brushTemplateY + i;
                if (mode == 1) //Eraser
                {
                    colors[applyPoint] = reversedTextureDict[tex].GetPixel((int)pixelUV.x + i - offsetx, (int)pixelUV.y + j - offsety);
                }
                else
                {
                    colors[applyPoint] = Color.Lerp(CurrentColor, tex.GetPixel((int)pixelUV.x + i  - offsetx, (int)pixelUV.y + j - offsety), 1 - (brushTemplate[applyPoint] * Transparentcy));
                }
            }
        }

        // Apply textures
        tex.SetPixels32((int)pixelUV.x - offsetx, (int)pixelUV.y - offsety, brushTemplateX, brushTemplateY, colors);
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
            ModeSelector.GetComponent<Image>().sprite = Painter;
        }
        else
        {
            mode += 1;
            ModeSelector.GetComponent<Image>().sprite = Eraser;
        }
    }

    public void CanPaintSwitch()
    {
        CanPaint = !CanPaint;
        if (CanPaint)
        {
            Debug.Log("Enable Paint");
            MoverButton.GetComponent<Button>().enabled = false;
        }
        if (!CanPaint)
        {
            Debug.Log("Disable Paint");
            MoverButton.GetComponent<Button>().enabled = true;
        }
    }

    public void PaintSizeManager(int PaintSizeFromSlider)
    {
        BrushSize = PaintSizeFromSlider;
        sizeDecimal = (float)BrushSize * (float)2 / (float)100;
        brushTemplateX = (int)(brush.width * sizeDecimal);
        brushTemplateY = (int)(brush.height * sizeDecimal);
        brushTemplate = new float[brushTemplateX*brushTemplateY];

        for (int i = 0; i < brushTemplateX; i++)
        {
            for (int j = 0; j < brushTemplateY; j++)
            {
                int m = (int)(i / sizeDecimal + 0.5);              // +0.5 for rounding
                if (m > brushTemplateX - 1) {          // limit the value
                    m = brushTemplateX - 1;
                }
                int n = (int)(j / sizeDecimal + 0.5);
                if (n > brushTemplateY - 1) {
                    n = brushTemplateY - 1;
                }
                brushTemplate[j * brushTemplateY + i] = brush.GetPixel(m, n).a;
            }
        }
    }
}