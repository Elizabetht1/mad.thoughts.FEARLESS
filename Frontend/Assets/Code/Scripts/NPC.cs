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
    [SerializeField] private AudioSource audioSource;

    private void Start() {
        api.OnNPCResponse += speak;
    }

    private void speak(object sender, APIManager.NPCResponseArgs e) {
        AudioClip clip = e.audioClip;
        // play audio clip
        if (clip != null) {
            audioSource.clip = clip;
            audioSource.Play();
        }

        Debug.Log("npc responded");
    }
}
