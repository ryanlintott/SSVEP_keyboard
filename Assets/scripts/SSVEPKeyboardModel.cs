using UnityEngine;
using System.Collections;

[System.Serializable]
public struct KeyboardKey {
	public string key;
	public int status;
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

	// Use this for initialization
	void Start () {
		keys = new KeyboardKey[numKeys];
		keyStrings = keyboardfile.text.Split('\n');
		for (int i = 0; i < numKeys; i++) {
			keys[i].key = keyStrings[i];
			keys[i].status = 0;
		}
		_textOutputDisplay.SetTextOutput("");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonUp("Fire1")) {
			Debug.Log("Fire1");
			ChooseKeyState(1);
		}
		if (Input.GetButtonUp("Fire2")) {
			Debug.Log("Fire2");
			ChooseKeyState(2);
		}
		if (Input.GetKeyUp("space")) {
			ResetKeyboardKeys();
		}
	}

	void ResetKeyboardKeys () {
		keysLeft = numKeys;
		for (int i = 0; i < numKeys; i++) {
			keys[i].status = 0;
		}
		ChooseKeyState(0);
	}

	void ChooseKeyState (int state) {
		keysLeft = 0;
		toggle = 0;
		for (int i = 0; i < numKeys; i++) {
			if (keys[i].status == state) {
				keys[i].status = toggle+1;
				toggle = (toggle+1) % 2;
				keysLeft++;
			} else {
				keys[i].status = 0;
			}
			UpdateKeyboardKey(i);
		}
		if (keysLeft == 1) {
			PressKeyboardKey();
		}
	}

	void PressKeyboardKey () {
		for (int i = 0; i < numKeys; i++) {
			if (keys[i].status > 0) {
				switch (keys[i].key) {
					case "space":
						textOutput += " ";
						break;
					default:
						textOutput += keys[i].key;
						break;
				}
				_textOutputDisplay.SetTextOutput(textOutput);
				Debug.Log(keys[i].key);
				//add a letter to the display screen
			}
		}
		ResetKeyboardKeys();
	}

	void UpdateKeyboardKey (int i) {
		_SSVEPKeyboardView.SetKeyboardKey(i, keys[i].status, keys[i].key);
	}
}
