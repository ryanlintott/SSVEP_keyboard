using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchZoom : MonoBehaviour {
	public float scaleSpeed = 0.5f;
	public float minScale = 0.1f;
	public float maxScale = 1f;
	public float scale = 0.5f;

	public SSVEPKeyboardSpriteView _SSVEPKeyboardSpriteView;

	void Update() {
		// If there are two touches on the device...
		if (Input.touchCount == 2) {
			// Store both touches.
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

			scale = Mathf.Clamp(scale + scale * deltaMagnitudeDiff * scaleSpeed, minScale, maxScale);

			_SSVEPKeyboardSpriteView.ScaleKeys(scale);

		}
	}
}
