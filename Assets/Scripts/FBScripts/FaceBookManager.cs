using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;


public class FaceBookManager : MonoBehaviour
{
	public static FaceBookManager _instance;

	private Texture2D FacebookResponseTexture { get; set; }

	private string FacebookResponse;
	// Use this for initialization
	// Awake function from Unity's MonoBehavior
	void Awake ()
	{
		_instance = this;
		DontDestroyOnLoad (gameObject);

		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init (OnInitComplete);

		} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp ();
		}
	}

	void OnInitComplete ()
	{
		FBAppEvents.LaunchEvent ();

		if (FB.IsLoggedIn) {
			Debug.Log ("Already logged in &&&&&&&&&&&&&&&&&&&&&&&&&&&&");
			//OnLoginComplete ();
		}
	}

	private void OnLoginComplete ()
	{
		Debug.Log ("OnLoginComplete");

		if (!FB.IsLoggedIn) {
			// Reenable the Login Button
			//LoginButton.interactable = true;
			return;
		}

		// Show loading animations
		//LoadingText.SetActive (true);

		// Begin querying the Graph API for Facebook data
		//FBGraph.GetPlayerInfo ();
		//FBGraph.GetFriends ();
	}

	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}


	public void FacebookLogin ()
	{
		ManuManager._instance.SwitchLayouts (LayoutStates._loading.ToString ());
		FB.LogInWithReadPermissions (new List<string> () { "public_profile", "email", "user_friends" }, this.AuthCallback);
	}

	private void AuthCallback (ILoginResult result)
	{
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log (aToken.UserId);
			FB.API ("me/picture?type=square&height=200&width=200", HttpMethod.GET, this.ProfilePhotoCallback);
			//ManuManager._instance.InputprofilePic.mainTexture = 
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions) {
				Debug.Log (perm);
			}
		} else {
			ManuManager._instance.OpenLayout (LayoutStates._login.ToString ());
			Debug.Log ("User cancelled login");
		}
	}

	protected void HandleResult (IResult result)
	{
		if (result == null) {
			this.FacebookResponse = "Null Response\n";
			Debug.Log (this.FacebookResponse);
			return;
		}

		this.FacebookResponseTexture = null;

		// Some platforms return the empty string instead of null.
		if (!string.IsNullOrEmpty (result.Error)) {
			this.FacebookResponse = "Error Response:\n" + result.Error;
		} else if (result.Cancelled) {
			this.FacebookResponse = "Cancelled Response:\n" + result.RawResult;
		} else if (!string.IsNullOrEmpty (result.RawResult)) {
			this.FacebookResponse = "Success Response:\n" + result.RawResult;
		} else {
			this.FacebookResponse = "Empty Response\n";
		}
	}

	private void ProfilePhotoCallback (IGraphResult result)
	{
		if (string.IsNullOrEmpty (result.Error) && result.Texture != null) {
			ManuManager._instance.InputprofilePic.texture = result.Texture;
			FB.API ("/me?fields=name,first_name,last_name,email", HttpMethod.GET, FbGetUserData);

		} else {
			ManuManager._instance.OpenLayout (LayoutStates._login.ToString ());
			Debug.Log ("Error to to get image");
		}
		this.HandleResult (result);
	}

	private void FbGetUserData (IGraphResult result)
	{
		if (string.IsNullOrEmpty (result.Error) && result.Texture != null) {
			BackEnd.instance.Login (result.ResultDictionary ["name"].ToString (), result.ResultDictionary ["id"].ToString (), result.ResultDictionary ["id"].ToString ());
			//FBGraph.GetFriends ();
		} else {
			ManuManager._instance.OpenLayout (LayoutStates._login.ToString ());
			Debug.Log ("Error to get Player Data");
		}
	}

	public void FbShareImage ()
	{
		if (!FB.IsLoggedIn) {
			FB.LogInWithReadPermissions (new List<string> () { "public_profile", "email", "user_friends" }, this.LoginBeforeShareCallback);
		} else {

			byte[] dataToSave = GameEngine._instance._screenShotTexture.EncodeToPNG ();
			Debug.Log (dataToSave + "**");
			var wwwForm = new WWWForm ();
			wwwForm.AddBinaryData ("image", dataToSave, "ScreenShotImg.png");
			FB.API ("me/photos", HttpMethod.POST, ShareImgCallback, wwwForm);
		}
	}

	private void ShareImgCallback (IGraphResult result)
	{
		if (string.IsNullOrEmpty (result.Error) && result.Texture != null) {
			Debug.Log ("Share image Done");
		} else {
			Debug.Log ("Error to Share image");
		}
	}

	private void LoginBeforeShareCallback (ILoginResult result)
	{
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log (aToken.UserId);

			byte[] dataToSave = GameEngine._instance._screenShotTexture.EncodeToPNG ();

			var wwwForm = new WWWForm ();
			wwwForm.AddBinaryData ("image", dataToSave, "ScreenShotImg.png");
			FB.API ("me/photos", HttpMethod.POST, ShareImgCallback, wwwForm);

			//ManuManager._instance.InputprofilePic.mainTexture = 
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions) {
				Debug.Log (perm);
			}
		} else {
			ManuManager._instance.OpenLayout (LayoutStates._login.ToString ());
			Debug.Log ("User cancelled login");
		}
	}
		
}