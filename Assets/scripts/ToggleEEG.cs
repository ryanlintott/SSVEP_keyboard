using UnityEngine;
using RektTransform;

public class ToggleEEG : MonoBehaviour {

	[SerializeField] RectTransform largeVersion;
	
	private RectTransform eegrect;
	private float smallHeight;
	private float largeHeight;
	private bool sizeLarge = false;

	// Use this for initialization
	void Awake () {
		eegrect = GetComponent<RectTransform>();
		smallHeight = eegrect.GetHeight();
		largeHeight = largeVersion.GetHeight();

	}

	public void ToggleEEGsize () {
		if (sizeLarge) {
			eegrect.SetHeight(smallHeight);
			sizeLarge = false;
		} else {
			eegrect.SetHeight(largeHeight);
			sizeLarge = true;
		}
	}
}
