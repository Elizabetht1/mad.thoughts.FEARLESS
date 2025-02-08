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
        SavWav.Save(speechFileName, e.audioClip);
        // make a post request, sending wav file
    }

    private void getRequest() {
        AudioClip clip = null;
        // make a get request and set clip to response
        // fire event to make NPC respond
        OnNPCResponse?.Invoke(this, new NPCResponseArgs { audioClip = clip });
    }

    private void OnApplicationQuit() {
        if (File.Exists(speechFilePath)) File.Delete(speechFilePath);
    }
}
