using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class NetworkPlayer : Photon.MonoBehaviour
{
	//[SyncVar (hook = "OnChangeScore")]
	public int playerScore;

	//[SyncVar]
	public string playerName;

	//[SyncVar]
	public string playerRank;

	//[SyncVar]
	public string playerId;

	//[SyncVar]
	public string deviceId;

	//[SyncVar]
	public string totalScore;


	// Use this for initialization
	void Start ()
	{
		if (photonView.isMine) {
			gameObject.name = "Player1";
			GameEngine._instance.player1 = gameObject;
			SetPlayerData (BackEnd.instance.currentPlayer.PlayerName, BackEnd.instance.currentPlayer.UserRank, BackEnd.instance.currentPlayer.PlayerID, BackEnd.instance.deviceId, BackEnd.instance.currentPlayer.TotalScore);
		}

		if (!photonView.isMine) {
			gameObject.name = "Player2";
			photonView.RPC ("StartGame", PhotonTargets.All, null);
			photonView.RPC ("CloseRoom", PhotonTargets.MasterClient, null);
		}

		Debug.Log ("Start" + gameObject.name);
	}

	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			// We own this player: send the others our data
			stream.SendNext (playerScore);
			stream.SendNext (playerName);
			stream.SendNext (playerRank);
			stream.SendNext (playerId);
			stream.SendNext (deviceId);
			stream.SendNext (totalScore);
		} else {
			// Network player, receive data
			this.playerScore = (int)stream.ReceiveNext ();
			this.playerName = (string)stream.ReceiveNext ();
			this.playerRank = (string)stream.ReceiveNext ();
			this.playerId = (string)stream.ReceiveNext ();
			this.deviceId = (string)stream.ReceiveNext ();
			this.totalScore = (string)stream.ReceiveNext ();

		}
	}

	public void SetPlayerData (string _playername, string _playrRank, string _playerId, string _deviceId, string _totalScore)
	{
		this.playerName = _playername;
		this.playerRank = _playrRank;
		this.playerId = _playerId;
		this.deviceId = _deviceId;
		this.totalScore = _totalScore;
	}

	[PunRPC]
	public void StartGame ()
	{
		if (photonView.isMine) {
			BackEnd.instance.GetListOfMBQuestions ((10).ToString (), MultiplayersData._instance.sessionNum);

			GameEngine._instance.isGameStart = true;
			Debug.Log ("Start Game ");
			StartCoroutine (CoPlayersReady ());

			if (GameObject.Find ("Player2") != null)
				GameObject.Find ("Player2").GetComponent<PhotonView> ().RPC ("setTheFriendData", PhotonTargets.All, null);
			
			GameObject.Find ("Player1").GetComponent<PhotonView> ().RPC ("setTheFriendData", PhotonTargets.All, null);
			
		}
	}

	[PunRPC]
	public void CloseRoom ()
	{
		PhotonNetwork.room.open = false;
	}

	public void AddScore (int score)
	{
		this.playerScore = score;
		photonView.RPC ("OnChangeScore", PhotonTargets.All, score);
	}

	[PunRPC]
	public void setTheFriendData ()
	{
		if (!photonView.isMine) {
			Debug.Log (playerName + " " + deviceId + " ++++++++++++++++++++++++++++++++++++++");

			foreach (Text text in ManuManager._instance.friendName) {
				text.text = playerName;
			}

			foreach (Text text in ManuManager._instance.friendScore) {
				text.text = totalScore.ToString ();
			}

			foreach (Text rank in ManuManager._instance.friendRank) {
				rank.text = playerRank.ToString ();
			}

			BackEnd.instance.GetFriendPic (playerId, deviceId);
		}
	}

	[PunRPC]
	public void OnChangeScore (int _playerScore)
	{
		if (!photonView.isMine) {
			playerScore = _playerScore;
			ManuManager._instance.mFriendResult.text = playerScore + "/10";
		} else {
			ManuManager._instance.mPlayerResult.text = GameEngine._instance.NumofCorrectAnswers + "/10";
		}
	}


	public IEnumerator CoPlayersReady ()
	{
		yield return new WaitForSeconds (2);
		ManuManager._instance.OpenLayout (LayoutStates._MultiPlayerLobbyPanel.ToString ());

	}

}
