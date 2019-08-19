using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Fps : MonoBehaviour {

	private float deltaTime = 0.0f;
	//public Text textOutput;
	public TextMeshProUGUI textOutput;
	
	void Start () {
		deltaTime = Time.deltaTime;
	}

	void Update() {
		//Instead of just printing the framerate every frame, this calculates a rolling average using a % of the difference between frames
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("Framerate: {0:0.0} ms ({1:0.} fps) Signal Output: {2:0.} HZ", msec, fps, AudioSettings.outputSampleRate);
		textOutput.text = text;
	}

}
