using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEngine.Microphone;

public class MicrophoneInput : MonoBehaviour {

	public bool readSamplesOn = true;
	public AudioSource audio;
	public AudioSource tone;
	private float[] samples;					//full range of recorded samples
	private float[] sampleSet;					//small subset of samples used for analysis
	private float[] sampleSetAvg;				//small subset of samples used for analysis
	private float[,] sampleSetPrev;				//saved from previous read
	private float[] sampleSetProcessed;			//processed for interpretation
	private float[,] sampleSetProcessedPrev;	//saved from previous read
	public EQView _eqView;
	public bool useMicrophone = true;
	public bool useClamp = true;
	public float leftClamp = 0.0f;
	public float rightClamp = 1.0f;
	public int inputHz;
	public int fTarget = 1000;
	public int fWidth = 120;
	public float ssvepLowF = 12.0f;
	public float ssvepHighF = 20.0f;
	private int startValue;
	private int endValue;
	private int sampleSetSize;
	private int[] ssvepLowValues;
	private int[] ssvepHighValues;
	private int numSamplesTaken = 0;
	public bool averageOverTime = false;
	public int maxSamples = 0;					//Number of frames to sample.  Set to 0 for infinite samples
	private int sampleSetCounter = 0;			//Keeps track of location in sampleSetPrev
	private bool useMaxSamples;
	public int sAvgWidth = 1;					//Number of samples to average together. Default: 1 is no averaging
	public int sampleProcessingMode = 0;
	private int numSamples = 8192;
	private float fMax;
	private FFTWindow specFFTwindow = FFTWindow.Hanning;
	//public int activeAudioClip = 0;
	//public AudioClip[] audioClips;

	
	void Awake () {
		InitializeAudio();
	}

	// Update is called once per frame
	void Update () {
		if (readSamplesOn) {
			ReadSamples();
		}
	}

	public void ResetSamples() {
		audio.GetSpectrumData (samples, 0, specFFTwindow);
		for (int i = 0; i < numSamples; i++) {
			samples[i] = Mathf.Abs(samples[i]);
		}
		numSamplesTaken = 0;
	}

	public void NextProcessingMode() {
		sampleProcessingMode = (sampleProcessingMode + 1)%5;
	}

	void InitializeAudio () {
		audio.loop = true;
		//audio.clip.set(audioClips[activeAudioClip]);
		samples = new float[numSamples];
		
		fMax = AudioSettings.outputSampleRate / 2;
		startValue = Mathf.FloorToInt((fTarget - (fWidth / 2)) * numSamples / fMax);
		endValue = Mathf.FloorToInt((fTarget + (fWidth / 2)) * numSamples / fMax);
		sampleSetSize = endValue - startValue;

		ssvepLowValues = new int[2];
		ssvepLowValues[0] = Mathf.FloorToInt((fTarget - ssvepLowF) * numSamples / fMax) - startValue;
		ssvepLowValues[1] = Mathf.FloorToInt((fTarget + ssvepLowF) * numSamples / fMax) - startValue;

		ssvepHighValues = new int[2];
		ssvepHighValues[0] = Mathf.FloorToInt((fTarget - ssvepHighF) * numSamples / fMax) - startValue;
		ssvepHighValues[1] = Mathf.FloorToInt((fTarget + ssvepHighF) * numSamples / fMax) - startValue;		

		if (maxSamples > 0 ) {
			useMaxSamples = true;
		} else {
			useMaxSamples = false;
			maxSamples = 1;
		}
		sampleSet = new float[sampleSetSize];
		sampleSetPrev = new float[maxSamples,sampleSetSize];
		sampleSetProcessed = new float[sampleSetSize];
		sampleSetProcessedPrev = new float[maxSamples,sampleSetSize];
		sampleSetAvg = new float[sampleSetSize];

		//fMax = inputHz;
		Debug.Log("fMax: "+fMax.ToString());
		if (useMicrophone) {
			audio.clip = Microphone.Start("Built-in Microphone", true, 10, inputHz);
			while (Microphone.GetPosition(null) <= 0){}
		}
		//audio.mute = true;
		audio.Play();
		audio.GetSpectrumData (samples, 0, specFFTwindow);
	}

	void ReadSamples () {
		// Read in audio data to samples
		audio.GetSpectrumData (samples, 0, specFFTwindow);
		//audio.GetOutputData (samples, 0);

		numSamplesTaken++;

		// Copy a small subset of samples to sampleSet
		System.Array.Copy(samples,startValue,sampleSet,0,sampleSet.Length);

		// Create a processed sample set
		System.Array.Copy(sampleSet,sampleSetProcessed,sampleSet.Length);		

		//sampleSetProcessed = FoldSamples(sampleSetProcessed);
		// Normalize the data
		sampleSetProcessed = NormalizeSamples(sampleSetProcessed);

		// Average the samples over time.  If maxSamples = 1 this will just retrun the same values
		if (averageOverTime) {
			sampleSetAvg = AverageSamplesTime(sampleSetProcessed, sampleSetAvg, sampleSetPrev, numSamplesTaken, sampleSetCounter, useMaxSamples);
			sampleSetProcessed = sampleSetAvg;
		}

		switch (sampleProcessingMode) {
			case 0:
				//NoProcessSamples();
				break;
			case 1:
				sampleSetProcessed = AverageSamples(sampleSetProcessed, sAvgWidth);
				break;
			case 2:
				sampleSetProcessed = AverageNeighbourSamples(sampleSetProcessed, sAvgWidth);
				break;
			case 3:
				sampleSetProcessed = PeakSamples(sampleSetProcessed, sAvgWidth);
				break;
			case 4:
				sampleSetProcessed = PeaksOnlySamples(sampleSetProcessed, sAvgWidth);
				break;
		}

		// Boost the data for visual output
		sampleSetProcessed = BoostSamples(sampleSetProcessed);

		//Testing SSVEP peak locations
		if (numSamplesTaken % 5 == 0) {
			ssvepLowValues[0] = Mathf.FloorToInt((fTarget - ssvepLowF) * numSamples / fMax) - startValue;
			ssvepLowValues[1] = Mathf.FloorToInt((fTarget + ssvepLowF) * numSamples / fMax) - startValue;
			ssvepHighValues[0] = Mathf.FloorToInt((fTarget - ssvepHighF) * numSamples / fMax) - startValue;
			ssvepHighValues[1] = Mathf.FloorToInt((fTarget + ssvepHighF) * numSamples / fMax) - startValue;	
			sampleSetProcessed[ssvepLowValues[0]] = 0.2f;
			sampleSetProcessed[ssvepLowValues[1]] = 0.2f;
			sampleSetProcessed[ssvepHighValues[0]] = 0.1f;
			sampleSetProcessed[ssvepHighValues[1]] = 0.1f;	
		}
		
		//DrawDebugLines();
		//Debug.Log(sampleSetProcessed[0].ToString());

		if (useClamp) {
			//Used for updating eq with % clamps on either side
			_eqView.UpdateEQClamped(sampleSetProcessed, leftClamp, rightClamp);
		} else {
			_eqView.UpdateEQClamped(sampleSetProcessed, 0.0f, 1.0f);
		}
		
		//Debug.Log(samples[512].ToString());
		// Copy samplesets to previous sample sets for use in averaging
		for (int i = 0; i < sampleSetSize; i++) {
			sampleSetPrev[sampleSetCounter,i] = sampleSet[i];
			//sampleSetProcessedPrev[sampleSetCounter,i] = sampleSetProcessed[i];
		}

		// Increment the counter used to identify which previous sample set to use next
		sampleSetCounter = (sampleSetCounter + 1) % maxSamples;

	}

	static float[] FoldSamples(float[] sIn) {
		float[] sOut = new float[Mathf.CeilToInt(sIn.Length/2)];
		for (int i = 0; i < sOut.Length; i++) {
			sOut[i] = sIn[i] + sIn[sIn.Length-i-1];
		}
		return sOut;
	}

	static float[] NormalizeSamples(float[] sIn) {
		float[] sOut = new float[sIn.Length];
		float maxSample = sIn.Max();
		float minSample = sIn.Min();
		for (int i = 0; i < sIn.Length; i++) {
			sOut[i] = Mathf.InverseLerp(minSample, maxSample, sIn[i]);
		}
		return sOut;
	}

	static float[] BoostSamples(float[] sIn) {
		float[] sOut = new float[sIn.Length];
		for (int i = 0; i < sIn.Length; i++) {
			sOut[i] = Mathf.Sqrt(Mathf.Sqrt(sIn[i]));
		}
		return sOut;
	}

	static float[] AverageSamplesTime(float[] sIn, float[] sAvg, float[,] sPrev, int t, int c, bool useMax) {
		if (t == 1) {
			return sIn;
		}
		float[] sOut = new float[sIn.Length];
		int maxS = sPrev.GetLength(0);
		if ((t < maxS) || !useMax) {
			for (int i = 0; i < sIn.Length; i++) {
				sOut[i] = (sAvg[i] * (t-1) + sIn[i]) / t;
			}
		} else {	
			for (int i = 0; i < sIn.Length; i++) {
				sOut[i] = sAvg[i] + ((sIn[i] - sPrev[c,i]) / maxS);
			}
		}
		return sOut;
	}

	static float[] AverageSamples(float[] sIn, int w) {
		float[] sOut = new float[sIn.Length];
		for (int i = 0; i < sIn.Length; i++) {
			//sOut[i] = 0.0f;
			for (int j = Mathf.Max(i - w,0); j < Mathf.Min(i + w + 1,sIn.Length); j++) {
				sOut[i] += sIn[j];
			}
			sOut[i] /= Mathf.Min(i + w + 1,sIn.Length) - Mathf.Max(i - w,0);
			sOut[i] = sIn[i] - sOut[i];
		}
		return sOut;
	}


	static float[] AverageNeighbourSamples(float[] sIn, int w) {
		float[] sOut = new float[sIn.Length];
		for (int i = 0; i < sIn.Length; i++) {
			sOut[i] = Mathf.Abs(sIn[i] - Mathf.Lerp(sIn[Mathf.Max(i-w,0)],sIn[Mathf.Min(i+w,sIn.Length-1)],0.5f));
		}
		return sOut;
	}


	static float[] PeakSamples(float[] sIn, int w) {
		float[] sOut = new float[sIn.Length];
		for (int i = 0; i < sIn.Length; i++) {
			if (sIn[i] > sIn[Mathf.Max(i-w,0)] && sIn[i] > sIn[Mathf.Min(i+w,sIn.Length-1)]) {
				//sOut[i] = Mathf.Abs(sIn[i] - Mathf.Lerp(sIn[Mathf.Max(i-w,0)],sIn[Mathf.Min(i+w,sIn.Length-1)],0.5f));
				sOut[i] = sIn[i] - Mathf.Lerp(sIn[Mathf.Max(i-w,0)],sIn[Mathf.Min(i+w,sIn.Length-1)],0.5f);
			} else {
				sOut[i] = 0.0f;
			}
		}
		return sOut;
	}

	static float[] PeaksOnlySamples(float[] sIn, int w) {
		float[] sOut = new float[sIn.Length];
		for (int i = 0; i < sIn.Length; i++) {
			if (sIn[i] > sIn[Mathf.Max(i-w,0)] && sIn[i] > sIn[Mathf.Min(i+w,sIn.Length-1)]) {
				sOut[i] = 1.0f;
			} else {
				sOut[i] = 0.0f;
			}
		}
		return sOut;
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


}
