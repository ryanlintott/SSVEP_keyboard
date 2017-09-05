using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ChartLineDataUI : MonoBehaviour {

	public UILineRenderer _UILine;
	private LineRenderer _line;
	public Vector2 dataBottomLeftValue = new Vector2(0f, 0f);
	public Vector2 dataTopRightValue = new Vector2(1f, 1f);
	public GameObject _verticalMarkerLinePrefab;
	private string lineTag = "lineTag";

	// Use this for initialization
	void Start () {
		//_UILine = GetComponentInParent<UILineRenderer>();
		//SetVerticalMarkerLine(1f, "test");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateUILine (Vector2[] data) {

		//set number of points in UI Line
		_UILine.Points = data;

		// for (int i = 0; i < data.Length; i++) {
		// 	//set all values for UI Line
		// }

	}

	public void ShowDataOneD (float[] values) {
		Vector2[] newData;
		newData = new Vector2[values.Length];

		for (int i = 0; i < values.Length; i++) {
			newData[i] = new Vector2(Mathf.Lerp(dataBottomLeftValue.x, dataTopRightValue.x, ((float)i / (float)values.Length)), values[i]);
		}
		UpdateUILine(newData);
	}

	public static void SetRect(RectTransform trs, float left, float top, float right, float bottom) {
		trs.offsetMin = new Vector2(left, bottom);
		trs.offsetMax = new Vector2(-right, -top);
	}

	public void RemoveVerticalMarkerLines () {
		foreach (Transform child in transform) {
			if (child.tag == lineTag) {
				Destroy(child.gameObject);
			}
		}
	}

	public void SetVerticalMarkerLine (float x, string label) {
		GameObject line = Instantiate(_verticalMarkerLinePrefab) as GameObject;
		//line.tag = lineTag;
		RectTransform rt = line.GetComponent<RectTransform>();
		rt.parent = GetComponent<RectTransform>();
		rt.anchorMin = new Vector2(x, 0f);
		rt.anchorMax = new Vector2(x, 1f);
		rt.localPosition = new Vector3(0f, 0f, -2f);
		rt.localScale = new Vector3(1f, 1f, 1f);
		SetRect(rt, 0f, 0f, -2f, 0f);
		line.GetComponentInChildren<Text>().text = label;
	}

}
