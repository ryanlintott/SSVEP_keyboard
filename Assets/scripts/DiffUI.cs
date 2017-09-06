using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class DiffUI : MonoBehaviour {

	public Slider _diffSlider;
	public UICircle _diffTime;
	public Slider _triggerLowSlider;
	public Slider _triggerHighSlider;
	public Slider _resetLowSlider;
	public Slider _resetHighSlider;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void UpdateDiff (float diff) {
		_diffSlider.value = diff;
	}

	public void UpdateTriggerPercent (int timePercent) {
		_diffTime.FillPercent = Mathf.Min(timePercent, 100);
	}

	public void SetDiffSettings (float diffMin, float diffMax, float triggerLow, float triggerHigh, float resetLow, float resetHigh) {
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
		_triggerLowSlider.value = triggerLow;
		_triggerHighSlider.value = triggerHigh;
		_resetLowSlider.value = resetLow;
		_resetHighSlider.value = resetHigh;
	}

	public void SetDiffMinMax (float diffMin, float diffMax) {
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

	public void SetTriggers (float triggerLow, float triggerHigh, float resetLow, float resetHigh) {
		_triggerLowSlider.value = triggerLow;
		_triggerHighSlider.value = triggerHigh;
		_resetLowSlider.value = resetLow;
		_resetHighSlider.value = resetHigh;
	}

}
