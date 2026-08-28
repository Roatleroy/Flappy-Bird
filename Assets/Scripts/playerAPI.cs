using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Audio.ProcessorInstance;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Text.Json.Serialization;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.PackageManager.Requests;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerAPI : MonoBehaviour
{
    public LogicBoard playerData;
    public LogicBoard playerPost;
    private string apiUrl = "https://localhost:7130/api/game/player/";
    private string postUrl = "https://localhost:7130/api/game/PostPlayer";
    private string userUrl = "https://localhost:7130/api/game/username/";
    private string GetserverUrl = "https://localhost:7130/api/game/scores";

    //-------------------- Json Variable Key:Value(Struct) Object
    [System.Serializable]
    public class PlayerStats
    {
        public int score;
        public int gold;
        public int silver;
        public int bronze;
    }

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public PlayerStats stats;
    }

    public List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();

    //----- Game Object for LeaderBoard Username Spawning
    public GameObject textMeshPrefab;
    public Transform uiContentContainer;

    public TextMeshProUGUI First_Place;
    public TextMeshProUGUI Second_Place;
    public TextMeshProUGUI Third_Place;


    public async Task<PlayerData> CheckPlayer(string playerId)
    { 
        using (UnityWebRequest request = new UnityWebRequest(apiUrl + playerId, "GET"))
        {

            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                PlayerData player =
                    JsonUtility.FromJson<PlayerData>(
                        request.downloadHandler.text
                    );

                return player;
            }
            else if (request.responseCode == 404)
            {
                return null;
            }
            else
            {
                Debug.LogError(
                    "Status: " + request.responseCode +
                    "\nError: " + request.error +
                    "\nServer Response: " + request.downloadHandler.text
                );
                return null;
            }
        }
    }


    public async Task<bool> CheckUsername(string username)
    {
        using (UnityWebRequest request = new UnityWebRequest(userUrl + username, "GET"))
        {
            //Debug.Log("Username URL: " + userUrl + username);
            request.downloadHandler = new DownloadHandlerBuffer();
            await request.SendWebRequest();

           if (request.result == UnityWebRequest.Result.Success)
           {
                Debug.Log("Username API returned: " + request.downloadHandler.text);
                bool exists = bool.Parse(request.downloadHandler.text);
                return exists; 
           }
           else
           {
                Debug.LogError(
                    "Status: " + request.responseCode +
                    "\nError: " + request.error +
                    "\nServer Response: " + request.downloadHandler.text
                );
                return false;
           }

        }
    }


    public async Task<bool> PostPlayer(PlayerCreate playerId)
    {

        string json = JsonUtility.ToJson(playerId);
        Debug.Log("JSON being sent: " + json);

        byte[] body = Encoding.UTF8.GetBytes(json);

       
        using (UnityWebRequest request = new UnityWebRequest(postUrl, "POST"))
        {

            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                return true;
            }

            Debug.LogError(
                   "Status: " + request.responseCode +
                   "\nError: " + request.error +
                   "\nServer Response: " + request.downloadHandler.text
               );

            return false;
        }
    }

    // LeaderBoard API Code

    public IEnumerator FetchScoresFromServer()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GetserverUrl))
        {
            // Crucial for testing: allows Unity to trust Visual Studio's local SSL certificate
            webRequest.certificateHandler = new BypassCertificateHandler();

            Debug.Log("Pinging the game server...");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = webRequest.downloadHandler.text;
                Debug.Log("Successfully fetched data from server: " + jsonResult);

                // Next, you will parse this jsonResult to update your UI!
                var dict = JsonConvert.DeserializeObject<Dictionary<string, PlayerStats>>(jsonResult);

                leaderboard.Clear();
                foreach (Transform child in uiContentContainer)
                {
                    Destroy(child.gameObject);
                }

                if (dict != null)
                {
                    foreach (var kvp in dict)
                    {
                        leaderboard.Add(new LeaderboardEntry { playerName = kvp.Key, stats = kvp.Value });
                    }
                }

                switch (leaderboard.Count)
                {
                    case 0:
                        break;
                    case 1:
                        First_Place.text = leaderboard[0].playerName;
                        break;
                    case 2:
                        First_Place.text = leaderboard[0].playerName;
                        Second_Place.text = leaderboard[1].playerName;
                        break;
                    case 3:
                        First_Place.text = leaderboard[0].playerName;
                        Second_Place.text = leaderboard[1].playerName;
                        Third_Place.text = leaderboard[2].playerName;
                        break;
                    default:
                        First_Place.text = leaderboard[0].playerName;
                        Second_Place.text = leaderboard[1].playerName;
                        Third_Place.text = leaderboard[2].playerName;
                        for (int i = 3; i < leaderboard.Count; i++)
                        {
                            LeaderboardEntry entry = leaderboard[i];

                            GameObject TextRow = Instantiate(textMeshPrefab, uiContentContainer);

                            TextMeshProUGUI nameText = TextRow.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                            TextMeshProUGUI scoreText = TextRow.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                            TextMeshProUGUI medalText = TextRow.transform.GetChild(2).GetComponent<TextMeshProUGUI>();


                            nameText.text = entry.playerName;
                            scoreText.text = entry.stats.score.ToString();

                            // Optional: Combine gold, silver, and bronze into the third medal column
                            medalText.text = $"{entry.stats.gold}   {entry.stats.silver}   {entry.stats.bronze}";
                        }
                        break;

                }
            }
            else
            {
                Debug.LogError("Server Connection Failed: " + webRequest.error);
            }
        }
    }

    // Helper class so Unity doesn't block local development certificates
    public class BypassCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }
}
