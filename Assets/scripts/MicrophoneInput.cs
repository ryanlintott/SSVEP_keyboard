using UnityEngine;
using System.Collections;
//using UnityEngine.Microphone;

public class MicrophoneInput : MonoBehaviour {

	public AudioSource audio;
	private float[] samples;
	public EQView _eqView;
	public bool useMicrophone = true;

	private int numSamples = 1024;
	// Use this for initialization
	void Start () {
		audio.loop = true;
		samples = new float[numSamples];
		if (useMicrophone) {
			audio.clip = Microphone.Start("Built-in Microphone", true, 10, 44100);
			while (Microphone.GetPosition(null) <= 0){}
		}
		audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
		audio.GetSpectrumData (samples, 0, FFTWindow.Hamming);
		//audio.GetOutputData (samples, 0, FFTWindow.Hamming);
		_eqView.UpdateEQ(samples);

		//Debug.Log(samples[512].ToString());
	}
}
