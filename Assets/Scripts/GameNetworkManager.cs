using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;

public class GameNetworkManager : PunBehaviour
{
	public static GameNetworkManager _instance;

	public GameObject player;
	private PhotonView myPhotonView;
	private static PhotonView ScenePhotonView;

	// Use this for initialization
	void Start ()
	{
		_instance = this;
		//PhotonNetwork.ConnectUsingSettings ("0.1");
		PhotonNetwork.logLevel = PhotonLogLevel.Full;
	}


	public void JoinFirstFoundMatch ()
	{	
		ManuManager._instance.OpenLayout (LayoutStates._MultiplayerLoading.ToString ());

		if (PhotonNetwork.connected) {
			if (!PhotonNetwork.insideLobby) {
				PhotonNetwork.JoinLobby ();
			} else {
				PhotonNetwork.JoinRandomRoom ();
			}
		} else {
			PhotonNetwork.ConnectUsingSettings ("0.1");
		}
	}

	public override void OnConnectedToMaster ()
	{
		base.OnConnectedToMaster ();

		if (!PhotonNetwork.insideLobby) {
			PhotonNetwork.JoinLobby ();
		} else {
			PhotonNetwork.JoinRandomRoom ();
		}
	}

	public override void OnJoinedLobby ()
	{
		PhotonNetwork.JoinRandomRoom ();
	}

	public override void OnJoinedRoom ()
	{
		GameObject player = PhotonNetwork.Instantiate ("Player", Vector3.zero, Quaternion.identity, 0);
		//monster.GetComponent<myThirdPersonController> ().isControllable = true;
		myPhotonView = player.GetComponent<PhotonView> ();
		//StartCoroutine ("CoWaitForUsers");
	}

	public void LeaveRoom ()
	{
		PhotonNetwork.LeaveRoom ();
	}

	void OnPhotonRandomJoinFailed ()
	{
		Debug.Log ("Can't join random room!");
		BackEnd.instance.GetMBSessionNum ((10).ToString ());

	}

	//	void OnCreatedRoom ()
	//	{
	//		PhotonNetwork.Instantiate ("MultiplayersData", Vector3.zero, Quaternion.identity, 0);
	//	}

	public void GetFakeUser ()
	{
		StartCoroutine (CoWaitForUsers ());
	}


	public void OnConnectionFail ()
	{
		Debug.Log ("ConnectionFail");
		GameEngine._instance.StopTheGame ();
		ManuManager._instance.ConnictionErrorPanel.SetActive (true);

	}

	public IEnumerator CoWaitForUsers ()
	{
		yield return new WaitForSeconds (12f);
		//TO Start Face Game Play
		if (!GameEngine._instance.isGameStart) {
			PhotonNetwork.room.open = false;
			PhotonNetwork.Disconnect ();
			BackEnd.instance.GetFakeUser ();
			GameObject obj = Instantiate (player);
			GameEngine._instance.player1 = obj;

			//TODO Close the Room
		}
	}

	void OnGUI ()
	{
		//GUILayout.Label (PhotonNetwork.connectionStateDetailed.ToString ());
	}



	void OnApplicationQuit ()
	{
		
	}
}
