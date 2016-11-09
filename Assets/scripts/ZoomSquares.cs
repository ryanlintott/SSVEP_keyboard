using UnityEngine;
using System.Collections;

public class ZoomSquares : MonoBehaviour {
    public float scaleSpeed = 0.5f;
    public float minScale = 1f;
    public float maxScale = 2000f;

    public GameObject[] objs;

    void Update()
    {
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
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

            foreach(GameObject obj in objs) {
            	float scaleChange = Mathf.Clamp(obj.transform.localScale.x + obj.transform.localScale.x * deltaMagnitudeDiff * scaleSpeed, minScale, maxScale);
            	obj.transform.localScale = new Vector3(scaleChange, scaleChange, scaleChange);
            }

        }
    }
}