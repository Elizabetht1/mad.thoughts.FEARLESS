// ---------------------------------------
// Creation Date:
// Author: 
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class APIManager : MonoBehaviour
{
    // events
    public class NPCResponseArgs { public AudioClip audioClip; }
    public event EventHandler<NPCResponseArgs> OnNPCResponse;

    [SerializeField] private Player player;
    [SerializeField] private AudioSource audioSource;

    private string speechFileName = "player-speech";
    private string speechFilePath;

    private void Start() {
        player.OnPlayerSpoke += OnPlayerSpoke;
        speechFilePath = Application.dataPath + "/" + speechFileName + ".wav";
    }

    private void OnPlayerSpoke(object sender, Player.PlayerSpokeArgs e) {
        /* // Other method that converts audio to wav then wav to byte[]
           SavWav.Save(speechFileName, e.audioClip);
           byte[] audioData = File.ReadAllBytes(speechFilePath); */

        // Better method that converts straight to byte array
        byte[] audioData = WavUtility.ConvertAudioClipDataToInt16ByteArray(e.audioClip.GetData());
        // make a post request, sending byte data
    }

    private void GetResponse() {
        byte[] audioData = null; // update with response
        AudioClip clip = WavUtility.ToAudioClip(audioData);

        // fire event to make NPC respond
        OnNPCResponse?.Invoke(this, new NPCResponseArgs { audioClip = clip });
    }

    // Uncomment if using other method for converting audio to byte data method 
    // private void OnApplicationQuit() {
    //     if (File.Exists(speechFilePath)) File.Delete(speechFilePath);
    // }
}
