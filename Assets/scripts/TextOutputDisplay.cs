using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextOutputDisplay : MonoBehaviour {

	private Text text;
	// Use this for initialization
    void Start() {
    	text = gameObject.GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetTextOutput (string t) {
		text.text = t;
	}
}
