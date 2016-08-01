using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.Microphone;

public class MicrophoneInput : MonoBehaviour {

	public AudioSource audio;
	private float[] samples;
	private float[] processedSamples;
	private float[] lastSamples;
	private float[] smallSamples;
	private List<float[]> smallSampleArrays;
	public EQView _eqView;
	public bool useMicrophone = true;
	public bool useClamp = true;
	public float leftClamp = 0.0f;
	public float rightClamp = 1.0f;
	public float smoothing = 0.0f;
	public int inputHz = 441000;
	public int fTarget = 1000;
	public int fWidth = 120;
	private int startValue;
	private int endValue;
	private int numSamplesTaken = 0;
	public int maxSampesTaken = 0;
	public int sampleAverageWidth = 0;
	private int numSamples = 8192;
	private float fMax;
	private FFTWindow specFFTwindow = FFTWindow.Hanning;
	//public int activeAudioClip = 0;
	//public AudioClip[] audioClips;

	
	void Awake () {
		audio.loop = true;
		//audio.clip.set(audioClips[activeAudioClip]);
		samples = new float[numSamples];
		lastSamples = new float[numSamples];
		processedSamples = new float[numSamples];
		fMax = AudioSettings.outputSampleRate;
		//fMax = inputHz;
		Debug.Log(fMax.ToString());
		if (useMicrophone) {
			audio.clip = Microphone.Start("Built-in Microphone", true, 10, inputHz);
			while (Microphone.GetPosition(null) <= 0){}
		}
		//audio.mute = true;
		audio.Play();
		audio.GetSpectrumData (samples, 0, specFFTwindow);

		for (int i = 0; i < numSamples; i++) {
			//samples[i] = Mathf.Abs(samples[i]);
			samples[i] = 0.0f;
		}

		// startValue = Mathf.FloorToInt((fTarget - (fWidth / 2)) * numSamples / fMax);
		// endValue = Mathf.FloorToInt((fTarget + (fWidth / 2)) * numSamples / fMax);

		// smallSamples = new float[endValue - startValue];
		// smallSampleArrays = new List<float[]>();

		// for (int i = startValue; i < endValue; i++) {
		// 	smallSamples[i - startValue] = Mathf.Abs(samples[i]);
		// 	samples[i] = Mathf.Abs(samples[i]);
		// }
	}

	public void ResetSamples() {
		audio.GetSpectrumData (samples, 0, specFFTwindow);
		for (int i = 0; i < numSamples; i++) {
			samples[i] = Mathf.Abs(samples[i]);
		}
		numSamplesTaken = 0;
	}
	
	// Update is called once per frame
	void Update () {
		ReadSamples();
	}

	void NoProcessSamples () {
		System.Array.Copy(samples,processedSamples,numSamples);
	}

	void AverageSamples () {
		for (int i = 0; i < numSamples; i++) {
			processedSamples[i] = 0.0f;
			for (int j = Mathf.Max(i - sampleAverageWidth,0); j < Mathf.Min(i + sampleAverageWidth + 1,numSamples); j++) {
				processedSamples[i] += samples[j];
			}
			processedSamples[i] /= Mathf.Min(i + sampleAverageWidth + 1,numSamples) - Mathf.Max(i - sampleAverageWidth,0);
			processedSamples[i] = samples[i] - processedSamples[i];
		}
	}


	void AverageNeighbourSamples () {
		for (int i = 0; i < numSamples; i++) {
			processedSamples[i] = Mathf.Abs(samples[i] - Mathf.Lerp(samples[Mathf.Max(i-sampleAverageWidth,0)],samples[Mathf.Min(i+sampleAverageWidth,numSamples-1)],0.5f));
		}
	}

	void PeakSamples () {
		for (int i = 0; i < numSamples; i++) {
			if (samples[i] > samples[Mathf.Max(i-sampleAverageWidth,0)] && samples[i] > samples[Mathf.Min(i+sampleAverageWidth,numSamples-1)]) {
				processedSamples[i] = Mathf.Abs(samples[i] - Mathf.Lerp(samples[Mathf.Max(i-sampleAverageWidth,0)],samples[Mathf.Min(i+sampleAverageWidth,numSamples-1)],0.5f));
			} else {
				processedSamples[i] = 0.0f;
			}
		}
	}

	void PeaksOnlySamples () {
		for (int i = 0; i < numSamples; i++) {
			if (samples[i] > samples[Mathf.Max(i-sampleAverageWidth,0)] && samples[i] > samples[Mathf.Min(i+sampleAverageWidth,numSamples-1)]) {
				processedSamples[i] = 1.0f;
			} else {
				processedSamples[i] = 0.0f;
			}
		}
	}

	void DrawDebugLines () {
		Debug.Log(numSamplesTaken.ToString());

		int xScale = 8;
		int yScale = 400;
		int xoffset = 3000;
		int yoffset = 1000;
		for (int i = 1; i < numSamples-1; i++) {
		 	Debug.DrawLine (new Vector3 ((i - 1)*xScale-xoffset, samples[i]*yScale-yoffset, 0),
						new Vector3 (i*xScale-xoffset, samples[i + 1]*yScale-yoffset, 0), Color.red);

			Debug.DrawLine (new Vector3 ((i - 1)*xScale-xoffset, Mathf.Log(samples[i])*yScale-yoffset, 0),
						new Vector3 (i*xScale-xoffset, Mathf.Log(samples[i + 1])*yScale-yoffset, 0), Color.cyan);

			Debug.DrawLine (new Vector3 (Mathf.Log(i - 1)*xScale-xoffset, samples[i]*yScale-yoffset, 0),
						new Vector3 (Mathf.Log(i)*xScale-xoffset, samples[i + 1]*yScale-yoffset, 0), Color.green);

			Debug.DrawLine (new Vector3 (Mathf.Log(i - 1)*xScale-xoffset, Mathf.Log(samples[i])*yScale-yoffset, 0),
						new Vector3 (Mathf.Log(i)*xScale-xoffset, Mathf.Log(samples[i + 1])*yScale-yoffset, 0), Color.yellow);
		 }
	}

	void ReadSamples () {

		System.Array.Copy(samples,lastSamples,numSamples);
		audio.GetSpectrumData (samples, 0, specFFTwindow);
		//audio.GetOutputData (samples, 0);
		//float maxSample = Mathf.Max(samples);
		float maxSample = Mathf.Max(samples);
		for (int i = 0; i < numSamples; i++) {
			samples[i] = smoothing * lastSamples[i] + (1/smoothing) * Mathf.Abs(samples[i])/maxSample;
			//samples[i] = (lastSamples[i] * numSamplesTaken + Mathf.Abs(samples[i])) / (numSamplesTaken + 1);
		}

		//AverageSamples();
		//AverageNeighbourSamples();
		PeakSamples();
		//PeaksOnlySamples();
		//NoProcessSamples();

		maxSample = Mathf.Max(processedSamples);
		for (int i = 0; i < numSamples; i++) {
			processedSamples[i] = processedSamples[i]/maxSample;
			//samples[i] = (lastSamples[i] * numSamplesTaken + Mathf.Abs(samples[i])) / (numSamplesTaken + 1);
		}

		//DrawDebugLines();

		numSamplesTaken++;

		if (useClamp) {
			//Used for updating eq with % clamps on either side
			_eqView.UpdateEQClamped(processedSamples, leftClamp, rightClamp);
		} else {
			//Used for updating eq with a target range specified in Hz
			_eqView.UpdateEQHzRange(processedSamples, fTarget, fWidth, fMax/2);
		}
		
		//Debug.Log(samples[512].ToString());

	}
}
