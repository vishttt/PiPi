using UnityEngine;
using System.Collections;

public class LevelStartAnim : MonoBehaviour
{

	void OnEnable ()
	{
		if (ManuManager._instance.levelsObjects [GameEngine._instance.SelectedLevel].transform.FindChild ("Lock").gameObject.activeSelf) {
			ManuManager._instance.levelsObjects [GameEngine._instance.SelectedLevel].GetComponent<Animator> ().SetTrigger ("Empty");
		} else {
			ManuManager._instance.levelsObjects [GameEngine._instance.SelectedLevel].GetComponent<Animator> ().SetTrigger ("NextShow");
		}
	}
}