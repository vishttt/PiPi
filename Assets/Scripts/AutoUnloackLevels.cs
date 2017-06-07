using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoUnloackLevels : MonoBehaviour
{

	// Use this for initialization
	void OnEnable ()
	{
		for (int i = 1; i <= ManuManager._instance.LevelsLocks.Count; i++) {
			if (GameEngine._instance._playerData.LevelsData [i].Active == "false" &&
			    GameEngine._instance._playerData.TotalCorrectAnswers >= int.Parse (ManuManager._instance.LevelsLocks [i - 1].transform.GetChild (0).gameObject.name.Split ('_') [1])) {
				StartCoroutine ("CoUnlockLevel", i);
			}
		}

	}

	public IEnumerator CoUnlockLevel (int i)
	{
		ManuManager._instance.levelsObjects [GameEngine._instance.SelectedLevel].GetComponent<Animator> ().SetTrigger ("NextHide");
		yield return new WaitForSeconds (ManuManager._instance.levelsObjects [GameEngine._instance.SelectedLevel].GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).length);
		ManuManager._instance.levelsObjects [GameEngine._instance.SelectedLevel].SetActive (false);
		GameEngine._instance.SelectedLevel = i;

		ManuManager._instance.levelsObjects [GameEngine._instance.SelectedLevel].SetActive (true);
		ManuManager._instance.levelsObjects [GameEngine._instance.SelectedLevel].GetComponent<Animator> ().SetTrigger ("Empty");
		ManuManager._instance.LevelsLocks [i - 1].GetComponent<Animator> ().SetTrigger ("Hide");

		yield return new WaitForSeconds (1);

		foreach (Button btn in ManuManager._instance.levelsObjects[i].GetComponentsInChildren<Button>()) {
			btn.gameObject.SetActive (true);	
		}

		ManuManager._instance.LevelsLocks [i - 1].SetActive (false);
		Debug.Log ("Auto Unlock Level " + i);

		if (GameEngine._instance.SelectedLevel == 0) {
			ManuManager._instance.backLevelBtn.interactable = false;
		} else {
			ManuManager._instance.backLevelBtn.interactable = true;
		}

		if (GameEngine._instance.SelectedLevel == ManuManager._instance.levelsObjects.Count - 1) {
			ManuManager._instance.nextLevelBtn.interactable = false;
		} else {
			ManuManager._instance.nextLevelBtn.interactable = true;
		}

		GameEngine._instance._playerData.LevelsData [i].Active = "true";
		GameEngine._instance._playerData.SaveData (GameEngine._instance._playerData);
	}
}