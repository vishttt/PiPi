using UnityEngine;
using System.Collections;
using UnityEngine.UI;



public class FixQuestionText : MonoBehaviour
{
	private Text QuestionText;
	private string InputText = "من اين ينبع نهر النيل ،لماذا يا هظا نحن الملوك وانتم الضعفام وايري باللي فام شي من شي يا جدعان وذا هو العبي يا اخواني العرد ؟";
	float TextWidth;
	// Use this for initialization
	void Start ()
	{
		QuestionText = gameObject.GetComponent<Text> ();

		Debug.Log (QuestionText.text);
		QuestionText.text = InputText;
	}

	// Update is called once per frame
	void Update ()
	{

	}

	void OnGUI ()
	{
	}
}
