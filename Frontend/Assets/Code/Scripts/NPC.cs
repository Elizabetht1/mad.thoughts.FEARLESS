// ---------------------------------------
// Creation Date:
// Author: 
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private APIManager api;
    [SerializeField] private Player player;
    [SerializeField] private AudioSource audioSource;

    private bool playerIsSpeaking = false;
    private float timePlayerSpeaking = 0f;
    private float stopThreshold = 1.25f;

    private void Start() {
        api.OnNPCResponse += Speak;
        player.OnIsSpeakingChange += (_, e) => { playerIsSpeaking = e.isSpeaking; };
    }

    private void Speak(object sender, APIManager.NPCResponseArgs e) {
        AudioClip clip = e.audioClip;
        // play audio clip
        if (clip != null && (!playerIsSpeaking || timePlayerSpeaking < stopThreshold)) { // don't play response if player is still speaking
            audioSource.clip = clip;
            audioSource.Play();
        }

        Debug.Log("NPC responded");
    }

    private void Update() {
        if (playerIsSpeaking) {
            timePlayerSpeaking += Time.deltaTime;

            if (timePlayerSpeaking > stopThreshold) { // stop response if player starts speaking
                audioSource.Stop();
            }
        } else {
            timePlayerSpeaking = 0;
        }
    }
}