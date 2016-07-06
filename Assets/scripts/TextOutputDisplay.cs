using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextOutputDisplay : MonoBehaviour {

	private Text textOutputUI;
	// Use this for initialization
    void Start() {
    	textOutputUI = gameObject.GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetTextOutput (string t) {
		textOutputUI.text = t;
	}
}
