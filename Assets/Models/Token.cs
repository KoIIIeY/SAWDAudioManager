using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SAWDAudio.Models
{
    [Serializable]
    public class Token {
        public string api_token;

        public static string Login(string email, string password)
        {
            using (var request = UnityWebRequest.Get(SAWDAudioManager.baseURL + "/call/token/postLogin?email="+email+"&password="+password))
            {
                request.SendWebRequest();

                while (!request.isDone) { }; // hack for syncronous request!

                switch (request.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError($"SAWD:Could not authorize! Error: {request.error}");
                        return "";
                }

                Debug.Log("RESP:"+request.downloadHandler.text);
                return JsonUtility.FromJson<Token>(request.downloadHandler.text).api_token;
            }
        }
    }
}