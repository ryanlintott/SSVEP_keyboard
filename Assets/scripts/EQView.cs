using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EQView : MonoBehaviour {

	private Slider[] eqBars;
	private float max = 0.0f;
	private int groupValues;

	// Use this for initialization
	void Start () {
		eqBars = GetComponentsInChildren<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateEQ (float[] values) {
		groupValues = values.Length/eqBars.Length;
		Debug.Log(groupValues.ToString());
		for (int i = 0; i < eqBars.Length; i++) {
			float tempValue = 0.0f;
			for (int j = 0; j < groupValues; j++) {
				tempValue += values[i*groupValues+j];
			}
			if (tempValue > max) {
				max = tempValue;
			}
			eqBars[i].value = (tempValue/max);
		}
	}
}
