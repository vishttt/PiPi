using UnityEngine;
using System.Collections;

public class OnEnableLayout : MonoBehaviour
{
	public AudioSource sound;

	void OnEnable ()
	{
		SoundManager._instance.PlaySound (sound);
	}
}
