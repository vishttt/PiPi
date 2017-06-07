using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class DataSpawner : NetworkBehaviour
{
	public GameObject gameData;

	public override void OnStartServer ()
	{
		var data = (GameObject)Instantiate (gameData);
		NetworkServer.Spawn (data);
	}
}
