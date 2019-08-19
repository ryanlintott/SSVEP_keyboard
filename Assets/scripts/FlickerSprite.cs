using UnityEngine;
using System.Collections;

public class FlickerSprite : MonoBehaviour {
    public Color c1;
    public Color c2;
    //public Material mat;
    public float cycleHz; // Hz, the mesurement of cycles.
    private SpriteRenderer _spriteRenderer;
    private int updateCounter = 0;
    private bool swap = false;
    private TouchScreenKeyboard keyboard;

    float dtime = 0; // delta time

    void Awake() {
    	_spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void Start() {
    	_spriteRenderer.color = c1;
    }

    public void MakeFlicker() {

		// old equation (I was making my own time.time for some reason)
		//dtime += Time.deltaTime;
		//float wave = Mathf.Sin( (dtime * 2.0f * Mathf.PI) * cycleHz);

		// Sin works on RAD not DEG.
		// Sin wave flashing is best for SSVEP
		// Time in seconds (mod to keep small values) * Pi (one sin wave per second) * cycleHz (cycleHz sin waves per second) / 2 (half a sin wave per cycleHz)
		float colorMix = Mathf.InverseLerp(-1f, 1f, Mathf.Sin((Time.time % 10.0f) * Mathf.PI * cycleHz * 2.0f));

		_spriteRenderer.color = Color.Lerp(c1, c2, colorMix);

		// Count cycles (also used if I want full flashing colours insetad of smooth sin wave)
		if (colorMix > 0.5f) {
			//_spriteRenderer.color = c1;
			if (swap) {
				updateCounter++;
				swap = false;
			}
		} else {
			//_spriteRenderer.color = c2;
			swap = true;
		}

		//Debug.LogFormat("Cycle Count = {0}", updateCounter);
		//Debug.LogFormat("Accuracy = {0}", Time.time - (updateCounter / cycleHz));

		//Debug.Log("Seconds: " + Time.time.ToString() + " Flashes: " + updateCounter.ToString() + " Hz expected: " + cycleHz.ToString() + " Hz actual: " + (updateCounter / Time.time).ToString());
		//Debug.Log((1/Time.deltaTime).ToString());
	}

	public void SetHz(float newHz) {
        if (newHz >= 0) {
            cycleHz = newHz;
        }
    }

    public void SetHzString(string newHzString) {
        if (newHzString.Length > 0) {
            SetHz(float.Parse(newHzString));
        }
    }

}
