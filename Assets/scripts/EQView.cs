using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EQView : MonoBehaviour {

	private Slider[] eqBars;
	private float max = 0.0f;

	// Use this for initialization
	void Start () {
		eqBars = GetComponentsInChildren<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateEQ (float[] values) {
		for (int i = 0; i < eqBars.Length; i++) {
			float tempValue = values[(i*values.Length)/eqBars.Length];
			if (tempValue > max) {
				max = tempValue;
			}
			eqBars[i].value = (tempValue/max);
		}
	}
}
