using UnityEngine;

public class BingoPhotoHandler : MonoBehaviour
{
    public TrashDetector detector;
    public UnityEngine.UI.RawImage photoDisplay;


    public void OnPhotoTaken(Texture2D photo)
    {
        photoDisplay.texture = photo;

        var detections = detector.Detect(photo);

        if (detections.Count == 0)
        {
            Debug.Log("Nothing detected");
            return;
        }

        detections.Sort((a, b) => b.confidence.CompareTo(a.confidence));

        Debug.Log($"Found {detections.Count} items:");
        foreach (var d in detections)
        {
            Debug.Log($"- {d.className} ({d.confidence:P0})");

            // Cross off every detected class on the bingo card
            // BingoCard.CrossOff(d.className);
        }
    }
}