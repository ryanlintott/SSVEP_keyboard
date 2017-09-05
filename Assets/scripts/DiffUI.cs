using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class DiffUI : MonoBehaviour {

	public Slider _diffSlider;
	public UICircle _diffTime;
	public Text _diffLabel;
	public Slider _triggerLowSlider;
	public Slider _triggerHighSlider;
	public Slider _resetLowSlider;
	public Slider _resetHighSlider;

	public float diffMin = -0.1f;
	public float diffMax = 0.1f;

	// Use this for initialization
	void Start () {
		_diffSlider.minValue = diffMin;
		_diffSlider.maxValue = diffMax;
		_triggerLowSlider.minValue = diffMin;
		_triggerLowSlider.maxValue = diffMax;
		_triggerHighSlider.minValue = diffMin;
		_triggerHighSlider.maxValue = diffMax;
		_resetLowSlider.minValue = diffMin;
		_resetLowSlider.maxValue = diffMax;
		_resetHighSlider.minValue = diffMin;
		_resetHighSlider.maxValue = diffMax;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void UpdateDiff (float diff) {
		_diffSlider.value = diff;
		_diffLabel.text = diff.ToString("0.0000");
	}

	public void UpdateTriggerPercent (int timePercent) {
		_diffTime.FillPercent = timePercent;
	}

	public void UpdateTriggers (float trigLow, float trigHigh, float resetLow, float resetHigh) {
		_triggerLowSlider.value = trigLow;
		_triggerHighSlider.value = trigHigh;
		_resetLowSlider.value = resetLow;
		_resetHighSlider.value = resetHigh;
	}

}
