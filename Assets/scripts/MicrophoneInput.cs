using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using UnityEngine.Microphone;

public class MicrophoneInput : MonoBehaviour {

	public bool _readSamplesOn = true;
	public AudioSource _audio;
	public AudioSource _tone;
	public AudioClip _demoTone;					//Demo tone to use in the app
	private float[] samples;					//full range of recorded samples
	private float[] sampleSet;					//small subset of samples used for analysis
	private float[] sampleSetAvg;				//small subset of samples used for analysis
	private float[,] sampleSetPrev;				//saved from previous read
	private float[] sampleSetProcessed;			//processed for interpretation
	public EQView _eqView;
	public ChartLineDataUI _chartLineDataUI;
	public bool useMicrophone = true;
	public bool useClamp = true;

	private int sampleRate;						//44100 Hz is the typical sample rate
	private int deviceSampleRateMin;			//Value in Hz
	private int deviceSampleRateMax;			//Value in Hz
	private float pitchMultiplier;				//Pitch multiplier when playing back the audio clip. Used to increase signal resolution
	private float fMax;							//half of the sample rate
	public int fTarget = 1000;					//Target in HZ
	public int fRangeWidth = 120;				//Width of area of interest around target in HZ
	private float fToSampleRange;				//Ratio of frequency to sample range
	public float ssvepLowF = 15.0f;				//Flicker speed in Hz for low frequency flashes
	public float ssvepHighF = 20.0f;			//Flicker speed in Hz for high frequency flashes
	private int sampleRangeStart;				//The first bucket for FFT samples in our sample range
	private int sampleRangeEnd;					//The last bucket for FFT samples in our sample range
	private int numSamples = 4096;				//Must be power of 2  Min: 64, Max: 8192

	private int sampleSetSize;
	private int[] ssvepLowValues;
	private int[] ssvepHighValues;
	private int numSamplesTaken = 0;
	public bool averageOverTime = false;
	public int _avgTimeSamples = 0;					//Number of frames to sample.  Set to 0 for infinite samples
	private int sampleSetCounter = 0;			//Keeps track of location in sampleSetPrev
	public int sAvgWidth = 1;					//Number of samples to average together. Default: 1 is no averaging
	public int sampleProcessingMode = 0;
	public float _logBase = 10f;
	
	private FFTWindow specFFTwindow = FFTWindow.Hanning;
	private float diff = 0.0f;
	public int diffTrigger = 0;
	public int triggerTime = 60;				//Number of frames it takes within a range to trigger
	public float triggerHigh = 0.1f;
	public float triggerLow = -0.1f;
	public float triggerResetHigh = 0.001f;
	public float triggerResetLow = -0.001f;
	//public int activeAudioClip = 0;
	//public AudioClip[] audioClips;

	
	void Awake () {
		InitializeAudio();
	}

	// Update is called once per frame
	void Update () {
		if (_readSamplesOn) {
			ReadSamples();
		}
	}

	public void NextProcessingMode() {
		sampleProcessingMode = (sampleProcessingMode + 1)%5;
	}

	public void MicToggle() {
		useMicrophone =  !useMicrophone;
		InitializeAudio();
		//ResetSamples();
	}

	private void SetLowestSampleRateMobile() {
		//No loger necessary with pitch adjustments
		#if UNITY_IOS
			Debug.Log("iPhone");
			AudioConfiguration config = AudioSettings.GetConfiguration();
			config.sampleRate = 11025;
			AudioSettings.Reset(config);
		#endif

		#if UNITY_ANDROID
			Debug.Log("Android");
			AudioConfiguration config = AudioSettings.GetConfiguration();
			config.sampleRate = 11025;
			AudioSettings.Reset(config);
		#endif
	}

	public void ResetSamples() {
		samples = new float[numSamples];
		_audio.GetSpectrumData (samples, 0, specFFTwindow);

		sampleSet = new float[sampleSetSize];
		sampleSetProcessed = new float[sampleSetSize];

		if (averageOverTime) {
			sampleSetCounter = 0;
			sampleSetAvg = new float[sampleSetSize];
			sampleSetPrev = new float[Mathf.Max(_avgTimeSamples, 1), sampleSetSize];
		}
		numSamplesTaken = 0;
		diffTrigger = 0;
	}

	void InitializeAudio () {

		//Determine the output sample rate
		sampleRate = Mathf.FloorToInt(AudioSettings.outputSampleRate);
		Debug.Log("outputSampleRate: " + sampleRate.ToString());

		

		//finding the best whole number pitchMultiplier we can use to scale up the signal to fix the available frequencies
		//(sampleRate / 2) gives us the max potential frequency and the rest gives us the maximum frequency we care about
		pitchMultiplier = Mathf.Floor((sampleRate / 2) / (fTarget + (fRangeWidth / 2)));
		fMax = AudioSettings.outputSampleRate / 2;
		fToSampleRange = pitchMultiplier * numSamples / fMax;

		sampleRangeStart = Mathf.FloorToInt((fTarget - (fRangeWidth / 2)) * fToSampleRange);
		sampleRangeEnd = Mathf.FloorToInt((fTarget + (fRangeWidth / 2)) * fToSampleRange);
		sampleSetSize = sampleRangeEnd - sampleRangeStart;

		//Set sample location for SSVEP samples
		//only low and high values [1], those to the right of 1kHz are used at the moment but those on the left are still stored
		ssvepLowValues = new int[2];
		ssvepLowValues[0] = Mathf.FloorToInt((fTarget - ssvepLowF) * fToSampleRange) - sampleRangeStart;
		ssvepLowValues[1] = Mathf.FloorToInt((fTarget + ssvepLowF) * fToSampleRange) - sampleRangeStart;

		ssvepHighValues = new int[2];
		ssvepHighValues[0] = Mathf.FloorToInt((fTarget - ssvepHighF) * fToSampleRange) - sampleRangeStart;
		ssvepHighValues[1] = Mathf.FloorToInt((fTarget + ssvepHighF) * fToSampleRange) - sampleRangeStart;

		//Add visual guides for marker lines
		_chartLineDataUI.RemoveVerticalMarkerLines();
		_chartLineDataUI.SetVerticalMarkerLine((float)ssvepLowValues[0] / (float)sampleSetSize, ssvepLowF.ToString());
		_chartLineDataUI.SetVerticalMarkerLine((float)ssvepHighValues[0] / (float)sampleSetSize, ssvepHighF.ToString());
		_chartLineDataUI.SetVerticalMarkerLine((float)ssvepLowValues[1] / (float)sampleSetSize, ssvepLowF.ToString());
		_chartLineDataUI.SetVerticalMarkerLine((float)ssvepHighValues[1] / (float)sampleSetSize, ssvepHighF.ToString());

		//Debug.Log("fMax: "+fMax.ToString());
		if (useMicrophone) {
			foreach(string s in Microphone.devices) {
				Microphone.GetDeviceCaps(s, out deviceSampleRateMin, out deviceSampleRateMax);	
				Debug.Log("Device Name: " + s + " [" + deviceSampleRateMin.ToString() + "-" + deviceSampleRateMax.ToString() + "]");
			}

			//_audio.clip = Microphone.Start("Built-in Microphone", true, 10, sampleRate);
			_audio.clip = Microphone.Start(null, true, 10, sampleRate);
			while (Microphone.GetPosition(null) <= 0){}
		} else {
			_audio.clip = _demoTone;
		}

		//_audio.mute = true;
		_audio.pitch = pitchMultiplier;
		_audio.loop = true;
		_audio.Play();

		ResetSamples();

	}

	void ReadSamples () {
		// Read in audio data to samples
		_audio.GetSpectrumData (samples, 0, specFFTwindow);

		numSamplesTaken++;

		//Don't normalize until after averaging over time

		// Copy a small subset of samples to sampleSet
		System.Array.Copy(samples,sampleRangeStart,sampleSet,0,sampleSet.Length);

		// Create a processed sample set
		System.Array.Copy(sampleSet,sampleSetProcessed,sampleSet.Length);

		// Average the samples over time.  If maxSamples = 1 this will just return the same values
		if (averageOverTime) {
			if (_avgTimeSamples == 0) {
				sampleSetAvg = AverageSamplesForever(sampleSetProcessed, sampleSetAvg, numSamplesTaken);	
			} else {
				sampleSetAvg = AverageSamplesTime(sampleSetProcessed, sampleSetAvg, sampleSetPrev, numSamplesTaken, sampleSetCounter);

				// Copy samplesets to previous sample sets for use in averaging
				for (int i = 0; i < sampleSetSize; i++) {
					sampleSetPrev[sampleSetCounter,i] = sampleSet[i];
				}

				// Increment the counter used to identify which previous sample set to use next
				sampleSetCounter = (sampleSetCounter + 1) % _avgTimeSamples;
			}
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
				sampleSetProcessed = RelativePeakSamples(sampleSetProcessed, sAvgWidth);
				break;
			case 3:
				sampleSetProcessed = PeakSamples(sampleSetProcessed, sAvgWidth);
				break;
			case 4:
				sampleSetProcessed = PeaksOnlySamples(sampleSetProcessed, sAvgWidth);
				break;
		}

		// Normalize data for visual output and diff comparison
		//sampleSetProcessed = LogScale(sampleSetProcessed, _logBase);
		//sampleSetProcessed = NormalizeToZero(sampleSetProcessed);
		sampleSetProcessed = NormalizeToZeroDoubleSqrt(sampleSetProcessed);

		//Calculate the difference in the height of the low and high frequency peaks.
		diff = sampleSetProcessed[ssvepHighValues[0]] - sampleSetProcessed[ssvepLowValues[0]];
		if (diff>triggerHigh) {
			++diffTrigger;
		} else if (diff<triggerLow) {
			--diffTrigger;
		} else if ((diff<triggerResetHigh) && (diff>triggerResetLow)) {
			//Reset the diffTrigger value if it drops out of range for low or high triggers
			diffTrigger = 0;
		}

		//Debug.Log("Diff: " + diff.ToString("0.0000"));
		//Debug.Log(string.Format("{0:####.000} Hz", diff));
		//Debug.Log("diffTrigger: " + diffTrigger);
		
		//DrawDebugLines();
		//Debug.Log(sampleSetProcessed[0].ToString());

		_chartLineDataUI.ShowDataOneD(sampleSetProcessed);

	}

	//Average sIn samples with sAvg equivalent to t (number of elements averaged)
	static float[] AverageSamplesForever(float[] sIn, float[] sAvg, int n) {
		if (n == 1) {
			return sIn;
		}
		float[] sOut = new float[sIn.Length];
		for (int i = 0; i < sIn.Length; i++) {
			sOut[i] = sAvg[i] + ((sIn[i] - sAvg[i]) / (float)n);
		}
		return sOut;
	}

	//Average sIn samples with sPrev
	static float[] AverageSamplesTime(float[] sIn, float[] sAvg, float[,] sPrev, int n, int c) {
		if (n == 1) {
			return sIn;
		}
		float[] sOut = new float[sIn.Length];
		int maxS = sPrev.GetLength(0);
		if (n < maxS) {
			for (int i = 0; i < sIn.Length; i++) {
				sOut[i] = sAvg[i] + ((sIn[i] - sAvg[i]) / (float)n);
			}
		} else {	
			for (int i = 0; i < sIn.Length; i++) {
				sOut[i] = sAvg[i] + ((sIn[i] - sPrev[c,i]) / (float)maxS);
			}
		}
		return sOut;
	}

	static float[] FoldSamples(float[] sIn) {
		float[] sOut = new float[Mathf.CeilToInt(sIn.Length/2)];
		for (int i = 0; i < sOut.Length; i++) {
			sOut[i] = sIn[i] + sIn[sIn.Length-i-1];
		}
		return sOut;
	}

	static float[] SubtractAverage(float[] sIn) {
		float[] sOut = new float[sIn.Length];
		float sum = 0f;
		for (int i = 0; i < sIn.Length; i++) {
			sum += sIn[i];
		}
		float avg = sum / (float)sIn.Length;
		for (int i = 0; i < sIn.Length; i++) {
			sOut[i] = sIn[i] - avg;
		}
		return sOut;
	}

	static float[] Normalize(float[] sIn) {
		float[] sOut = new float[sIn.Length];
		float maxSample = sIn.Max();
		float minSample = sIn.Min();
		for (int i = 0; i < sIn.Length; i++) {
			sOut[i] = Mathf.InverseLerp(minSample, maxSample, sIn[i]);
		}
		return sOut;
	}

	static float[] NormalizeToZero(float[] sIn) {
		float[] sOut = new float[sIn.Length];
		float maxSample = sIn.Max();
		for (int i = 0; i < sIn.Length; i++) {
			sOut[i] = Mathf.InverseLerp(0f, maxSample, sIn[i]);
		}
		return sOut;
	}

	static float[] NormalizeDoubleSqrt(float[] sIn) {
		float[] sOut = new float[sIn.Length];
		float maxSample = sIn.Max();
		float minSample = sIn.Min();
		for (int i = 0; i < sIn.Length; i++) {
			sOut[i] = Mathf.Sqrt(Mathf.Sqrt(Mathf.InverseLerp(minSample, maxSample, sIn[i])));
		}
		return Normalize(sOut);
	}

	static float[] LogScale(float[] sIn, float logBase) {
		float[] sOut = new float[sIn.Length];
		for (int i = 0; i < sIn.Length; i++) {
			sOut[i] = Mathf.Log(sIn[i]+1f, logBase);
		}
		return sOut;
	}

	static float[] NormalizeToZeroDoubleSqrt(float[] sIn) {
		float[] sOut = new float[sIn.Length];
		float maxSample = sIn.Max();
		for (int i = 0; i < sIn.Length; i++) {
			sOut[i] = Mathf.Sqrt(Mathf.Sqrt(Mathf.InverseLerp(0f, maxSample, sIn[i])));
		}
		return NormalizeToZero(sOut);
	}

	//Each output sample will be an average of itself and w neighbours on each side.
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

	//Each output sample will be the input sample minus the midpoint between the neighbour vales w away in each direction.
	//Negative values are zeroed as we're only looking for relative peaks.
	static float[] RelativePeakSamples(float[] sIn, int w) {
		float[] sOut = new float[sIn.Length];
		for (int i = 0; i < sIn.Length; i++) {
			sOut[i] = Mathf.Max(sIn[i] - Mathf.Lerp(sIn[Mathf.Max(i-w,0)],sIn[Mathf.Min(i+w,sIn.Length-1)],0.5f),0);
		}
		return sOut;
	}

	//Each output sample will be the input sample minus the midpoint between the neighbour vales w away in each direction.
	//Any value less than either neighbour is zeroed as we're only looking for true peaks.
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

	//Any peaks (measuring sample against neighbour w away) are output as 1 and all other values are zero
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
		//Debug.Log(numSamplesTaken.ToString());

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
