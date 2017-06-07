using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MultiplayersData : Photon.MonoBehaviour
{

	public static MultiplayersData _instance;

	public string sessionNum;


	public void Awake ()
	{
		_instance = this;
	}

	public void GetQuestions ()
	{
		Debug.Log ("MultiplayersData Created");

		if (sessionNum != null && sessionNum != "") {
			BackEnd.instance.GetListOfMBQuestions ((10).ToString (), sessionNum);
		}
	}

	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			// We own this player: send the others our data
			stream.SendNext (sessionNum);
		} else {
			// Network player, receive data
			this.sessionNum = (string)stream.ReceiveNext ();
		}
	}
}
