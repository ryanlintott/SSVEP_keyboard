using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RektTransform;

public class ToggleEEG : MonoBehaviour {

	private GridLayoutGroup eeggrid;
	private RectTransform eegrect;
	private float topSmall;
	private float bottomSmall;
	private float eqHeightSmall;
	public float topLarge = 0.0f;
	public float bottomLarge = 0.0f;
	public float eqHeightLarge = 700.0f;
	private bool sizeLarge = false;


	// Use this for initialization
	void Start () {
		eeggrid = GetComponentInChildren<GridLayoutGroup>();
		eegrect = GetComponent<RectTransform>();
		topSmall = eegrect.GetTop().y;
		bottomSmall = eegrect.GetBottom().y;
		eqHeightSmall = eeggrid.cellSize.y;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ToggleEEGsize () {
		
		if (sizeLarge) {
			eeggrid.cellSize = new Vector2(eeggrid.cellSize.x, eqHeightSmall);
			//eeggrid.cellSize.Set(eeggrid.cellSize.x, eqHeightSmall);
			eegrect.SetTop(topSmall);
			eegrect.SetBottom(bottomSmall);
			sizeLarge = false;
			Debug.Log("ToggleEEGSmall Cellsize: "+eeggrid.cellSize.y.ToString()+"eqHeight: "+eegrect.GetTop().y.ToString());
		} else {
			eeggrid.cellSize = new Vector2(eeggrid.cellSize.x, eqHeightLarge);
			//eeggrid.cellSize.Set(eeggrid.cellSize.x, eqHeightLarge);
			eegrect.SetTop(topLarge);
			eegrect.SetBottom(bottomLarge);
			sizeLarge = true;
			Debug.Log("ToggleEEGLarge Cellsize: "+eeggrid.cellSize.y.ToString()+"eqHeight: "+eegrect.GetTop().y.ToString());
		}

	}
}
