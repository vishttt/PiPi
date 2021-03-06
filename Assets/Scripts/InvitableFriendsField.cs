﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class InvitableFriendsField : MonoBehaviour
{

	// Use this for initialization
	public Text name;
	public Button inviteBtn;
	public Image Pic;
	private string FBID;

	public void SetFBID (string _FBID)
	{
		this.FBID = _FBID;
	}

	public void SetName (string _name)
	{
		name.text = _name;
	}

	public void SetBtn (bool _active)
	{
		inviteBtn.interactable = _active;
	}

	public void InviteBtn ()
	{
		FBRequest.RequestChallenge (FBID);
		inviteBtn.interactable = false;
	}
}