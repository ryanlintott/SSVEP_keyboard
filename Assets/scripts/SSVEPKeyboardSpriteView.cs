using UnityEngine;

public class SSVEPKeyboardSpriteView : MonoBehaviour {

	[SerializeField] private GameObject _keyPrefab;
	[SerializeField] private GameObject _keyParent;
	[SerializeField] private GameObject _keyboardArea;
	[SerializeField] private Color baseColor;
	[SerializeField] private Color hzBaseColor;
	[SerializeField] private Color hz1Color;
	[SerializeField] private Color hz2Color;
	[SerializeField] private Color selectedColor;
	[SerializeField] private float hz1;
	[SerializeField] private float hz2;

	private FlickerSprite[] spriteKeysFlicker;
	private TextMesh[] spriteKeyText;
	private bool keyboardActive = true;
	private float keyboardWidth;
	private float keyboardHeight;
	private float keyHeight;
	private float keyScale = 0.5f;
	private float keySize;
	private float keyShiftX;
	private int keyRows;
	private int keyColumns;
	private int keyCount;
	private Vector3 topLeftPos;


	// Use this for initialization
	void Awake () {
		Application.targetFrameRate = 60;

		keyboardWidth = _keyboardArea.transform.localScale.x * 10f;
		keyboardHeight = _keyboardArea.transform.localScale.z * 10f;
	}
	
	// Update is called once per frame
	void Update () {
		if (keyboardActive) FlickerKeys();
	}

	void Start () {
		//find the width and height of the keyboard area
	}

	void FlickerKeys () {
		for (int i = 0; i < spriteKeysFlicker.Length; i++) {
			spriteKeysFlicker[i].MakeFlicker();
		}
	}

	private float SquaresInRect (int n, float x, float y) {
		//max area of a square where n will fit the area
		float maxA = (x * y) / (float)n;

		//max side of that square
		float sx = Mathf.Sqrt(maxA);
		
		//num squares to cover x side
		int px = Mathf.CeilToInt(x / sx);

		//size of px squares that fit x side
		sx = x / (float)px;

		//how many could possibly fit in y?
		int py = Mathf.FloorToInt(y / sx);

		//how many squares total?
		if (px * py < n) {
			//if there's not enough shrink to fit in y
			py = Mathf.CeilToInt(y / sx);

			if (px * py < n) Debug.Log("ERROR - not enough keys! sx");
			//size of px squares that fit y side
			sx = y / py;
		}

		//try the whole thing again from the other axis
		
		//max side of that square
		float sy = Mathf.Sqrt(maxA);
		
		//num squares to cover y side
		py = Mathf.CeilToInt(y / sy);

		//size of py squares that fit y side
		sy = y / (float)py;

		//how many could possibly fit in x?
		px = Mathf.FloorToInt(x / sy);

		//how many squares total?
		if (px * py < n) {
			//if there's not enough shrink to fit in x
			px = Mathf.CeilToInt(x / sy);

			if (px * py < n) Debug.Log("ERROR - not enough keys! sy");
			//size of px squares that fit y side
			sy = x / (float)px;
		}

		Debug.Log("sx"+sx+"sy"+sy);
		return Mathf.Max(sx,sy);
	}

	public void BuildKeyboard (int numKeys) {
		//stop flickering for a bit
		keyboardActive = false;

		//destroy all current keys
		foreach (Transform child in _keyParent.transform) {
			Destroy(child.gameObject);
		}
		_keyParent.transform.DetachChildren();

		keyHeight = SquaresInRect(numKeys, keyboardWidth, keyboardHeight);
		keyRows = Mathf.FloorToInt(keyboardHeight / keyHeight);
		keyColumns = Mathf.FloorToInt(keyboardWidth / keyHeight);
		keyShiftX = 0;
		keySize = (keyHeight / 10f);
		keyScale = 0.5f;

		topLeftPos = new Vector3( (-(keyHeight * keyColumns) / 2f) + (keyHeight / 2f), ((keyHeight * keyRows) / 2f) - (keyHeight / 2f), 0f);

		keyCount = 0;
		for (int y = 0; y < keyRows; y++) {
			if (y == keyRows - 1) {
				//shift to center keys on last row
				keyShiftX = ((keyColumns - (numKeys - keyCount)) * keyHeight) / (float)(numKeys - keyCount);
			}
			for (int x = 0; x < keyColumns; x++) {
				if (keyCount < numKeys) {
					keyCount += 1;
					GameObject childKey = Instantiate(_keyPrefab) as GameObject;
					childKey.transform.rotation = _keyParent.transform.rotation;
					childKey.transform.parent = _keyParent.transform;
					childKey.transform.localPosition = new Vector3(topLeftPos.x + ((keyHeight + keyShiftX) * x) + (keyShiftX / 2f), topLeftPos.y - (keyHeight * y), topLeftPos.z);
					childKey.transform.localScale = new Vector3(keySize, keySize, keySize);
					childKey.transform.GetChild(0).transform.localScale = new Vector3(keyScale, keyScale, keyScale);

				}
			}
		}

		spriteKeysFlicker = _keyParent.GetComponentsInChildren<FlickerSprite>();
		spriteKeyText = _keyParent.GetComponentsInChildren<TextMesh>();

		keyboardActive = true;

	}

	public void ScaleKeys (float scale) {

		keyScale = scale;
		keySize = (keyHeight / 10f) * keyScale;
		foreach (FlickerSprite childKey in spriteKeysFlicker) {
			childKey.gameObject.transform.localScale = new Vector3(keySize, keySize, keySize);
		}

	}

	public void SetKeyLetters (string[] keys) {
		for (int i = 0; i < keys.Length; i++) {
			spriteKeyText[i].text = keys[i];
		}	
	}

	public void SetKeyboardKey (int i, int state, string key) {
		spriteKeysFlicker[i].c1 = hzBaseColor;
		spriteKeyText[i].text = key;
		switch (state) {
			case 0:
				spriteKeysFlicker[i].cycleHz = 0;
				spriteKeysFlicker[i].c2 = hzBaseColor;
				break;
			case 1:
				spriteKeysFlicker[i].cycleHz = hz1;
				spriteKeysFlicker[i].c2 = hz1Color;
				break;
			case 2:
				spriteKeysFlicker[i].cycleHz = hz2;
				spriteKeysFlicker[i].c2 = hz2Color;
				break;
			case 3:
				spriteKeysFlicker[i].cycleHz = 0;
				spriteKeysFlicker[i].c2 = selectedColor;
				break;
			case 4:
				spriteKeysFlicker[i].cycleHz = 0;
				spriteKeysFlicker[i].c2 = baseColor;
				break;
		}
	}
}
