using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SSVEPKeyboardSpriteView : MonoBehaviour {

	public GameObject keyParent;
	private FlickerSprite[] spriteKeys;
	private TextMesh[] spriteKeyText;
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
		spriteKeys = GetComponentsInChildren<FlickerSprite>();
		spriteKeyText = GetComponentsInChildren<TextMesh>();

	}
	
	// Update is called once per frame
	void Update () {
		FlickerKeys();
	}

	void FlickerKeys () {
		for (int i = 0; i < spriteKeys.Length; i++) {
			spriteKeys[i].MakeFlicker();
		}
	}

	public void SetKeyLetters (string[] keys) {
		for (int i = 0; i < keys.Length; i++) {
			spriteKeyText[i].text = keys[i];
		}	
	}

	public void SetKeyboardKey (int i, int state, string key) {
		spriteKeys[i].c1 = hzBaseColor;
		spriteKeyText[i].text = key;
		switch (state) {
			case 0:
				spriteKeys[i].cycleHz = 0;
				spriteKeys[i].c2 = hzBaseColor;
				break;
			case 1:
				spriteKeys[i].cycleHz = hz1;
				spriteKeys[i].c2 = hz1Color;
				break;
			case 2:
				spriteKeys[i].cycleHz = hz2;
				spriteKeys[i].c2 = hz2Color;
				break;
			case 3:
				spriteKeys[i].cycleHz = 0;
				spriteKeys[i].c2 = selectedColor;
				break;
			case 4:
				spriteKeys[i].cycleHz = 0;
				spriteKeys[i].c2 = baseColor;
				break;
		}
	}
}
