using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenSizePlane : MonoBehaviour {

	public Camera _cam;
	//public float _dist = 1000f;
	public bool _matchToCam;
	public bool _fitWidth;
	//private Vector3 _screenPos;
	private Vector3 _screenScale;
	//private Vector3 _screenRot;

	// Use this for initialization
	void Start () {
		_matchToCam = true;
		_screenScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
		//_screenPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		if (_matchToCam) {
			_matchToCam = false;
			_screenScale.z = _cam.orthographicSize*0.2f;
			_screenScale.x = _screenScale.z * Screen.width / Screen.height;
			_screenScale.y = 1f;
			if (_fitWidth) {
				_screenScale.z = _screenScale.y = _screenScale.x;
			}
			
			//_screenPos = _cam.transform.position - new Vector3(0f, 0f, _dist);
			//_screenRot = _cam.transform.rotation * new Quaternion.Euler(-90, 0, 0);

			transform.localScale = _screenScale;
			//transform.position = _screenPos;
			//transform.rotation = _cam.transform.rotation;
		}
	}
}
