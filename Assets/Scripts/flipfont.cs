using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using ArabicSupport;

//using Syst![alt text][1]em.Collections.Generic;


public class flipfont : MonoBehaviour
{

	Text myText;
	//You can also make this public and attach your UI text here.

	string individualLine = "";
	List<string> lines = new List<string> ();

	//Control individual line in the multi-line text component.

	int numberOfAlphabetsInSingleLine = 15;

	string sampleString = "من اين ينبع نهر النيل ،لماذا يا هظا نحن الملوك وانتم الضعفام وايري باللي فام شي من شي يا جدعان وذا هو العبي يا اخواني العرد ؟";
	string newtext;

	void Awake ()
	{
		myText = GetComponent<Text> ();
		sampleString = ArabicFixer.Fix (sampleString);
		//if (sampleString.Length > 50)
		//numberOfAlphabetsInSingleLine = (sampleString.Length / 15) + numberOfAlphabetsInSingleLine;
		Debug.Log ("numberOfAlphabets = " + numberOfAlphabetsInSingleLine + "number of character strings = " + sampleString.Length);
	}

	void Start ()
	{
		List<string> listofWords = sampleString.Split (' ').ToList (); //Extract words from the sentence
		//Debug.Log (listofWords [0]);
		//lines.Add (listofWords [0]);
		foreach (string word in listofWords) {
			Debug.Log (word);
			if (individualLine.Length >= numberOfAlphabetsInSingleLine) {
				lines.Add (individualLine);
				Debug.Log ("new line");
				//numberOfAlphabetsInSingleLine += numberOfAlphabetsInSingleLine;
				individualLine = "";
			}

			individualLine += word + " ";

		}

		lines.Reverse ();
		foreach (string line in lines) {
			newtext += line + "\n";
		}

		myText.text = newtext;

	}
}