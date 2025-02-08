// ---------------------------------------
// Creation Date:
// Author: 
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
    [SerializeField] private AudioSource audioSource;

    private string speechFileName = "player-speech";
    private string speechFilePath;

    private void Start() {
        Debug.Log("I am alive!");
        StartCoroutine(postGenQuestionReq());
        player.OnPlayerSpoke += OnPlayerSpoke;
        speechFilePath = Application.dataPath + "/" + speechFileName + ".wav";
    }

    private void OnPlayerSpoke(object sender, Player.PlayerSpokeArgs e) {
        /* // Other method that converts audio to wav then wav to byte[]
           SavWav.Save(speechFileName, e.audioClip);
           byte[] audioData = File.ReadAllBytes(speechFilePath); */

        // Better method that converts straight to byte array
        int sampleCount = e.audioClip.samples * e.audioClip.channels;
        float[] tmp = new float[sampleCount];
        e.audioClip.GetData(tmp, 0);
        byte[] audioData = WavUtility.ConvertAudioClipDataToInt16ByteArray(tmp);
        // make a post request, sending byte data
    }

    private void GetResponse() {
        byte[] audioData = null; // update with response
        AudioClip clip = WavUtility.ToAudioClip(audioData);

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
        var asyncOperation = req.SendWebRequest();

        while (!asyncOperation.isDone)
        {
        // wherever you want to show the progress:

            float progress = req.downloadProgress;
            Debug.Log("Loading " + progress);
            yield return null;
        }

        while (!req.isDone){
            yield return null; // this worked for me
        }
        


        if (req.isNetworkError)
        {
            Debug.Log("Error While Sending: " + req.error);
        } else{
            Debug.Log("Received: " + string.Join(", ", req.downloadHandler.data));
            yield return req.downloadHandler.data;

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




    // Uncomment if using other method for converting audio to byte data method 
    // private void OnApplicationQuit() {
    //     if (File.Exists(speechFilePath)) File.Delete(speechFilePath);
    // }

}

