using UnityEngine;
using System.Collections;

[System.Serializable]
public struct KeyboardKey {
	public string key;
	public int status;
	public float probability;
	public int keyPosition;
}

public class SSVEPKeyboardModel : MonoBehaviour {

	public TextAsset keyboardfile;
	public int numKeys = 30;
	private KeyboardKey[] keys;
	private int toggle;
	public SSVEPKeyboardView _SSVEPKeyboardView;
	private string[] keyStrings;
	private int keysLeft;
	public TextOutputDisplay _textOutputDisplay;
	private string textOutput = "";
	private string lastLetter;
	private float probSum;
	private float halfProbSum;
	public NextLetterProbability _nextLetterProbability;
	public MicrophoneInput _microphoneInput;
	public int letterDelay = 3;

	// Use this for initialization
	void Awake () {
		keys = new KeyboardKey[numKeys];
		keyStrings = keyboardfile.text.Split('\n');
		for (int i = 0; i < numKeys; i++) {
			keys[i].key = keyStrings[i];
			keys[i].status = 0;
			keys[i].keyPosition = i;
		}
		lastLetter = " ";
	}

	void Start () {
		ResetKeyboardKeys();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void LowFrequency () {
		ChooseKeyState(1);
		_microphoneInput.ResetSamples();
	}

	public void HighFrequency () {
		ChooseKeyState(2);
		_microphoneInput.ResetSamples();
	}

	public void ResetKeyboardKeys () {
		keysLeft = numKeys;
		for (int i = 0; i < numKeys; i++) {
			keys[i].status = 0;
		}
		ChooseKeyState(0);
		_microphoneInput.ResetSamples();
	}

	void ChooseKeyState (int state) {
		keysLeft = 0;
		toggle = 0;
		probSum = 0.0f;
		//Debug.Log("lastLetter: "+lastLetter);
		for (int i = 0; i < numKeys; i++) {
			if (keys[i].status == state) {
				//Debug.Log("Last Letter: "+lastLetter+" Key: "+keys[i].key);
				keys[i].probability = _nextLetterProbability.GetProbability(lastLetter,keys[i].key);

				probSum += keys[i].probability;
				//Debug.Log(keys[i].probability.ToString());
			}
		}
		halfProbSum = (probSum / 2.0f);
		//Debug.Log("probSum: "+probSum.ToString());
		//Debug.Log("halfProbSum: "+halfProbSum.ToString());
		probSum = 0.0f;
		System.Array.Sort(keys, ProbabilityCondition);

		for (int i = 0; i < numKeys; i++) {
			if (keys[i].status == state) {
				keys[i].status = (toggle+1);
				probSum += keys[i].probability;
				//Debug.Log(keys[i].key+keys[i].probability.ToString());
				//Debug.Log(probSum.ToString());
				keysLeft++;
			} else {
				keys[i].status = 0;
			}
			UpdateKeyboardKey(keys[i]);
			if (toggle == 0 && probSum >= halfProbSum) toggle = 1;
		}
		if (keysLeft == 1) {
			PressKeyboardKey();
		}
	}

	int ProbabilityCondition(KeyboardKey itemA, KeyboardKey itemB) {
		if (itemA.probability > itemB.probability) return -1;
		if (itemA.probability < itemB.probability) return 1;
		return 0;
	}

	void PressKeyboardKey () {
		for (int i = 0; i < numKeys; i++) {
			if (keys[i].status > 0) {
				// switch (keys[i].key) {
				// 	case "Space":
				// 		lastLetter = " ";
				// 		break;
				// 	default:
				// 		lastLetter = keys[i].key;
				// 		break;
				// }
				keys[i].status = 3;
				lastLetter = keys[i].key;
				textOutput += lastLetter[0];
				_textOutputDisplay.SetTextOutput(textOutput);
				//Debug.Log(keys[i].key);
				
				//add a letter to the display screen
			} else {
				//keys[i].status = 4;
			}
			UpdateKeyboardKey(keys[i]);
		}
		//wait for some time
		Invoke("ResetKeyboardKeys", letterDelay);
	}

	void UpdateKeyboardKey (KeyboardKey _keyboardKey) {
		_SSVEPKeyboardView.SetKeyboardKey(_keyboardKey.keyPosition, _keyboardKey.status, _keyboardKey.key);
	}

	public void ClearOutput () {
		textOutput = "";
		_textOutputDisplay.SetTextOutput(textOutput);
	}
}
