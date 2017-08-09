using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenSizePlane : MonoBehaviour {

	public Camera _cam;
	public bool _matchToCam;
	public Transform _lockPosition;
	//public float _dist = 1000f;
	public bool _lockToPosition;
	public bool _liveMatch;
	public bool _fitWidth;
	public bool _fitHeight;
	private Vector3 _screenPos;
	private Vector3 _screenScale;
	//private Vector3 _screenRot;

	// Use this for initialization
	void Start () {
		_matchToCam = true;
		_screenScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
		_screenPos = transform.position;
		//_screenPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		if (_matchToCam) {
			//turns off matching if _liveMatch is off
			_matchToCam = _liveMatch;

			if (_fitHeight || _fitWidth) {
				if (_fitHeight && _fitWidth) {
					_screenScale.z = _cam.orthographicSize*0.2f;
					_screenScale.x = _screenScale.z * Screen.width / Screen.height;
				} else if (_fitHeight) {
					_screenScale.x = _screenScale.z = _cam.orthographicSize*0.2f;
				} else {
					_screenScale.z = _screenScale.x = _cam.orthographicSize*0.2f * Screen.width / Screen.height;
				}
				_screenScale.y = 1f;

				transform.localScale = _screenScale;
			}
		}
		if (_lockToPosition) {
			//turns off matching if _liveMatch is off
			_lockToPosition = _liveMatch;

			_screenPos = new Vector3(_lockPosition.transform.position.x, _lockPosition.transform.position.y, _lockPosition.transform.position.z);
			transform.position = _screenPos;
		}
	}
}
