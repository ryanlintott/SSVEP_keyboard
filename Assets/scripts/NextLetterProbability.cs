using UnityEngine;

public class NextLetterProbability : MonoBehaviour {

	[SerializeField] private TextAsset nextLetterFile;

	private float[,] letterProbability = new float[27,27];
	private string[] rawTextLines;
	private int aInt;
	private int bInt;
	private bool nonLetter;
	private int numNonLetters = 4;  //Number of non-letter characters on the keyboard

	void Awake () {
		//Debug.Log("Test Parse: "+float.Parse("0.02000139453"));
		rawTextLines = nextLetterFile.text.Split('\n');
		for (int i = 0; i < 27; i++) {
			for (int j = 0; j < 27; j++) {
				letterProbability[i,j] = float.Parse(rawTextLines[(i*27)+j]);
			}
		}
		//Debug.Log("Probability: "+letterProbability[0,1].ToString());
	}

	public float GetProbability(string a, string b) {
		nonLetter = false;
		aInt = System.Convert.ToInt32(a.ToLower()[0])-96;
		bInt = System.Convert.ToInt32(b.ToLower()[0])-96;
		//Debug.Log("a: "+a.ToLower()+" aInt: "+aInt.ToString()+" b: "+b.ToLower()+" bInt: "+bInt.ToString());
		if (aInt < 0 || aInt > 27) {
			aInt = 0;
		}
		if (bInt < 0 || bInt > 27) {
			bInt = 0;
			nonLetter = true;
		}
		if (nonLetter) {
			return letterProbability[aInt,bInt]/numNonLetters;
		}
		return letterProbability[aInt,bInt];
	}
}
