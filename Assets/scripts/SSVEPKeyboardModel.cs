using UnityEngine;

[System.Serializable]
public struct KeyboardKey {
	public string key;
	public int status;
	public float probability;
	public int keyPosition;
}

public class SSVEPKeyboardModel : MonoBehaviour {

	[SerializeField] private TextAsset[] _keyboardFiles;
	[SerializeField] private SSVEPKeyboardSpriteView _SSVEPKeyboardSpriteView;
	[SerializeField] private TextOutputDisplay _textOutputDisplay;
	[SerializeField] private NextLetterProbability _nextLetterProbability;
	[SerializeField] private MicrophoneInput _microphoneInput;
	[SerializeField] private int letterDelay = 2;
	[SerializeField] private bool useSSVEP = false;

	private int currentKeyboard = 0;
	private int numKeys;
	private KeyboardKey[] keys;
	private int toggle;
	
	private string[] keyStrings;
	private int keysLeft;
	
	private string textOutput = "";
	private string lastLetter;
	private float probSum;
	private float halfProbSum;
	
	void Awake () {
		InitializeKeyboard();
	}

	void Start () {
		_SSVEPKeyboardSpriteView.BuildKeyboard(numKeys);
		ResetKeyboardKeys();
	}
	
	// Update is called once per frame
	void Update () {
		if (useSSVEP)  {
			if (_microphoneInput.diffTrigger > _microphoneInput.triggerTime) {
				HighFrequency();
			}
			if (_microphoneInput.diffTrigger < -_microphoneInput.triggerTime) {
				LowFrequency();
			}
		}
	}

	private void InitializeKeyboard () {
		keyStrings = _keyboardFiles[currentKeyboard].text.Split('\n');
		numKeys = keyStrings.Length;
		keys = new KeyboardKey[numKeys];
		for (int i = 0; i < numKeys; i++) {
			keys[i].key = keyStrings[i];
			keys[i].status = 0;
			keys[i].keyPosition = i;
		}
		lastLetter = " ";

	}

	public void ToggleKeyboard () {
		currentKeyboard = (currentKeyboard + 1) % _keyboardFiles.Length;
		InitializeKeyboard();
		ClearOutput();
		_SSVEPKeyboardSpriteView.BuildKeyboard(numKeys);
		ResetKeyboardKeys();
	}

	public void ToggleUseSSVEP (bool toggleButton) {
		useSSVEP = toggleButton;
	}

	public void LowFrequency () {
		useSSVEP = false;
		ChooseKeyState(1);
		_microphoneInput.ResetSamples();
		useSSVEP = true;
	}

	public void HighFrequency () {
		useSSVEP = false;
		ChooseKeyState(2);
		_microphoneInput.ResetSamples();
		useSSVEP = true;
	}

	public void ResetKeyboardKeys () {
		keysLeft = numKeys;
		for (int i = 0; i < numKeys; i++) {
			keys[i].status = 0;
		}
		ChooseKeyState(0);
		_microphoneInput.TurnAudioOn();
	}

	void ChooseKeyState (int state) {
		keysLeft = 0;
		toggle = 0;
		probSum = 0.0f;

		for (int i = 0; i < numKeys; i++) {
			if (keys[i].status == state) {
				keys[i].probability = _nextLetterProbability.GetProbability(lastLetter,keys[i].key);
				probSum += keys[i].probability;
			}
		}
		halfProbSum = (probSum / 2.0f);

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
		_microphoneInput.TurnAudioOff();
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

				//this is a bit of a hack but it will only type the first character of the name of the key. It's used for spacebar as the first character is just a space
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
		_SSVEPKeyboardSpriteView.SetKeyboardKey(_keyboardKey.keyPosition, _keyboardKey.status, _keyboardKey.key);
	}

	public void ClearOutput () {
		textOutput = "";
		_textOutputDisplay.SetTextOutput(textOutput);
	}
}
