/**
 * Copyright (c) 2014-present, Facebook, Inc. All rights reserved.
 *
 * You are hereby granted a non-exclusive, worldwide, royalty-free license to use,
 * copy, modify, and distribute this software in source code or binary form for use
 * in connection with the web services and APIs provided by Facebook.
 *
 * As with any software that integrates with the Facebook platform, your use of
 * this software is subject to the Facebook Developer Principles and Policies
 * [http://developers.facebook.com/policy/]. This copyright notice shall be
 * included in all copies or substantial portions of the software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using UnityEngine;
using System;
using System.Collections.Generic;
using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// Class responsible for Facebook Graph API calls in Friend Smash!
//
// The Facebook Graph API allows us to fetch information about the player and their friends for the permissions
// they grant with Facebook Login.
// We can use this data to provide a set of real people to play against, showing names and pictures
// of the player's friends to make the game experience feel even more personal.
//
// For more details on the Graph API see: https://developers.facebook.com/docs/graph-api/overview
// See https://developers.facebook.com/docs/unity/reference/current/FB.API for Unity specific details
public static class FBGraph
{
	#region PlayerInfo

	// Once a player successfully logs in, we can welcome them by showing their name
	// and profile picture on the home screen of the game. This information is returned
	// via the /me/ endpoint for the current player. We'll call this endpoint via the
	// SDK and use the results to personalize the home screen.
	//
	// Make a Graph API GET call to /me/ to retrieve a player's information
	// See: https://developers.facebook.com/docs/graph-api/reference/user/
	public static void GetPlayerInfo ()
	{
		string queryString = "/me?fields=id,first_name,picture.width(120).height(120)";
		FB.API (queryString, HttpMethod.GET, GetPlayerInfoCallback);
	}

	private static void GetPlayerInfoCallback (IGraphResult result)
	{
		Debug.Log ("GetPlayerInfoCallback");
		if (result.Error != null) {
			Debug.LogError (result.Error);
			return;
		}
		Debug.Log (result.RawResult);

		// Save player name
		string name;
		if (result.ResultDictionary.TryGetValue ("first_name", out name)) {
			//	GameStateManager.Username = name;
		}

		//Fetch player profile picture from the URL returned
		//string playerImgUrl = GraphUtil.DeserializePictureURL (result.ResultDictionary);
		//GraphUtil.LoadImgFromURL (playerImgUrl, delegate(Texture pictureTexture) {
		// Setup the User's profile picture
		//if (pictureTexture != null) {
		//GameStateManager.UserTexture = pictureTexture;
		//}

		// Redraw the UI
		//GameStateManager.CallUIRedraw ();
		//});

	}

	// In the above request it takes two network calls to fetch the player's profile picture.
	// If we ONLY needed the player's profile picture, we can accomplish this in one call with the /me/picture endpoint.
	//
	// Make a Graph API GET call to /me/picture to retrieve a players profile picture in one call
	// See: https://developers.facebook.com/docs/graph-api/reference/user/picture/
	public static void GetPlayerPicture ()
	{
		FB.API (GraphUtil.GetPictureQuery ("me", 128, 128), HttpMethod.GET, delegate(IGraphResult result) {
			Debug.Log ("PlayerPictureCallback");
			if (result.Error != null) {
				Debug.LogError (result.Error);
				return;
			}
			if (result.Texture == null) {
				Debug.Log ("PlayerPictureCallback: No Texture returned");
				return;
			}
            
			// Setup the User's profile picture
			//GameStateManager.UserTexture = result.Texture;
            
			// Redraw the UI
			//GameStateManager.CallUIRedraw ();
		});
	}

	#endregion

	#region Friends

	// We can fetch information about a player's friends via the Graph API user edge /me/friends
	// This endpoint returns an array of friends who are also playing the same game.
	// See: https://developers.facebook.com/docs/graph-api/reference/user/friends
	//
	// We can use this data to provide a set of real people to play against, showing names
	// and pictures of the player's friends to make the experience feel even more personal.
	//
	// The /me/friends edge requires an additional permission, user_friends. Without
	// this permission, the response from the endpoint will be empty. If we know the user has
	// granted the user_friends permission but we see an empty list of friends returned, then
	// we know that the user has no friends currently playing the game.
	//
	// Note:
	// In this instance we are making two calls, one to fetch the player's friends who are already playing the game
	// and another to fetch invitable friends who are not yet playing the game. It can be more performant to batch
	// Graph API calls together as Facebook will parallelize independent operations and return one combined result.
	// See more: https://developers.facebook.com/docs/graph-api/making-multiple-requests
	//
	public static List<string> userFriendsIds = new List<string> ();
	//public static Dictionary<string, string> invitableFriends =
	//	new Dictionary<string, string> ();

	public static void GetFriends ()
	{
		// installed
		string queryString = "/me/friends?fields=id,installed,first_name&limit=200";//"/me/friends?fields=id,installed,first_name,picture.width(128).height(128)&limit=100";
		FB.API (queryString, HttpMethod.GET, GetFriendsCallback);
	}

	private static void GetFriendsCallback (IGraphResult result)
	{
		Debug.Log ("GetFriendsCallback");
		if (result.Error != null) {
			Debug.LogError (result.Error + ".........................");
			return;
		}
		Debug.Log (result.RawResult);


		// Store /me/friends result
		object dataList;
		if (result.ResultDictionary.TryGetValue ("data", out dataList)) {
			var friendsList = (List<object>)dataList;
			Debug.Log (friendsList.Count + "------------------------------");
			GameEngine._instance.Friends = friendsList;
			userFriendsIds.Clear ();

			for (int i = 0; i < GameEngine._instance.Friends.Count; i++) {
				
				var friend = GameEngine._instance.Friends [i] as Dictionary<string, object>;

				string name = friend ["first_name"] as string;
				string id = friend ["id"] as string;
				userFriendsIds.Add (id);
				Debug.Log (name + "  " + id);
			}
		}

		BackEnd.instance.GetFBUserData (FBGraph.userFriendsIds);
	}

	public static void GetInvitableFriends ()
	{
		string queryString = "/me/invitable_friends?fields=id,installed,first_name,picture.width(128).height(128)&limit=50";
		FB.API (queryString, HttpMethod.GET, GetInvitableFriendsCallback);
	}

	private static void GetInvitableFriendsCallback (IGraphResult result)
	{
		Debug.Log ("GetInvitableFriendsCallback");
		if (result.Error != null) {
			Debug.LogError (result.Error);
			return;
		}
		Debug.Log (result.RawResult);


		RootObject _data = new RootObject ();
		_data = JsonUtility.FromJson<RootObject> (result.RawResult);
		Debug.Log (" invitableFriendsList------------------------------ " + _data.data.Count);
		for (int i = 0; i < _data.data.Count; i++) {
			Debug.Log (" invitableFriendsList------------------------------ " + _data.data [i].first_name);
			ManuManager._instance.SetInvaitableFriendFaild (_data.data [i].id, _data.data [i].first_name, _data.data [i].picture.data.url);
		}
		//ManuManager._instance.LoadingPupUp.SetActive (false);
		//ManuManager._instance.OpenLayout (LayoutStates._FriendsListPanel.ToString ());
		// Store /me/invitable_friends result
//		object dataList;
//		if (result.ResultDictionary.TryGetValue ("data", out dataList)) {
//			var invitableFriendsList = (List<object>)dataList;
//			Debug.Log (invitableFriendsList.Count + " invitableFriendsList------------------------------ " + _data.data.Count);
//			CacheFriends (invitableFriendsList);
//		}

	

//		if (result.ResultDictionary.TryGetValue ("data", out dataList)) {
//			var invitableFriendsList = (List<object>)dataList;
//			Debug.Log (invitableFriendsList.Count + " invitableFriendsList------------------------------");
//			CacheFriends (invitableFriendsList);
//		}
	}

	//	private static void CacheFriends (List<object> newFriends)
	//	{
	//		GameEngine._instance.Friends = newFriends;
	//
	//		for (int i = 0; i < GameEngine._instance.Friends.Count; i++) {
	//
	//			var friends = GameEngine._instance.Friends [i] as Dictionary<string, object>;
	//
	//			//	var invitableFriendsPics = (List<object>)friends ["picture"];
	//			//	var picture = friends ["picture"] as Dictionary<string, object>;
	//
	//			string name = friends ["first_name"] as string;
	//			string id = friends ["id"] as string;
	//			//Debug.Log (picture ["url"] as string);
	//			//invitableFriends.Add (id, name);
	//			ManuManager._instance.SetInvaitableFriendFaild (id, name);
	//			//string is_app_user = friend ["is_app_user"] as string;
	//			Debug.Log (name + "  " + id + "  ");
	//		}
	//
	//	}

	#endregion

	// Graph API call to fetch friend picture from user ID returned from FBGraph.GetScores()
	//
	// Note: /me/invitable_friends returns invite tokens instead of user ID's,
	// which will NOT work with this /{user-id}/picture Graph API call.
	private static void LoadFriendImgFromID (string userID, Action<Texture> callback)
	{
		FB.API (GraphUtil.GetPictureQuery (userID, 128, 128),
			HttpMethod.GET,
			delegate (IGraphResult result) {
				if (result.Error != null) {
					Debug.LogError (result.Error + ": for friend " + userID);
					return;
				}
				if (result.Texture == null) {
					Debug.Log ("LoadFriendImg: No Texture returned");
					return;
				}
				callback (result.Texture);
			});
	}

	public static void GetFBImage (string userId, Image img)
	{
		LoadFriendImgFromID (userId, pictureTexture => {
			if (pictureTexture != null) {
				img.sprite = Sprite.Create ((Texture2D)pictureTexture, new Rect (0, 0, pictureTexture.width, pictureTexture.height), new Vector2 (0.5f, 0.5f));
			}
		});
	}
		
}
