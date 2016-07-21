using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextOutputDisplay : MonoBehaviour {

	private Text text;
	private string enteredString;
	private float m_TimeStamp;
	private bool cursor = false;
	private string cursorChar = "";
	private int maxStringLength = 24;

	// Use this for initialization
    void Awake() {
    	text = gameObject.GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
		if (Time.time - m_TimeStamp >= 0.5) {
			m_TimeStamp = Time.time;
			if (cursor) {
				cursor = false;
				cursorChar = "";
			} else {
				cursor = true;
				cursorChar = "_";
			}
		}
		text.text = enteredString + cursorChar;
	}

	public void SetTextOutput (string t) {
		enteredString = t;
	}

}
