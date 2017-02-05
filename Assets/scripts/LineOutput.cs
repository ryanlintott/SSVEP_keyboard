using UnityEngine;
using System;

public class LineOutput : MonoBehaviour {
    public int position = 0;
    public int samplerate = 44100;
    public float frequency = 1000;

    void Start() {
        AudioClip myClip = AudioClip.Create("MySinusoid", samplerate * 2, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
        AudioSource aud = GetComponent<AudioSource>();
        aud.clip = myClip;
        aud.Play();
    }

    void OnAudioRead(float[] data) {
        int count = 0;
        while (count < data.Length) {
            data[count] = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * frequency * position / samplerate));
            position++;
            count++;
        }
    }
    
    void OnAudioSetPosition(int newPosition) {
        position = newPosition;
    }
}

