using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ValueText : MonoBehaviour {

	public Text _t;

	// Use this for initialization
	void Start () {
		//_t = GetComponent<Text>();
	}

	public void SetValue (float val) {
		_t.text = val.ToString("0.0000");
	}
}

