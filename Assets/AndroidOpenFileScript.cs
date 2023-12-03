using UnityEngine;
using UnityEngine.UI;
using SFB;
using System.IO;

public class AndroidOpenFileScript : MonoBehaviour
{
    public GameObject modelContainer; // Reference to an empty GameObject where the loaded model will be placed.
    public Text statusText; // Reference to a Text component to display loading status.

    private void Start()
    {
        if (statusText != null)
        {
            statusText.text = "Ready to load a model.";

        }
    }

    public void LoadModel()
    {
        // Define the extensions and filters for the file browser.
        var extensions = new[] {
            new ExtensionFilter("3D Models", "obj", "stl")
        };

        // Open the file browser and wait for the user to select a file.
        var paths = StandaloneFileBrowser.OpenFilePanel("Open Model File", "", extensions, false);

        if (paths.Length > 0)
        {
            string filePath = paths[0];
            string extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".obj" || extension == ".stl")
            {
                // Load and display the selected model.
                GameObject model = ImportModel(filePath);
                model.transform.SetParent(modelContainer.transform);

                statusText.text = "Model loaded successfully.";
            }
            else
            {
                statusText.text = "Unsupported file format.";
            }
        }
    }

    private GameObject ImportModel(string filePath)
    {
        // You can use a suitable 3D model import library or script here.
        // For example, you can use the Unity AssetBundle system, or other third-party libraries to load OBJ and STL models.

        // Replace the following line with the code to import and display the 3D model.
        GameObject model = new GameObject("LoadedModel");
        return model;
    }
}
