using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class DiffUI : MonoBehaviour {
	
	[SerializeField] private Slider _diffSlider;
	[SerializeField] private UICircle _diffTime;
	[SerializeField] private Slider _triggerLowSlider;
	[SerializeField] private Slider _triggerHighSlider;
	[SerializeField] private Slider _resetLowSlider;
	[SerializeField] private Slider _resetHighSlider;

	public void UpdateDiff (float diff) {
		_diffSlider.value = diff;
	}

	public void UpdateTriggerPercent (float timeProgress) {
		_diffTime.SetProgress(Mathf.Min(timeProgress, 1));
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
