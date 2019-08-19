using UnityEngine;
using TMPro;


public class ValueText : MonoBehaviour {

	private TextMeshProUGUI _t;

	void Awake() {
		_t = gameObject.GetComponent<TextMeshProUGUI>();
	}

	public void SetValue (float val) {
		string text = val.ToString("0.0000");
		_t.text = text;
	}
}

