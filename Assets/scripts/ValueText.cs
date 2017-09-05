using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ValueText : MonoBehaviour {

	private Text t;

	// Use this for initialization
	void Start () {
		t = GetComponentInParent<Text>();
	}

	public void SetValue (float val) {
		t.text = val.ToString("0.0000");
	}
}

