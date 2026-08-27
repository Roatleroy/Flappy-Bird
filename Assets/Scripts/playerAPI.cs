using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Audio.ProcessorInstance;

public class PlayerAPI : MonoBehaviour
{
    public LogicBoard playerData;
    public LogicBoard playerPost;
    private string apiUrl = "https://localhost:7130/api/game/player/";
    private string postUrl = "https://localhost:7130/api/game/player";
    private string userUrl = "https://localhost:7130/api/game/username/";

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


    public async Task<Update> PostPlayer(string playerId)
    {
        using (UnityWebRequest request = new UnityWebRequest(postUrl, "POST"))
        {

            //request.uploadHandler = new UploadHandler(json);
            request.downloadHandler = new DownloadHandlerBuffer();

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Update player =
                    JsonUtility.FromJson<Update>(
                        request.downloadHandler.text
                    );

                return player;
            }

            Debug.LogError(
                   "Status: " + request.responseCode +
                   "\nError: " + request.error +
                   "\nServer Response: " + request.downloadHandler.text
               );

            return null;
        }
    }
}
