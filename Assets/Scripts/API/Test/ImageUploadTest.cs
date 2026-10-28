
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ImageUploader : MonoBehaviour
{
    public static ImageUploader Instance;

    private void Awake() {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    [SerializeField] private Texture2D TestImage;

    private string url =
        "https://api.imgbb.com/1/upload?key=7e7d546e3b03319c0ca02efebfc08b0e";

    public void TestUpload(Texture2D photo)
    {
        StartCoroutine(UploadPNG(photo));
    }

    IEnumerator UploadPNG(Texture2D imageToUpload)
    {
        if (TestImage == null)
        {
            Debug.LogError("TestImage is not assigned!");
            yield break;
        }

        // Encode the texture as a PNG file
        byte[] bytes = imageToUpload.EncodeToPNG();

        // Create the upload form
        WWWForm form = new WWWForm();
        form.AddBinaryData(
            "image",
            bytes,
            "image.png",
            "image/png"
        );

        using (UnityWebRequest w = UnityWebRequest.Post(url, form))
        {
            yield return w.SendWebRequest();

            if (w.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    $"Upload failed: HTTP {w.responseCode}\n" +
                    $"{w.error}\n{w.downloadHandler.text}"
                );
            }
            else
            {
                Debug.Log("Upload successful!");
                Debug.Log(w.downloadHandler.text);
            }
        }
    }
}
