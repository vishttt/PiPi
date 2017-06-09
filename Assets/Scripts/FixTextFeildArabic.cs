using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class FixTextFeildArabic : MonoBehaviour
{

	public Text text;
	private InputField inputField;
	private string fieldString;
	// Use this for initialization
	void Start ()
	{
		inputField = gameObject.GetComponent<InputField> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (fieldString != inputField.text) {
			text.text =inputField.text;
			fieldString = inputField.text;
		}
	}
}
