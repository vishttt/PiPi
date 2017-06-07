using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using RTLService;
using System;

public class FriendsRequestsFeild : MonoBehaviour
{
	// Use this for initialization

	public Text rank;
	public Text name;
	public Text score;
	public Button challangeBtn;
	public Image Pic;
	private string FBID;
	private string fbSessionID;
	public DateTime RemainingTime;
	int result;
	int hours;
	int min;
	int sec;
	float totalSec;

	public void SetFBSessionID (string _fbSessionID)
	{
		this.fbSessionID = _fbSessionID;
	}

	public void SetFBID (string _FBID)
	{
		this.FBID = _FBID;
	}

	public void SetName (string _name)
	{
		name.text = RTL.Convert (_name, RTL.NumberFormat.Arabic, false);
	}

	public void SetRank (string _rank)
	{
		rank.text = _rank;
	}

	public void SetScore (string _score)
	{
		score.text = _score;
	}

	public void SetBtn (bool _active)
	{
		challangeBtn.interactable = _active;
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

	public void ChallangeBtn ()
	{
		GameEngine._instance.isMultiPlayer = true;
		GameEngine._instance.isFBMultiPlayer = true;
		GameEngine._instance.NumofCorrectAnswers = 0;
		GameEngine._instance.currentQuestionNum = 0;

		BackEnd.instance.GetFBQuestions (10, BackEnd.instance.currentPlayer.FbId, this.FBID, this.fbSessionID);
		challangeBtn.interactable = false;
		GameObject obj = Instantiate (GameNetworkManager._instance.player);
		GameEngine._instance.player1 = obj;

		foreach (Text text in ManuManager._instance.friendName) {
			string s = "الله";
			string s2 = "الّله";
			text.text = name.text;
		}

		foreach (Text text in ManuManager._instance.friendScore) {
			text.text = score.text;
		}

		foreach (Text text in ManuManager._instance.friendRank) {
			text.text = rank.text;
		}

		foreach (RawImage image in ManuManager._instance.friendPic) {
			//if (www.text != "")
			image.texture = Pic.sprite.texture;
		}

		StartCoroutine (CoWaitforLoading ());
	}

	IEnumerator CoWaitforLoading ()
	{
		yield return new WaitForSeconds (0.5f);
		ManuManager._instance.OpenLayout (LayoutStates._loading.ToString ());
	}

	void Update ()
	{
		if (totalSec > 0) {
			totalSec -= Time.deltaTime;
			//DateText.text = Math.Round (totalSec / 60).ToString () + ":" + Math.Round (totalSec % 60).ToString ();
		} else if (totalSec <= 0) {
			//DateText.text = "00:00";
			challangeBtn.interactable = false;
		}
	}
}
