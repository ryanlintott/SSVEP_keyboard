using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fps : MonoBehaviour {

	private float deltaTime = 0.0f;
	public Text textOutput;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame 
	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("Framerate: {0:0.0} ms ({1:0.} fps) Signal Output: {2:0.} HZ", msec, fps, AudioSettings.outputSampleRate);
		textOutput.text = text;

	}

}
