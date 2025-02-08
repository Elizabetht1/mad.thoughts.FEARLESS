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
    public string ApiAdress = "http://127.0.0.1:8000";

    private class GenConvoReq{
        private string agentRole;
        private string agentTone;
        private string userRole;
        private string userQuery;
    }

    [SerializeField] private Player player;

    private void Start() {
        Debug.Log("I am alive!");
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
        // GenConvoReq convoReq = new GenConvoReq{
        //         agentRole = "interviewer",
        //         agentTone = "neutral",
        //         userRole = "interviewee",
        //         userQuery = "give me a job pretty please",
        // };

        // Create a Web Form
        Dictionary<string, string> genConvoReqBody = new();
        genConvoReqBody["agentRole"] = "interviewer";
        genConvoReqBody["agentTone"] = "neutral";
        genConvoReqBody["userRole"] = "interviewee";
        genConvoReqBody["userQuery"] = "give me a job pretty please";
        

        using (UnityWebRequest speechApiReq = UnityWebRequest.Post(ApiAdress,genConvoReqBody)) {
            yield return speechApiReq.SendWebRequest();
            
           if (speechApiReq.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(speechApiReq.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }

        }

    }



}

