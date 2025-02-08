// ---------------------------------------
// Creation Date:
// Author: 
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIManager : MonoBehaviour
{
    // events
    public class NPCResponseArgs { public AudioClip audioClip; }
    public event EventHandler<NPCResponseArgs> OnNPCResponse;

    [SerializeField] private Player player;
    [SerializeField] private AudioSource audioSource;

    private void Start() {
        player.OnPlayerSpoke += OnPlayerSpoke;
    }

    private void OnPlayerSpoke(object sender, Player.PlayerSpokeArgs e) {
        Debug.Log("player said something");
        audioSource.clip = e.audioClip;
        audioSource.Play();
        // convert audio clip to wav file
        // make a post request, sending wav file
    }

    private void getRequest() {
        AudioClip clip = null;
        // make a get request and set clip to response
        // fire event to make NPC respond
        OnNPCResponse?.Invoke(this, new NPCResponseArgs { audioClip = clip });
    }
}
