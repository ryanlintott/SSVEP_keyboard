using UnityEngine;

public class InputUI : MonoBehaviour {

	[SerializeField] private int activePanel;
	[SerializeField] private GameObject[] _panels;

	void Start () {
		ActivatePanel();
	}
	
	void Update () {
		
	}

	void ActivatePanel () {
		foreach (GameObject p in _panels) {
			p.SetActive(false);
		}

		if (_panels.Length > activePanel) {
			_panels[activePanel].SetActive(true);
		}
	}

	public void NextPanel () {
		activePanel = (activePanel + 1) % _panels.Length;
		ActivatePanel();
	}

	public void TogglePanel (bool toggle) {
		if (toggle) {
			activePanel = 1;
		} else {
			activePanel = 0;
		}
		ActivatePanel();
	}
}
