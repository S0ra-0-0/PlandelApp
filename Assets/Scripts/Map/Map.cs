using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

[RequireComponent(typeof(RectTransform))]
public class Map : MonoBehaviour
{
    public string apiKey;

    [Header("References (assign in Inspector)")]
    public RawImage mapImage;

    [Header("Map")]
    public int zoom = 17;
    public enum resolution { low = 1, high = 2 }
    public resolution mapResolution = resolution.high;
    public enum type { roadmap, satellite, hybrid, terrain }
    public type mapType = type.roadmap;

    [Header("Minimap placement")]
    public bool applyCorner = true;
    public enum corner { TopLeft, TopRight, BottomLeft, BottomRight }
    public corner screenCorner = corner.BottomRight;
    public float mapSize = 256f;
    public Vector2 margin = new Vector2(20f, 20f);

    [Header("Live tracking")]
    public float smoothing = 8f;
    public float edgePadding = 40f;
    public float minFetchInterval = 3f;

    [Header("Editor / fallback (arrow keys or WASD move you in the Editor)")]
    public double fallbackLat = 52.0907;
    public double fallbackLon = 5.1214;
    public double editorSpeed = 0.0005;

    const int ImageSize = 640;
    RectTransform root, mapRT;

    bool locationResolved, gpsReady, loading, hasMap;
    float lastFetchTime = -999f;

    double targetLat, targetLon;
    double smoothLat, smoothLon;
    bool smoothInit;

    double centerX, centerY;
    int fetchedZoom;
    type fetchedType;
    resolution fetchedRes;

    void Start()
    {
        root = GetComponent<RectTransform>();

        if (mapImage == null)
        {
            Debug.LogError("Map: assign the MapImage RawImage in the Inspector.");
            enabled = false;
            return;
        }

        mapRT = mapImage.rectTransform;
        mapRT.sizeDelta = new Vector2(ImageSize, ImageSize);
        mapImage.enabled = false;

        targetLat = fallbackLat;
        targetLon = fallbackLon;
        StartCoroutine(StartLocation());
    }

    void OnDestroy()
    {
        if (Input.location.status == LocationServiceStatus.Running)
            Input.location.Stop();
    }

    void Update()
    {
        if (!locationResolved) return;

        UpdateTarget();

        if (!smoothInit)
        {
            smoothLat = targetLat;
            smoothLon = targetLon;
            smoothInit = true;
        }

        double t = 1.0 - System.Math.Exp(-smoothing * Time.deltaTime);
        smoothLat += (targetLat - smoothLat) * t;
        smoothLon += (targetLon - smoothLon) * t;

        if (!hasMap)
        {
            if (!loading && Time.time - lastFetchTime >= minFetchInterval)
                StartCoroutine(GetGoogleMap(smoothLat, smoothLon));
            return;
        }

        Project(smoothLat, smoothLon, fetchedZoom, out double px, out double py);
        float dx = (float)(px - centerX);
        float dy = (float)(py - centerY);

        mapRT.anchoredPosition = new Vector2(-dx, dy);

        float limit = ImageSize / 2f - mapSize / 2f - edgePadding;
        bool nearEdge = Mathf.Abs(dx) > limit || Mathf.Abs(dy) > limit;
        bool settingsChanged = zoom != fetchedZoom || mapType != fetchedType || mapResolution != fetchedRes;

        if ((nearEdge || settingsChanged) && !loading && Time.time - lastFetchTime >= minFetchInterval)
            StartCoroutine(GetGoogleMap(smoothLat, smoothLon));
    }

    void UpdateTarget()
    {
        if (gpsReady && Input.location.status == LocationServiceStatus.Running)
        {
            targetLat = Input.location.lastData.latitude;
            targetLon = Input.location.lastData.longitude;
        }
#if UNITY_EDITOR
        else
        {
            targetLon += Input.GetAxis("Horizontal") * editorSpeed * Time.deltaTime;
            targetLat += Input.GetAxis("Vertical") * editorSpeed * Time.deltaTime;
        }
#endif
    }

    IEnumerator StartLocation()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            float waited = 0f;
            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation) && waited < 15f)
            {
                waited += Time.unscaledDeltaTime;
                yield return null;
            }
        }
#endif
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location disabled, using fallback position.");
            locationResolved = true;
            yield break;
        }

        Input.location.Start(1f, 1f);

        int wait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && wait > 0)
        {
            yield return new WaitForSeconds(1f);
            wait--;
        }

        gpsReady = Input.location.status == LocationServiceStatus.Running;
        if (!gpsReady) Debug.Log("Could not start GPS, using fallback position.");
        locationResolved = true;
    }


    IEnumerator GetGoogleMap(double lat, double lon)
    {
        loading = true;
        lastFetchTime = Time.time;

        int reqZoom = zoom;
        type reqType = mapType;
        resolution reqRes = mapResolution;

        string center = lat.ToString("F6", CultureInfo.InvariantCulture) + ","
                      + lon.ToString("F6", CultureInfo.InvariantCulture);

        string url = "https://maps.googleapis.com/maps/api/staticmap"
            + "?center=" + center
            + "&zoom=" + reqZoom
            + "&size=" + ImageSize + "x" + ImageSize
            + "&scale=" + (int)reqRes
            + "&maptype=" + reqType
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
                Texture old = mapImage.texture;
                mapImage.texture = DownloadHandlerTexture.GetContent(www);
                if (old != null) Destroy(old);
                mapImage.enabled = true;

                fetchedZoom = reqZoom;
                fetchedType = reqType;
                fetchedRes = reqRes;
                Project(lat, lon, reqZoom, out centerX, out centerY);
                hasMap = true;
            }
        }

        loading = false;
    }

    static void Project(double lat, double lon, int z, out double x, out double y)
    {
        double scale = 256.0 * System.Math.Pow(2, z);
        x = (lon + 180.0) / 360.0 * scale;
        double s = System.Math.Sin(lat * System.Math.PI / 180.0);
        s = System.Math.Max(-0.9999, System.Math.Min(0.9999, s));
        y = (0.5 - System.Math.Log((1 + s) / (1 - s)) / (4 * System.Math.PI)) * scale;
    }
}