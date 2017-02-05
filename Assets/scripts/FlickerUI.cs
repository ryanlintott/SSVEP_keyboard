using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlickerUI : MonoBehaviour {
    public Color c1;
    public Color c2;
    private Image image;
    public float cycleHz; // Hz, the mesurement of cycles.

    float dtime = 0; // delta time

    void Awake() {
        image = gameObject.GetComponent<Image>();
        Application.targetFrameRate = 60;
    }
    // Update is called once per frame
    void Update() {

    }

    public void MakeFlicker() {
        // update frequency time-step
        dtime += Time.deltaTime;

        // Sample the waveform at a specific time.
        float wave = Mathf.Sin( (dtime * 2.0f * Mathf.PI) * cycleHz);

        //print (dtime);
        //print (wave);

        // Cycle between sprites based on the waveform.
        if (wave > 0.0f) {
            image.color = c1;
            //print ("Black");
        } else {
            image.color = c2;
            //print ("White");
        }

        // prevents dtime from climbing to infinity,
        // by stepping it back in the waveform to a point
        // of equal value.
        if (wave == 0.0f) {
            dtime = 0.0f;
        }
    }
}
