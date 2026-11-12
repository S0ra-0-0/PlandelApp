using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable] class ImgbbData { public string url; public string display_url; }
[Serializable] class ImgbbResponse { public ImgbbData data; public bool success; }

public class ImageUploader : MonoBehaviour
{
    public static ImageUploader Instance;
    [SerializeField] private string apiKey; // vul in via de Inspector, NIET hier in de code

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    // done(true, url) bij succes, done(false, foutmelding) bij fout
    public void Upload(Texture2D photo, Action<bool, string> done)
        => StartCoroutine(UploadPNG(photo, done));

    IEnumerator UploadPNG(Texture2D photo, Action<bool, string> done)
    {
        if (photo == null) { done?.Invoke(false, "Geen afbeelding"); yield break; }

        byte[] bytes = photo.EncodeToPNG(); // texture moet Read/Write aan hebben
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", bytes, "image.png", "image/png");

        using var w = UnityWebRequest.Post("https://api.imgbb.com/1/upload?key=" + apiKey, form);
        yield return w.SendWebRequest();

        if (w.result != UnityWebRequest.Result.Success)
        {
            done?.Invoke(false, w.error + " " + w.downloadHandler.text);
            yield break;
        }

        var res = JsonUtility.FromJson<ImgbbResponse>(w.downloadHandler.text);
        done?.Invoke(true, res.data.url);
    }
}