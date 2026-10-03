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
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// Sends Get and Post requests to postGres API
public class PlayerAPI : MonoBehaviour
{
    public LogicBoard playerData;
    public LogicBoard playerPost;
    private string apiUrl = "https://flappyappi-gqfdhwhvbwhkbkgh.centralus-01.azurewebsites.net/api/game/player/";
    private string postUrl = "https://flappyappi-gqfdhwhvbwhkbkgh.centralus-01.azurewebsites.net/api/game/PostPlayer";
    private string postEnd = "https://flappyappi-gqfdhwhvbwhkbkgh.centralus-01.azurewebsites.net/api/game/PostEnd";
    private string userUrl = "https://flappyappi-gqfdhwhvbwhkbkgh.centralus-01.azurewebsites.net/api/game/username/";
    private string GetserverUrl = "https://flappyappi-gqfdhwhvbwhkbkgh.centralus-01.azurewebsites.net/api/game/scores";

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
    public GameObject textMeshPrefabFirst;
    public GameObject textMeshPrefabSecond;
    public GameObject textMeshPrefabThird;
    public GameObject textMeshPrefabOther;

    public Transform uiContentContainer;

    public TextMeshProUGUI First_Place;
    public TextMeshProUGUI Second_Place;
    public TextMeshProUGUI Third_Place;


    // Sends data to Database API to check if player exists in the database from UNITY'S created ID
    // If Exists return true else false
    public async Task<PlayerData> CheckPlayer(string playerId)
    { 
        Debug.Log("Player Checked: " +  playerId);
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
                Debug.Log("Check Returned NULL");
                return null;
            }
            else
            {
                Debug.LogError(
                    "Status: " + request.responseCode +
                    "\nError: " + request.error +
                    "\nServer Response: " + request.downloadHandler.text
                );
                PlayerData player = new PlayerData();
                return player;
            }
        }
    }


    //Create user screen unlocked and checks if username exists in the database already
    // Makes sure no duplicate usernames
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
                Console.WriteLine(
                    "Status: " + request.responseCode +
                    "\nError: " + request.error +
                    "\nServer Response: " + request.downloadHandler.text
                );
                return false;
           }

        }
    }


    // When user creates new player if its not illegal or player name doesn't already exist
    // Then post new player to database. 
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

            Console.WriteLine(
                   "Status: " + request.responseCode +
                   "\nError: " + request.error +
                   "\nServer Response: " + request.downloadHandler.text
               );

            return false;
        }
    }

    // When user interacts with Leaderboard or exits the game post highest score
    // and the amount of medals they've scored
    public async Task<bool> PostEnd(EndData endata)
    {

        string json = JsonUtility.ToJson(endata);
        Debug.Log("JSON being sent: " + json);

        byte[] body = Encoding.UTF8.GetBytes(json);


        using (UnityWebRequest request = new UnityWebRequest(postEnd, "POST"))
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

    // Fetches user data information from database
    public IEnumerator FetchScoresFromServer()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GetserverUrl))
        {
            // allows Unity to trust Visual Studio's local SSL certificate
            webRequest.certificateHandler = new BypassCertificateHandler();

            Console.WriteLine("Pinging the game server...");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = webRequest.downloadHandler.text;
                Console.WriteLine("Successfully fetched data from server: " + jsonResult);

                // parse jsonResult to update UI
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

                for (int i = 0; i < leaderboard.Count; i++)
                {
                    LeaderboardEntry entry = leaderboard[i];
                    GameObject TextRow;

                    if (i == 0)
                    {
                        TextRow = Instantiate(textMeshPrefabFirst, uiContentContainer);
                    }
                    else if (i == 1)
                    {
                        TextRow = Instantiate(textMeshPrefabSecond, uiContentContainer);
                    }
                    else if (i == 2)
                    {
                       TextRow = Instantiate(textMeshPrefabThird, uiContentContainer);
                    }
                    else
                    {
                        TextRow = Instantiate(textMeshPrefabOther, uiContentContainer);
                    }

                    TextMeshProUGUI NameText = TextRow.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI ScoreText = TextRow.transform.GetChild(2).GetComponent<TextMeshProUGUI>();


                    NameText.text = entry.playerName;
                    ScoreText.text = entry.stats.score.ToString();

                    if (i > 2)
                    {
                        TextMeshProUGUI RankText = TextRow.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                        RankText.text = (i+1).ToString();
                    }

                    // Optional: Combine gold, silver, and bronze into the third medal column
                    //medalText.text = $"{entry.stats.gold}   {entry.stats.silver}   {entry.stats.bronze}";
                }
            }
            else
            {
                Console.WriteLine("Server Connection Failed: " + webRequest.error);
            }
        }
    }

    // Helper class so Unity doesn't block local development certificates
    public class BypassCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }
}
