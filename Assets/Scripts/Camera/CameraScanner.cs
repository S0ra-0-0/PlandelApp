using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraScanner : MonoBehaviour
{
    const string FolderName = "PlandelAppPictures";

    WebCamTexture webCam;
    public RawImage imageCamera;
    string status = "Nog geen foto gemaakt";

    void Start()
    {
        webCam = new WebCamTexture();
        imageCamera.texture = webCam;
        webCam.Play();
    }

    void Update()
    {
        if (webCam == null || !webCam.isPlaying) return;

        // Preview rechtzetten
        imageCamera.rectTransform.localEulerAngles = new Vector3(0, 0, -webCam.videoRotationAngle);
        imageCamera.rectTransform.localScale = new Vector3(1, webCam.videoVerticallyMirrored ? -1 : 1, 1);
    }

    void OnDestroy()
    {
        if (webCam != null && webCam.isPlaying)
            webCam.Stop();
    }

    // Koppel deze aan je button OnClick
    public void OnTakePhotoButton()
    {
        status = "Knop ingedrukt...";
        StartCoroutine(TakePhoto());
    }

    IEnumerator TakePhoto()
    {
        yield return new WaitForEndOfFrame();

        int w = webCam.width;
        int h = webCam.height;
        Color32[] pixels = webCam.GetPixels32();

        // Verticale spiegeling corrigeren
        if (webCam.videoVerticallyMirrored)
        {
            Color32[] flipped = new Color32[pixels.Length];
            for (int y = 0; y < h; y++)
                Array.Copy(pixels, y * w, flipped, (h - 1 - y) * w, w);
            pixels = flipped;
        }

        // Foto draaien met de kloksrichting mee
        int turns = ((webCam.videoRotationAngle / 90) % 4 + 4) % 4;
        for (int t = 0; t < turns; t++)
        {
            Color32[] rotated = new Color32[pixels.Length];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    rotated[(w - 1 - x) * h + y] = pixels[y * w + x];
            pixels = rotated;
            int tmp = w; w = h; h = tmp;
        }

        Texture2D photo = new Texture2D(w, h);
        photo.SetPixels32(pixels);
        photo.Apply();

        Debug.Log("foto gemaakt?");

        ImageUploader.Instance.TestUpload(photo);

        byte[] bytes = photo.EncodeToPNG();
        Destroy(photo);

        string fileName = $"photo_{DateTime.Now:yyyyMMdd_HHmmss}.png";

        NativeGallery.SaveImageToGallery(bytes, FolderName, fileName, (success, path) =>
        {
            status = "Success: " + success + "\nPad: " + path;
            Debug.Log(status);
        });
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 40;
        GUI.Label(new Rect(20, 20, Screen.width - 40, 400), status);
    }
}