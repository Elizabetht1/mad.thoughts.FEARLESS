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

    public int sampleRate = 44100;  // Common sample rate
    public int channels = 1;

    public string audioUrl;
    private string speechFileName = "player-speech";
    private string speechFilePath;

    private void Start() {
        Debug.Log("I am alive!");
    
        audioSource = gameObject.AddComponent<AudioSource>();
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
        StartCoroutine(postGenQuestionReq(audioData));
    }

    private void GetResponse(AudioClip clip) {
        OnNPCResponse?.Invoke(this, new NPCResponseArgs { audioClip = clip });
    }

    /**
    Make a request to generate a question to the backend
    */
    float[] ConvertByteToFloat(byte[] byteData)
    {
        int samples = byteData.Length / 2; // Assuming 16-bit PCM
        float[] floatData = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            short sample = (short)(byteData[i * 2] | (byteData[i * 2 + 1] << 8));
            floatData[i] = sample / 32768.0f; // Normalize to range -1.0 to 1.0
        }

        return floatData;
    }
    

    private IEnumerator postGenQuestionReq(byte[] audioData) {

        /* Step 1: Send a post request to the server to generate conversation based on posted parameters */
        GenConvoReq convoReq = new GenConvoReq{
                agentRole = "interviewer",
                agentTone = "neutral",
                userRole = "interviewee",
                userQuery = "give me a job pretty please",
        };

        //construct body of http request
        string json = JsonUtility.ToJson(convoReq); 
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json); 

        var req = new UnityWebRequest(ApiAdress, "POST");
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        
        // Wait for server to complete text generation procedure
        var asyncOperation = req.SendWebRequest();

        while (!asyncOperation.isDone)
        {

            float progress = req.downloadProgress;
            Debug.Log("Loading " + progress);
            yield return null;
        }

        while (!req.isDone){
            yield return null; 
        }
        
        /* Step 2: Receive generate text resource url from the server */
        if (req.isNetworkError)
        {
            Debug.Log("Error While Sending: " + req.error);
        } else{
            Debug.Log("Received data! : " + string.Join(", ", req.downloadHandler.text));
            audioUrl = string.Join(", ", req.downloadHandler.text);
        }
    

        /* Step 3: Play the audio clip according the generated conversation parameters*/
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.MPEG)){
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error downloading audio: " + request.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                GetResponse(clip);
                // audioSource.clip = clip;
                // audioSource.Play();
            }
        }
         //     GenConvoReq convoReq = new GenConvoReq{
    //             agentRole = "interviewer",
    //             agentTone = "neutral",
    //             userRole = "interviewee",
    //             userQuery = "give me a job pretty please",
    //     };
    //     string json = JsonUtility.ToJson(convoReq);
    //     var req = new UnityWebRequest(ApiAdress, "POST");


    //     byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        
        
    //     req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
    //     req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    //     req.SetRequestHeader("Content-Type", "application/json");

    //     //Send the request then wait here until it returns
    //     var asyncOperation = req.SendWebRequest();

    //     while (!asyncOperation.isDone)
    //     {
    //     // wherever you want to show the progress:

    //         float progress = req.downloadProgress;
    //         Debug.Log("Loading " + progress);
    //         yield return null;
    //     }

    //     while (!req.isDone){
    //         yield return null; // this worked for me
    //     }
        


    //     if (req.isNetworkError)
    //     {
    //         Debug.Log("Error While Sending: " + req.error);
    //     } else{
    //         Debug.Log("Received data!");
    //         byte[] data = req.downloadHandler.data;
    //         float[] floatData = ConvertByteToFloat(data);
    //         AudioClip clip = AudioClip.Create("testClip", floatData.Length, channels, sampleRate, false);
    //         clip.SetData(floatData, 0);
    //         audioSource.clip = clip;
    //         audioSource.Play();

    //         // AudioClip clip = DownloadHandlerAudioClip.GetContent(req);
    //         // Debug.Log( clip + " length: " + clip.length );
    //         // if (clip)
    //         // {
    //         //     GetComponent<AudioSource>().clip = clip;
    //         //     GetComponent<AudioSource>().Play();
    //         // }

    //     }

    // }

    /*
    use get requests with this: UnityWebRequestMultimedia
    */
    }

    // Uncomment if using other method for converting audio to byte data method 
    // private void OnApplicationQuit() {
    //     if (File.Exists(speechFilePath)) File.Delete(speechFilePath);
    // }

}

