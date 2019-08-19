using UnityEngine;
//using UnityEngine.UI;
using TMPro;


public class ValueText : MonoBehaviour {

	//public Text _t;
	private TextMeshProUGUI _t;

	// Use this for initialization
	void Awake() {
		_t = gameObject.GetComponent<TextMeshProUGUI>();
		Debug.Log(_t.ToString());
	}

	public void SetValue (float val) {
		string text = val.ToString("0.0000");
		_t.text = text;
	}
}

