using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using RTLService;

public class ResultFeild : MonoBehaviour
{

	// Use this for initialization
	public Text Prank;
	public Text Pname;
	public Text Pscore;
	public Image PPic;
	private string PFBID;

	public Text Frank;
	public Text Fname;
	public Text Fscore;
	public Image FPic;
	private string FFBID;

	public Text ResultText;
	public Image ResultBG;
	public Image PResultBG;
	public Text PResultText;
	public Image FResultBG;
	public Text FResultText;


	public void PSetName (string _name)
	{
		Pname.text = RTL.Convert (_name, RTL.NumberFormat.Arabic, false);
	}

	public void PSetRank (string _rank)
	{
		Prank.text = _rank;
	}

	public void PSetScore (string _score)
	{
		Pscore.text = _score;
	}

	public void PSetFBID (string _FBID)
	{
		this.PFBID = _FBID;
	}

	public void FSetName (string _name)
	{
		Fname.text = RTL.Convert (_name, RTL.NumberFormat.Arabic, false);
	}

	public void FSetRank (string _rank)
	{
		Frank.text = _rank;
	}

	public void FSetScore (string _score)
	{
		Fscore.text = _score;
	}

	public void FSetFBID (string _FBID)
	{
		this.FFBID = _FBID;
	}

	public void FSetResultNum (int _resulNum)
	{
		FResultText.text = _resulNum.ToString ();
	}

	public void SetResult (int playerResult, int FriendResult)
	{
		PResultText.text = playerResult.ToString ();
		FResultText.text = FriendResult.ToString ();

		if (playerResult == FriendResult) {
			//Draw
			ResultText.text = RTL.Convert ("تعادل");
			ResultBG.color = Color.green;
			PResultBG.color = Color.green;
			FResultBG.color = Color.green;

		} else if (playerResult < FriendResult) {
			//Lose
			ResultText.text = RTL.Convert ("خاسر");
			ResultBG.color = Color.red;
			PResultBG.color = Color.red;
			FResultBG.color = Color.green;

		} else {
			//Won
			ResultText.text = RTL.Convert ("فائز");
			ResultBG.color = Color.green;
			PResultBG.color = Color.green;
			FResultBG.color = Color.red;
		}
	}
}
