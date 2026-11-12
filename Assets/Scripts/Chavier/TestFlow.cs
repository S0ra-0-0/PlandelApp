using UnityEngine;

public class TestFlow : MonoBehaviour
{
    [SerializeField] private Texture2D testImage;

    void Start()
    {
        // 1. Registreren (alleen de eerste keer, daarna geeft het "already taken")
        ApiClient.Instance.Register("jan", "jan@mail.nl", "geheim", 17, (ok, res) =>
        {
            Debug.Log("Register: " + ok + " " + res);

            // 2. Inloggen
            ApiClient.Instance.Login("jan", "geheim", (ok2, res2) =>
            {
                Debug.Log("Login: " + ok2 + " " + res2);
                if (!ok2) return;
                // 3. Lobby maken
                ApiClient.Instance.CreateLobby((ok3, res3) =>
            {
                if (!ok3) { Debug.LogError("Lobby mislukt: " + res3); return; }
                var lobby = JsonUtility.FromJson<LobbyResponse>(res3);
                Debug.Log("Lobby code: " + lobby.code + " seed: " + lobby.seed);
            });


                // 4. Afbeelding uploaden naar imgbb en daarna opslaan in de database
                ImageUploader.Instance.Upload(testImage, (ok4, url) =>
                {
                    if (!ok4) { Debug.LogError(url); return; }
                    ApiClient.Instance.SaveObject("Mijn object", url, "Beschrijving", (ok5, res5) =>
                        Debug.Log("Object opgeslagen: " + ok5 + " " + res5));
                });
            });
        });
    }
}