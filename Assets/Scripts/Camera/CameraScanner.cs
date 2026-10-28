using UnityEngine;
using UnityEngine.UI;

public class CameraScanner : MonoBehaviour
{
    WebCamTexture webcam;
    public RawImage imageCamera;
    void Start()
    {
        webcam = new WebCamTexture();
        imageCamera.texture = webcam;
        webcam.Play();
    }

    
    void Update()
    {
        
    }
}
