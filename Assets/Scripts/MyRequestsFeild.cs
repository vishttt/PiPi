using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System;

public class MyRequestsFeild : MonoBehaviour
{

	// Use this for initialization
	public Text rank;
	public Text name;
	public Text score;
	public Text DateText;
	public Image Pic;
	public DateTime RemainingTime;
	private string FBID;
	int result;
	int hours;
	int min;
	int sec;
	float totalSec;

	public void SetFBID (string _FBID)
	{
		this.FBID = _FBID;
	}

	public void SetName (string _name)
	{
		name.text = _name;
	}

	public void SetRank (string _rank)
	{
		rank.text = _rank;
	}

	public void SetScore (string _score)
	{
		score.text = _score;
	}

	public void SetTime (DateTime _date)
	{
		RemainingTime = _date;
		Debug.Log (RemainingTime.ToString ());
		hours = RemainingTime.Hour;
		min = RemainingTime.Minute;
		sec = RemainingTime.Second;
		totalSec = sec + (min * 60) + (hours * 60 * 60);
	}

	void Update ()
	{
		if (totalSec > 0) {
			totalSec -= Time.deltaTime;
			DateText.text = Math.Round (totalSec / 60).ToString () + ":" + Math.Round (totalSec % 60).ToString ();
		} else if (totalSec <= 0) {
			DateText.text = "00:00";
		}
//		result = DateTime.Compare (ExpiryDate, System.DateTime.Now);
//
//		if (result > 0) {
//			//Debug.Log (ExpiryDate - System.DateTime.Now);
//			string t = (ExpiryDate - System.DateTime.Now).Minutes.ToString () + ":" + (ExpiryDate - System.DateTime.Now).Seconds.ToString ();
//			DateText.text = t.ToString ();
//		} else {
//			DateText.text = "00:00";
//		}
	}

	//	public IEnumerator CoStartTimer ()
	//	{
	//
	//	}
}
