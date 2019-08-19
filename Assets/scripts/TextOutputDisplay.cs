using UnityEngine;
using TMPro;


public class TextOutputDisplay : MonoBehaviour {

	private TextMeshProUGUI text;
	private string enteredString;
	private float m_TimeStamp;
	private bool cursor;
	private string cursorChar = "";

    void Awake() {
		text = gameObject.GetComponent<TextMeshProUGUI>();
    }
	
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
