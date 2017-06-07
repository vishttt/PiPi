using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour
{
	public AudioSource sound;

	public void playSoundOnAnim ()
	{
		SoundManager._instance.PlaySound (sound);
	}
}
