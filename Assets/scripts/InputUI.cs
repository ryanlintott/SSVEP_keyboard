using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUI : MonoBehaviour {

	public int activePanel = 0;
	public GameObject[] _panels;

	// Use this for initialization
	void Start () {
		ActivatePanel();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void ActivatePanel () {
		foreach (GameObject p in _panels) {
			p.active = false;
		}
		_panels[activePanel].active = true;
	}

	public void NextPanel () {
		activePanel = (activePanel + 1) % _panels.Length;
		ActivatePanel();
	}

	public void TogglePanel (bool toggle) {
		if (toggle) {
			activePanel = 1;
		} else {
			activePanel = 0;
		}
		ActivatePanel();
	}
}
