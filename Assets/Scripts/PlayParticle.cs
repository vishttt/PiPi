using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayParticle : MonoBehaviour
{

	public ParticleSystem particle;

	public void StartPlayParticle ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.particle);
		particle.Play ();
		ManuManager._instance.WinScreenTotalScore.text = GameEngine._instance.CalculateTotalQuestions ().ToString ();

		foreach (Text score in ManuManager._instance.PlayerTotalScore) {
			score.text = BackEnd.instance.currentPlayer.TotalScore;
		}

		ManuManager._instance.ResultNumberAnim.gameObject.SetActive (false);
	}
}
