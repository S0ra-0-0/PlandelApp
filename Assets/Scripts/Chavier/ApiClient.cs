using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// ----- Request/response klassen -----
[Serializable] public class RegisterRequest { public string username, mail, password; public int age; }
[Serializable] public class LoginRequest { public string username, password; }
[Serializable] public class LoginResponse { public int id; public string username; public int age; }
[Serializable] public class LobbyResponse { public string code; public long seed; }
[Serializable] public class ObjectRequest { public string name, imageUrl, description; public int userId; }
[Serializable] public class ObjectItem { public int id; public string name, imageUrl, description; public int userId; }
[Serializable] public class ObjectList { public ObjectItem[] items; }

public class ApiClient : MonoBehaviour
{
    public static ApiClient Instance;
    [SerializeField] private string baseUrl = "http://localhost:5000";

    // Wordt na login ingevuld, zodat andere scripts weten wie er is ingelogd
    public LoginResponse CurrentUser;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    // ----- Users -----
    public void Register(string user, string mail, string pass, int age, Action<bool, string> done)
        => StartCoroutine(Send("POST", "/register",
            JsonUtility.ToJson(new RegisterRequest { username = user, mail = mail, password = pass, age = age }), done));

    public void Login(string user, string pass, Action<bool, string> done)
        => StartCoroutine(Send("POST", "/login",
            JsonUtility.ToJson(new LoginRequest { username = user, password = pass }),
            (ok, res) =>
            {
                if (ok) CurrentUser = JsonUtility.FromJson<LoginResponse>(res);
                done?.Invoke(ok, res);
            }));

    // ----- Lobby -----
    public void CreateLobby(Action<bool, string> done)
        => StartCoroutine(Send("POST", "/lobby", "{}", done));

    public void JoinLobby(string code, Action<bool, string> done)
        => StartCoroutine(Send("GET", "/lobby/" + code, null, done));

    // ----- Objecten -----
    public void SaveObject(string name, string imageUrl, string description, Action<bool, string> done)
        => StartCoroutine(Send("POST", "/objects",
            JsonUtility.ToJson(new ObjectRequest
            {
                name = name, imageUrl = imageUrl, description = description,
                userId = CurrentUser.id
            }), done));

    public void GetMyObjects(Action<bool, string> done)
        => StartCoroutine(Send("GET", "/objects/user/" + CurrentUser.id, null, done));

    // ----- Algemene request-functie -----
    IEnumerator Send(string method, string path, string json, Action<bool, string> done)
    {
        using var req = new UnityWebRequest(baseUrl + path, method);
        if (json != null)
        {
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.SetRequestHeader("Content-Type", "application/json");
        }
        req.downloadHandler = new DownloadHandlerBuffer();
        yield return req.SendWebRequest();

        bool ok = req.result == UnityWebRequest.Result.Success;
        done?.Invoke(ok, ok ? req.downloadHandler.text : req.downloadHandler.text + " | " + req.error);
    }
}