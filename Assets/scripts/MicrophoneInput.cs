using UnityEngine;
using System.Collections;
//using UnityEngine.Microphone;

public class MicrophoneInput : MonoBehaviour {

	public AudioSource audio;
	private float[] samples;
	public EQView _eqView;

	private int numSamples = 1024;
	// Use this for initialization
	void Start () {
		audio.clip = Microphone.Start("Built-in Microphone", true, 10, 44100);
		audio.loop = true;
		samples = new float[numSamples];
		//while (!Microphone.GetPosition(null)){}
		audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
		audio.GetSpectrumData (samples, 0, FFTWindow.Hamming);
		_eqView.UpdateEQ(samples);

		Debug.Log(samples[512].ToString());
	}
}
