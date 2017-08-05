using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SSVEPKeyboardSpriteView : MonoBehaviour {

	public GameObject _keyPrefab;
	public GameObject _keyParent;
	public GameObject _keyboardArea;
	private FlickerSprite[] spriteKeys;
	private TextMesh[] spriteKeyText;
	private bool keyboardActive = false;
	private float keyboardWidth;
	private float keyboardHeight;
	private float keyHeight;
	private int keyRows;
	private int keyColumns;
	private int keyCount;
	private Vector3 topLeftPos;
	public Color baseColor;
	public Color hzBaseColor;
    public Color hz1Color;
    public Color hz2Color;
    public Color selectedColor;
    public float hz1;
    public float hz2;

	// Use this for initialization
	void Awake () {
		Application.targetFrameRate = 60;

		keyboardWidth = _keyboardArea.transform.localScale.x * 10f;
		keyboardHeight = _keyboardArea.transform.localScale.z * 10f;
	}
	
	// Update is called once per frame
	void Update () {
		FlickerKeys();
	}

	void Start () {
		//find the width and height of the keyboard area
	}

	void FlickerKeys () {
		for (int i = 0; i < spriteKeys.Length; i++) {
			spriteKeys[i].MakeFlicker();
		}
	}

	public void BuildKeyboard (int numKeys) {

		//destroy all current keys
		
		//divide it by the number of keys

		keyRows = 0;
		keyCount = 0;
		keyColumns = 0;

		//Write a better algorithm for figuring out how big the keys need to be. The ideal may still have less rows but with shorter keys to fit more columns
		while ((keyCount < numKeys) && (keyRows < 10)) {
			keyRows += 1;
			keyHeight = keyboardHeight / keyRows;
			keyColumns = Mathf.FloorToInt(keyboardWidth / keyHeight);
			keyCount = keyRows * keyColumns;
			Debug.Log("Keycount: "+keyCount);
		}

		topLeftPos = new Vector3( (-(keyHeight * keyColumns) / 2f) + (keyHeight / 2f), ((keyHeight * keyRows) / 2f) - (keyHeight / 2f), 0f);

		for (int y = 0; y < (keyRows-1); y++) {
			for (int x = 0; x < (keyColumns-1); x++) {
				GameObject childKey = Instantiate(_keyPrefab) as GameObject;
				childKey.transform.rotation = _keyParent.transform.rotation;
				childKey.transform.parent = _keyParent.transform;
				childKey.transform.localPosition = new Vector3(topLeftPos.x + (keyHeight * x), topLeftPos.y - (keyHeight * y), topLeftPos.z);
				childKey.transform.localScale = new Vector3(keyHeight / 10f, keyHeight / 10f, keyHeight / 10f);
			}
		}

		spriteKeys = _keyParent.GetComponentsInChildren<FlickerSprite>();
		spriteKeyText = _keyParent.GetComponentsInChildren<TextMesh>();
		//find each key's position and instantiate a key there
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
