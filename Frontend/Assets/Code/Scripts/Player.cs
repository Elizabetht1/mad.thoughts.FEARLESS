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
    private float speakThreshold = 0.05f;
    private float silenceThreshold = 0.03f;
    private int secsListen = 1;
    private int secsRecord = 60;
    private int freq = 44100;
    
    private float timeSinceLowVolume = 0f;
    private float delay = 2f;

    private bool isSpeaking = false;

    private void Start() {
        // start the recording on the microphone 
        devices = Microphone.devices;
        audioClip = Microphone.Start(devices[0], true, secsListen, freq);
        Debug.Log("Recording on mic: " + devices[0]);
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
        int micPosition = Microphone.GetPosition(devices[0]);
        if (micPosition > 0) {
            int startSample = Mathf.Max(0, micPosition - clipSampleData.Length);
            audioClip.GetData(clipSampleData, startSample);
        }

        float sum = 0f;
        for (int i = 0; i < clipSampleData.Length; i++) {
            sum += clipSampleData[i] * clipSampleData[i]; // Square each sample
        }
        
        float volume = Mathf.Sqrt(sum / clipSampleData.Length); // RMS calculation
        return volume;
    }

    private void HandleListening(float volume) {
        if (volume > speakThreshold) { // check if user is speaking
            Debug.Log("User is speaking");
            Microphone.End(devices[0]); // stop listening
            isSpeaking = true;
            audioClip = Microphone.Start(devices[0], false, secsRecord, freq); // start recording
        }
    }

    private void HandleRecording(float volume) {
        if (volume < silenceThreshold) {
            timeSinceLowVolume += Time.deltaTime;
            if (timeSinceLowVolume > delay) { // check if user is no longer speaking
                Debug.Log("User stopped speaking");
                Microphone.End(devices[0]); // stop recording
                timeSinceLowVolume = 0;
                isSpeaking = false;
                OnPlayerSpoke?.Invoke(this, new PlayerSpokeArgs {audioClip = audioClip});
                audioClip = Microphone.Start(devices[0], true, secsListen, freq); // start listening
            }
        } else timeSinceLowVolume = 0;
    }
}
