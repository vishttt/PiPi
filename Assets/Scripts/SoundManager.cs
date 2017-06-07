using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{

	public AudioSource btn;
	public AudioSource animAppear;
	public AudioSource animDisapear;
	public AudioSource animAppearEnd;
	public AudioSource correctAnswer;
	public AudioSource wrongAnswer;
	public AudioSource BtnMove;
	public AudioSource homeAppear;
	public AudioSource homeDisappear;
	public AudioSource levelsAppear;
	public AudioSource levelsDisappear;
	public AudioSource questionsAppear;
	public AudioSource particle;
	public AudioSource tie;
	public AudioSource win;
	public AudioSource lose;

	public bool mute = true;

	public static SoundManager _instance;
	// Use this for initialization
	void Awake ()
	{
		_instance = this;

		if (PlayerPrefs.GetString ("sound") == "") {
			PlayerPrefs.SetString ("sound", "on");
		} 
	}

	void Start ()
	{
		if (PlayerPrefs.GetString ("sound") == "off") {
			ManuManager._instance.soundBtn.color = new Color (1, 1, 1, 0.4f);
			PlayerPrefs.SetString ("sound", "off");
			mute = true;
		} else {
			ManuManager._instance.soundBtn.color = new Color (1, 1, 1, 1);
			PlayerPrefs.SetString ("sound", "on");
			mute = false;
		}
	}

	public void PlaySound (AudioSource audioSource)
	{
		if (!mute) {
			audioSource.Play ();
		}
	}

	public void PlayAppearLayoutSound (string layout)
	{
		if (!mute) {
			switch (layout) {
			case "_selectGame":
				PlaySound (homeAppear);
				break;
			case "_selectCategory":
				PlaySound (levelsAppear);
				break;
			case  "_MultiPlayerLobbyPanel":
				PlaySound (homeAppear);
				break;
			}
		}
	}

	public void PlayDisAppearLayoutSound (string layout)
	{
		if (!mute) {
			switch (layout) {
			case "_selectGame":
				PlaySound (homeDisappear);
				break;
			case "_selectCategory":
				PlaySound (levelsDisappear);
				break;
			case  "_MultiPlayerLobbyPanel":
				PlaySound (homeDisappear);
				break;
			}
		}
	}
}