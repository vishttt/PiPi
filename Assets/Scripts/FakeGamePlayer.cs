using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class FakeGamePlayer : Photon.MonoBehaviour
{
	//[SyncVar (hook = "OnChangeScore")]
	public int playerScore;



	// Use this for initialization
	void Start ()
	{
		gameObject.name = "FakeGamePlayer";
		GameEngine._instance.player1 = gameObject;
		Debug.Log ("Start" + gameObject.name);
	}


	public void AddScore (int score)
	{
		this.playerScore = score;
		OnChangeScore (score);
	}


	[PunRPC]
	public void OnChangeScore (int _playerScore)
	{
		ManuManager._instance.mPlayerResult.text = GameEngine._instance.NumofCorrectAnswers + "/10";
	}

}
