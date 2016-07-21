using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SSVEPKeyboardView : MonoBehaviour {

	public GridLayoutGroup grid;
	private Flicker[] gridKeys;
	private Text[] gridKeyText;
	private bool keyboardActive = false;

	public Color baseColor;
	public Color hzBaseColor;
    public Color hz1Color;
    public Color hz2Color;
    public Color selectedColor;
    public float hz1;
    public float hz2;

	// Use this for initialization
	void Awake () {
		gridKeys = GetComponentsInChildren<Flicker>();
		gridKeyText = GetComponentsInChildren<Text>();

	}
	
	// Update is called once per frame
	void Update () {
		FlickerKeys();
	}

	void FlickerKeys () {
		for (int i = 0; i < gridKeys.Length; i++) {
			gridKeys[i].MakeFlicker();
		}
	}

	public void SetKeyLetters (string[] keys) {
		for (int i = 0; i < keys.Length; i++) {
			gridKeyText[i].text = keys[i];
		}	
	}

	public void SetKeyboardKey (int i, int state, string key) {
		gridKeys[i].c1 = hzBaseColor;
		gridKeyText[i].text = key;
		switch (state) {
			case 0:
				gridKeys[i].cycleHz = 0;
				gridKeys[i].c2 = hzBaseColor;
				break;
			case 1:
				gridKeys[i].cycleHz = hz1;
				gridKeys[i].c2 = hz1Color;
				break;
			case 2:
				gridKeys[i].cycleHz = hz2;
				gridKeys[i].c2 = hz2Color;
				break;
			case 3:
				gridKeys[i].cycleHz = 0;
				gridKeys[i].c2 = selectedColor;
				break;
			case 4:
				gridKeys[i].cycleHz = 0;
				gridKeys[i].c2 = baseColor;
				break;
		}
	}
}
