using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SSVEPKeyboardView : MonoBehaviour {

	public GameObject grid;
	private Flicker[] gridKeys;
	private Text[] gridKeyText;

	public Color baseColor;
    public Color hz1Color;
    public Color hz2Color;
    public float hz1;
    public float hz2;

	// Use this for initialization
	void Start () {
		gridKeys = GetComponentsInChildren<Flicker>();
		gridKeyText = GetComponentsInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetKeyLetters (string[] keys) {
		for (int i = 0; i < keys.Length; i++) {
			gridKeyText[i].text = keys[i];
		}	
	}

	public void SetKeyStates (int[] states) {
		for (int i = 0; i < states.Length; i++) {
			gridKeys[i].c1 = baseColor;
			switch (states[i]) {
				case 0:
					gridKeys[i].cycleHz = 0;
					gridKeys[i].c2 = baseColor;
					break;
				case 1:
					gridKeys[i].cycleHz = hz1;
					gridKeys[i].c2 = hz1Color;
					break;
				case 2:
					gridKeys[i].cycleHz = hz2;
					gridKeys[i].c2 = hz2Color;
					break;
			}
			//Debug.Log(i.ToString());
		}
	}

	public void SetKeyboardKey (int i, int state, string key) {
		gridKeys[i].c1 = baseColor;
		gridKeyText[i].text = key;
		switch (state) {
			case 0:
				gridKeys[i].cycleHz = 0;
				gridKeys[i].c2 = baseColor;
				break;
			case 1:
				gridKeys[i].cycleHz = hz1;
				gridKeys[i].c2 = hz1Color;
				break;
			case 2:
				gridKeys[i].cycleHz = hz2;
				gridKeys[i].c2 = hz2Color;
				break;
		}
	}
}
