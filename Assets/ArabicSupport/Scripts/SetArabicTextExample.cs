using UnityEngine;
using System.Collections;
using ArabicSupport;
using UnityEngine.UI;

public class SetArabicTextExample : MonoBehaviour
{
	
	public string text;
	
	// Use this for initialization
	void Start ()
	{	
		gameObject.GetComponent<Text> ().text = ArabicFixer.Fix (text, false, false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}
