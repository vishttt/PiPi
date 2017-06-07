using UnityEngine;
using System.Collections;
using ArabicSupport;
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
		//TextWidth = QuestionText.rectTransform.rect.width;
		//Debug.Log (QuestionText.text.Split ('\n') [0]);
		//QuestionText.text = ArabicFixer.Fix (QuestionText.text, false, false);
		Debug.Log (QuestionText.text);
		QuestionText.text = InputText;
	}

	// Update is called once per frame
	void Update ()
	{
//		if (QuestionText != QuestionTextString) {
//			text.text = ArabicFixer.Fix (QuestionText.text, false, false);
//			fieldString = inputField.text;
//		}
	}

	void OnGUI ()
	{
		//	GUI.Label (QuestionText.rectTransform.rect, QuestionText.text, null);
	}
}
