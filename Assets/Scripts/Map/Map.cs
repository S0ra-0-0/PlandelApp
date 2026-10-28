using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

[RequireComponent(typeof(RawImage))]
public class Map : MonoBehaviour
{
    public string apiKey;

    [Header("Map")]
    public int zoom = 16;
    public enum resolution { low = 1, high = 2 }
    public resolution mapResolution = resolution.high;
    public enum type { roadmap, satellite, hybrid, terrain }
    public type mapType = type.roadmap;

    public enum corner { TopLeft, TopRight, BottomLeft, BottomRight }
    public corner screenCorner = corner.BottomRight;
    public float mapSize = 256f;     // size on screen (also requested size from Google, max 640)
    public Vector2 margin = new Vector2(20f, 20f);

    [Header("Updating")]
    public float refreshInterval = 10f;      // seconds between map refreshes
    public float minMoveMeters = 10f;        // only refresh if you moved this far

    [Header("Editor / fallback position (used when GPS is unavailable)")]
    public float fallbackLat = 52.0907f;
    public float fallbackLon = 5.1214f;

    private RawImage rawImage;
    private bool gpsReady = false;
    private bool loading = false;

    private float lastLat, lastLon;
    private int lastZoom;
    private type lastType;
    private float lastRequestTime = -999f;
    private bool hasMap = false;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        ApplyCorner();
        StartCoroutine(StartLocation());
    }

    void Update()
    {
        if (loading) return;

        GetPosition(out float lat, out float lon);

        bool settingsChanged = !hasMap || zoom != lastZoom || mapType != lastType;
        bool timeElapsed = Time.time - lastRequestTime >= refreshInterval;
        bool moved = DistanceMeters(lat, lon, lastLat, lastLon) >= minMoveMeters;

        if (settingsChanged || (timeElapsed && moved))
        {
            StartCoroutine(GetGoogleMap(lat, lon));
        }
    }

    void ApplyCorner()
    {
        RectTransform rt = rawImage.rectTransform;
        Vector2 anchor;
        Vector2 pos = margin;

        switch (screenCorner)
        {
            case corner.TopLeft: anchor = new Vector2(0, 1); pos = new Vector2(margin.x, -margin.y); break;
            case corner.TopRight: anchor = new Vector2(1, 1); pos = new Vector2(-margin.x, -margin.y); break;
            case corner.BottomLeft: anchor = new Vector2(0, 0); break;
            default: anchor = new Vector2(1, 0); pos = new Vector2(-margin.x, margin.y); break;
        }

        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.sizeDelta = new Vector2(mapSize, mapSize);
        rt.anchoredPosition = pos;
    }

    IEnumerator StartLocation()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(1f);
        }
#endif
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location is disabled, using fallback position.");
            yield break;
        }

        Input.location.Start(5f, 5f); // desired accuracy, update distance (meters)

        int wait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && wait > 0)
        {
            yield return new WaitForSeconds(1f);
            wait--;
        }

        if (Input.location.status == LocationServiceStatus.Running)
            gpsReady = true;
        else
            Debug.Log("Could not get GPS, using fallback position.");
    }

    void GetPosition(out float lat, out float lon)
    {
        if (gpsReady && Input.location.status == LocationServiceStatus.Running)
        {
            lat = Input.location.lastData.latitude;
            lon = Input.location.lastData.longitude;
        }
        else
        {
            lat = fallbackLat;
            lon = fallbackLon;
        }
    }

    IEnumerator GetGoogleMap(float lat, float lon)
    {
        loading = true;

        lastLat = lat;
        lastLon = lon;
        lastZoom = zoom;
        lastType = mapType;
        lastRequestTime = Time.time;
        hasMap = true;

        int size = Mathf.Clamp(Mathf.RoundToInt(mapSize), 1, 640);
        string center = lat.ToString(CultureInfo.InvariantCulture) + "," + lon.ToString(CultureInfo.InvariantCulture);

        string url = "https://maps.googleapis.com/maps/api/staticmap"
            + "?center=" + center
            + "&zoom=" + zoom
            + "&size=" + size + "x" + size
            + "&scale=" + (int)mapResolution
            + "&maptype=" + mapType
            + "&markers=color:red%7Csize:mid%7C" + center   // "you are here" marker
            + "&key=" + apiKey;

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("WWW ERROR: " + www.error);
            }
            else
            {
                if (rawImage.texture != null)
                    Destroy(rawImage.texture);
                rawImage.texture = DownloadHandlerTexture.GetContent(www);
            }
        }

        loading = false;
    }

    static float DistanceMeters(float lat1, float lon1, float lat2, float lon2)
    {
        const float R = 6371000f;
        float dLat = (lat2 - lat1) * Mathf.Deg2Rad;
        float dLon = (lon2 - lon1) * Mathf.Deg2Rad;
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2)
                + Mathf.Cos(lat1 * Mathf.Deg2Rad) * Mathf.Cos(lat2 * Mathf.Deg2Rad)
                * Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        return R * 2f * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
    }
}