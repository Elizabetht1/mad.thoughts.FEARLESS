// ---------------------------------------
// Creation Date:
// Author: 
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* NETOWRKING PACKAGES */
using UnityEngine.Networking;


public class APIManager : MonoBehaviour
{
    // events
    public class NPCResponseArgs { public AudioClip audioClip; }
    public event EventHandler<NPCResponseArgs> OnNPCResponse;


    //HTTP client 
    private string ApiAdress = "http://127.0.0.1:8000/gen-convo";

    public class GenConvoReq{
        public string agentRole;
        public string agentTone;
        public string userRole;
        public string userQuery;
    }

    [SerializeField] private Player player;

    private void Start() {
        Debug.Log("I am alive!");
        StartCoroutine(postGenQuestionReq());
        player.OnPlayerSpoke += OnPlayerSpoke;
    }

    private void OnPlayerSpoke(object sender, Player.PlayerSpokeArgs e) {
        Debug.Log("player said something");
        
        // convert audio clip to wav file
        // make a post request, sending wav file
    }

    private void getRequest() {
        AudioClip clip = null;
        // make a get request and set clip to response
        // fire event to make NPC respond
        OnNPCResponse?.Invoke(this, new NPCResponseArgs { audioClip = clip });
    }

    /**
    Make a request to generate a question to the backend
    */
    private IEnumerator postGenQuestionReq() {
        
        GenConvoReq convoReq = new GenConvoReq{
                agentRole = "interviewer",
                agentTone = "neutral",
                userRole = "interviewee",
                userQuery = "give me a job pretty please",
        };
        string json = JsonUtility.ToJson(convoReq);
        var req = new UnityWebRequest(ApiAdress, "POST");


        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        
        
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.isNetworkError)
        {
                Debug.Log("Error While Sending: " + req.error);
        }else{
            Debug.Log("Received: " + req.downloadHandler.text);
        }

        // // Create a Web Form
        // WWWForm form = new WWWForm();
        // form.AddField("agentRole", "interviewer");
        // form.AddField("agentTone", "neutral");
        // form.AddField("userRole", "interviewee");
        // form.AddField("userQuery","give me a job pretty please");




        // using (UnityWebRequest speechApiReq = UnityWebRequest.Post(ApiAdress,form)) {
        //     yield return speechApiReq.SendWebRequest();
        //     Debug.Log("receiving a response \n");

        //    if (speechApiReq.result != UnityWebRequest.Result.Success)
        //     {
        //         Debug.LogError(JsonUtility.ToJson(speechApiReq.result));
        //     }
        //     else
        //     {
        //         Debug.Log("Form upload complete!");
        //     }

        // }

    }



}

