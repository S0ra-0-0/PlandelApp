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

        Texture2D photo = new Texture2D(webCam.width, webCam.height);
        photo.SetPixels(webCam.GetPixels());
        photo.Apply();

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