// ---------------------------------------
// Creation Date: 
// Author: 
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public class PlayerSpokeArgs { public AudioClip audioClip; }
    public event EventHandler<PlayerSpokeArgs> OnPlayerSpoke;

    private AudioClip audioClip;

    private string[] devices;

    private float[] clipSampleData = new float[1024];
    private float speakThreshold = 0.2f;
    private float silenceThreshold = 0.05f;
    private int secsListen = 5;
    private int secsRecord = 60;
    private int freq = 44100;
    
    private float timeSinceSpeak = 0f;
    private float delay = 2f;

    private bool isSpeaking = false;

    private void Start() {
        // start the recording on the microphone 
        devices = Microphone.devices;
        audioClip = Microphone.Start(devices[0], true, secsListen, freq);
    }
    
    private void Update() {
        float volume = GetMicVolume();
        if (!isSpeaking) {
            HandleListening(volume);
        } else {
            HandleRecording(volume);
        }
    }

    private float GetMicVolume() {
        // read audioClip data and calculate volume from it
        if (timeSinceSpeak > 3) return 0;
        return 1f;
    }

    private void HandleListening(float volume) {
        if (volume > speakThreshold) { // check if user is speaking
            Microphone.End(devices[0]); // stop listening
            isSpeaking = true;
            audioClip = Microphone.Start(devices[0], false, secsRecord, freq); // start recording
        }
    }

    private void HandleRecording(float volume) {
        timeSinceSpeak += Time.deltaTime;

        if (timeSinceSpeak > delay && volume < silenceThreshold) { // check if user is no longer speaking
            Microphone.End(devices[0]); // stop recording
            timeSinceSpeak = 0;
            OnPlayerSpoke?.Invoke(this, new PlayerSpokeArgs {audioClip = audioClip});
            Debug.Log("recorded player speaking");
            audioClip = Microphone.Start(devices[0], true, secsListen, freq); // start listening
        }
    }
}
