using UnityEngine;

public class LineOutput : MonoBehaviour {
	[SerializeField] private int position = 0;
	[SerializeField] private int samplerate = 44100;
	[SerializeField] private float frequency = 1000;

	private AudioSource aud;
	private AudioClip myClip;

	void Start() {
		aud = GetComponent<AudioSource>();
		PlayClip();
	}

	void PlayClip() {
		aud.Stop();
		myClip = AudioClip.Create("MySinusoid", samplerate * 2, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
		aud.clip = myClip;
		aud.Play();
	}

	public void setFrequencyString (string sVal) {
		float val = frequency;
		if (float.TryParse(sVal, out val)) {
			frequency = val;
		}
		PlayClip();
	}

	void OnAudioRead(float[] data) {
		int count = 0;
		while (count < data.Length) {
			//data[count] = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * frequency * position / samplerate));
			data[count] = Mathf.Sin(2 * Mathf.PI * frequency * position / (float)samplerate);
			position++;
			count++;
		}
	}

	void OnAudioSetPosition(int newPosition) {
	    position = newPosition;
	}
}