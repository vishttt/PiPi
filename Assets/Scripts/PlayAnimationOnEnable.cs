using UnityEngine;
using System.Collections;

public class PlayAnimationOnEnable : MonoBehaviour
{

	void OnEnable ()
	{
		gameObject.GetComponent<Animator> ().SetTrigger ("Show");		
	}
}