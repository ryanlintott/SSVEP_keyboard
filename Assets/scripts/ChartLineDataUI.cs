using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class ChartLineDataUI : MonoBehaviour {

	public UILineRenderer _UILine;
	private LineRenderer _line;
	public Vector2 dataBottomLeftValue = new Vector2(0f, 0f);
	public Vector2 dataTopRightValue = new Vector2(1f, 1f);

	// Use this for initialization
	void start () {
		//_UILine = GetComponentInParent<UILineRenderer>();
		Debug.Log("UILine Points:"+_UILine.Points.Length);
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

}
