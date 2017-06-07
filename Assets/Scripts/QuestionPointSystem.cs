using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class QuestionPointSystem : MonoBehaviour
{

	//public static QuestionPointSystem _instance;
	public List<Image> pointsList;

	private int currentPoint;

	public int restValue;


	// Use this for initialization
	void Awake ()
	{
		//_instance = this;
		currentPoint = restValue;
	}

	//	void OnEnable ()
	//	{
	//		Rest ();
	//	}

	public void NextPoint (int num)
	{
		//currentPoint++;

		if (num > 0)
			pointsList [num - 1].rectTransform.localScale = new Vector3 (1, 1, 1);

		//if (currentPoint < 10) {
		pointsList [num].color = new Color (1, 1, 1, 1);
		pointsList [num].rectTransform.localScale = new Vector3 (1.7f, 1.7f, 1.7f);

	}

	public void Rest ()
	{
		currentPoint = restValue;
		foreach (Image point in pointsList) {
			point.color = new Color (1, 1, 1, 0.4f);
			point.rectTransform.localScale = new Vector3 (1, 1, 1);
		}

		pointsList [0].color = new Color (1, 1, 1, 1);
		pointsList [0].rectTransform.localScale = new Vector3 (1.7f, 1.7f, 1.7f);
	}
		
}
