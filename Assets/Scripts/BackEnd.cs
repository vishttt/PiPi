using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System;
using UnityEngine.UI;
using System.Text;
using RTLService;

public class BackEnd : MonoBehaviour
{

	//Here is a private reference only this class can access
	private static BackEnd _instance;
	public GameObject leaderboardContent;
	public GameObject FriendsFeild;
	public GameObject InvitableFriendsField;
	public GameObject MyRequestsFeild;
	public GameObject FriendsRequestsFeild;
	public GameObject ResultFeild;
	public GameObject EmptyListField1;
	public GameObject EmptyListField2;
	public GameObject EmptyListField3;
	public GameObject top10ContentParent;
	public GameObject yourPlaceContentParent;
	public Scrollbar top10Slider;
	public Scrollbar yourPlaceSlider;
	public List<string> pendingUsers = new List<string> ();
	public string LastFBSessionID;

	public Player currentPlayer;
	public Texture2D PlyaerPicTex;
	public Texture2D defultTex;
	public Dictionary<string, Texture2D> QImages =
		new Dictionary<string, Texture2D> ();

	//This is the public reference that other classes will use
	public static BackEnd instance {
		get {
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<BackEnd> ();
			return _instance;
		}
	}

	private string webServiceLink = WWW.UnEscapeURL ("http://myloadblancer-941018846.us-west-2.elb.amazonaws.com/ksa");
	//"http://89.108.164.28:8282/ksa";
	// "http://192.168.2.8:8282/ksa";
	private bool isLoggedIn = false;
	private int userId, totalQ, correctA, score, result, type;
	//SignIn Members
	public string deviceId;


	// Use this for initialization
	void Start ()
	{
		Application.runInBackground = true;
		//StartCoroutine ("CoGetTopTen", 3.ToString ());
		//PlayerPrefs.SetString ("_username", "");
		//GetFakeUser ();
		deviceId = SystemInfo.deviceUniqueIdentifier;

		if (PlayerPrefs.GetString ("_username") != "") {
			Debug.Log ("_username Not null");
			ManuManager._instance.OpenLayout (LayoutStates._loading.ToString ());
			StartCoroutine (CoGetUser (PlayerPrefs.GetString ("_username"), PlayerPrefs.GetString ("_deviceid"), PlayerPrefs.GetString ("_FBID")));
		} else {
			Debug.Log ("_username null");
			ManuManager._instance.OpenLayout (LayoutStates._login.ToString ());
		}

		//StartCoroutine (GetUser ("josephkadi44", deviceId));
	}

	public IEnumerator CoGetUser (string _username, string _DeviceID, string _FBID)
	{
		ManuManager._instance.OpenLayout (LayoutStates._loading.ToString ());
		//TODO Show the Loading screen
		var wwwForm = new WWWForm ();
		//ManuManager._instance.loadingLayout.SetActive (true);

		wwwForm.AddField ("p", _username + "||." + _DeviceID + "||." + _FBID);

		if (PlayerPrefs.GetString ("_username") == "") {
			//Create a texture the size of the screen, RGB24 format
			Texture2D profileTex = ManuManager._instance.InputprofilePic.mainTexture as Texture2D;
			byte[] bytes = profileTex.EncodeToPNG ();
			Debug.Log ("Uploading the Image");
			wwwForm.AddBinaryData ("img", bytes, "ProfilePic.png", "image/png");
		}

		WWW www = new WWW (webServiceLink + "/login.php", wwwForm);

		yield return www;
		if (www.error == null) {
			Debug.Log (www.text);
			var serializer = new XmlSerializer (typeof(Player), new XmlRootAttribute ("Player"));
			using (TextReader reader = new StringReader (www.text)) {
				try {
					currentPlayer = (Player)serializer.Deserialize (reader);
					Debug.Log (currentPlayer.Header);
					PlayerPrefs.SetString ("_username", currentPlayer.PlayerName);
					PlayerPrefs.SetString ("_deviceid", _DeviceID);
					PlayerPrefs.SetString ("_playerid", currentPlayer.PlayerID);
					PlayerPrefs.SetString ("_FBID", _FBID);
					Debug.Log ("_username  " + currentPlayer.PlayerName);
					isLoggedIn = true;
					//set the player name Texts
					ManuManager._instance.SetPlayerNameText ();
					reader.Close ();
					StartCoroutine (CoGetImage (currentPlayer.PlayerID));
					ManuManager._instance.OpenLayout (LayoutStates._loading.ToString ());
					//yield return new WaitForSeconds (2);
					//ManuManager._instance.loadingLayout.SetActive (false);

					GetGuestPendingMatches (true);
				} catch {
					Debug.Log ("Xml Parsing Exception caught.");
					ManuManager._instance.ConnictionErrorPanel.SetActive (true);
					//Handle the interconiction error and restart the game
				}
			}
		} else {
			//TODO close the Loading screen & Player logined
			//ManuManager._instance.OpenLayout (LayoutStates._login.ToString ());
			Debug.Log (www.error + " error");
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	public IEnumerator CoGetImage (string id)
	{
		PlyaerPicTex = new  Texture2D (256, 256, TextureFormat.DXT1, false);
		string url = webServiceLink + "/img.php?user=" + id;
		Debug.Log (url);
		WWW www = new WWW (url);
		//Debug.Log (www.url);
		yield return www;
		if (www.error == null) {
			www.LoadImageIntoTexture (PlyaerPicTex);

			Debug.Log (www.texture);
			foreach (RawImage image in ManuManager._instance.profilePics) {
				//if (www.text != "")
				image.texture = www.texture;
			}

			ManuManager._instance.profilePic.sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), new Vector2 (0.5f, 0.5f));

			Debug.Log ("Image Loaded");
			//ManuManager._instance.loadingLayout.SetActive (false);
			ManuManager._instance.OpenLayout (LayoutStates._selectGame.ToString ());
			Debug.Log ("Image Loaded2");
		} else {
			//ManuManager._instance.loadingLayout.SetActive (false);
			Debug.Log (www.error);
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	public IEnumerator CoGetFriendImage (string playerid, string FdevoceId)
	{
		WWWForm wwwForm = new WWWForm ();
		PlyaerPicTex = new  Texture2D (256, 256, TextureFormat.DXT1, false);
		wwwForm.AddField ("user", playerid);
		wwwForm.AddField ("device", FdevoceId);
		name = name.Replace (" ", "+");
		string url = webServiceLink + "/img.php?user=" + playerid;
		Debug.Log (url);
		WWW www = new WWW (url);
//		Debug.Log (www.url);
		yield return www;
		if (www.error == null) {
			www.LoadImageIntoTexture (PlyaerPicTex);
			//Debug.Log (www.texture);
			foreach (RawImage image in ManuManager._instance.friendPic) {
				//if (www.text != "")
				image.texture = www.texture;
			}
			//TODO Disable the Multiplayer Loading and wait fow sec to Start the game Scene

		} else {
			Debug.Log (www.error);
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	public IEnumerator CoGetListOfQuestions (string _questionNum, string _difficultyID, string _categoryID)
	{
		GameEngine._instance.PointsSys.GetComponent<QuestionPointSystem> ().Rest ();
		//TODO Show the Loading screen
		ManuManager._instance.levelsObjects [GameEngine._instance.SelectedLevel].GetComponent<Animator> ().SetTrigger ("NextHide");
		ManuManager._instance.OpenLayout (LayoutStates._loading.ToString ());
		var wwwForm = new WWWForm ();
		wwwForm.AddField ("p", _questionNum + "||." + _difficultyID + "||." + _categoryID);
		WWW www = new WWW (webServiceLink + "/singleq.php", wwwForm);

		yield return www;
		if (www.error == null) {
			Debug.Log (www.text);
			var serializer = new XmlSerializer (typeof(Question), new XmlRootAttribute ("Question"));

			string s = "الله";
			string s2 = "الّله";
			string text = www.text.Replace (s, s2);

			using (TextReader reader = new StringReader (text)) {
				try {
					GameEngine._instance.questionList = (Question)serializer.Deserialize (reader);
					reader.Close ();
					StartCoroutine ("CoGetQImage");
				} catch {
					Debug.Log ("Xml Parsing Exception caught.");
					ManuManager._instance.ConnictionErrorPanel.SetActive (true);
					//Handle the interconiction error and restart the game
				}
			}
		} else {
			//TODO close the Loading screen & Player logined
			Debug.Log (www.error + " error");
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	public IEnumerator CoGetListOfMBQuestions (string _questionNum, string sessionNum)
	{
		GameEngine._instance.MPointsSys.GetComponent<QuestionPointSystem> ().Rest ();
		//TODO Show the Loading screen
		var wwwForm = new WWWForm ();
		wwwForm.AddField ("p", _questionNum + "||." + sessionNum);
		WWW www = new WWW (webServiceLink + "/multiq.php", wwwForm);

		yield return www;
		if (www.error == null) {
			Debug.Log (www.text);

			string s = "الله";
			string s2 = "الّله";
			string text = www.text.Replace (s, s2);

			var serializer = new XmlSerializer (typeof(MQuestion), new XmlRootAttribute ("MQuestion"));
			using (TextReader reader = new StringReader (text)) {
				var mQuestion = (MQuestion)serializer.Deserialize (reader);
				GameEngine._instance.questionList = mQuestion.Question;
				reader.Close ();
				//StartGamePlay
				StartCoroutine ("CoGetQImage");
				yield return new WaitForSeconds (6);
				ManuManager._instance.OpenLayout (LayoutStates._MultiPlayerPanel.ToString ());
				GameEngine._instance.SetNewMQuestion ();
			}
		} else {
			//TODO close the Loading screen & Player logined
			Debug.Log (www.error + " error");
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	public IEnumerator CoGetMBSessionNum (string _questionNum)
	{
		GameEngine._instance.MPointsSys.GetComponent<QuestionPointSystem> ().Rest ();
		//TODO Show the Loading screen
		var wwwForm = new WWWForm ();
		wwwForm.AddField ("p", _questionNum + "||.");
		WWW www = new WWW (webServiceLink + "/multiq.php", wwwForm);

		yield return www;
		if (www.error == null) {
			Debug.Log (www.text);

			string s = "الله";
			string s2 = "الّله";
			string text = www.text.Replace (s, s2);

			var serializer = new XmlSerializer (typeof(MQuestion), new XmlRootAttribute ("MQuestion"));
			using (TextReader reader = new StringReader (text)) {
				try {
					var mQuestion = (MQuestion)serializer.Deserialize (reader);
					GameEngine._instance.questionList = mQuestion.Question;
					MultiplayersData._instance.sessionNum = mQuestion.Sessionid;
					reader.Close ();
					//StartGamePlay
					PhotonNetwork.CreateRoom (null);
					GameNetworkManager._instance.GetFakeUser ();
					StartCoroutine ("CoGetQImage");
				} catch {
					Debug.Log ("Xml Parsing Exception caught.");
					ManuManager._instance.ConnictionErrorPanel.SetActive (true);
					//Handle the interconiction error and restart the game
				}
			}
		} else {
			//TODO close the Loading screen & Player logined
			Debug.Log (www.error + " error");
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	public IEnumerator CoGetQImage ()
	{
		QImages.Clear ();
		Debug.Log ("Load Question image");
		for (int i = 0; i < GameEngine._instance.questionList.QuestionDesc.Count; i++) {

			Debug.Log ("loaded Image == " + GameEngine._instance.questionList.Image [i]);
			if (GameEngine._instance.questionList.Image [i] != "") {
				WWW www = new WWW (webServiceLink + "/qimg.php?p=" + GameEngine._instance.questionList.QuestionId [i]);
				yield return www;
				if (www.error == null) {
					QImages.Add (GameEngine._instance.questionList.QuestionId [i], www.texture);
					Debug.Log ("Image Loaded : " + GameEngine._instance.questionList.QuestionId [i]);

				} else {
					//TODO close the Loading screen & Player logined
					Debug.Log (www.error + " error");
					ManuManager._instance.ConnictionErrorPanel.SetActive (true);
				}
			}
		}

		if (!GameEngine._instance.isMultiPlayer) {
			ManuManager._instance.OpenLayout (LayoutStates._singlePlayer.ToString ());
			GameEngine._instance.SetNewQuestion ();
		}

		yield return null;

	}

	public IEnumerator CoGetFakeUser ()
	{
		Debug.Log ("user Rank" + currentPlayer.UserRank);
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("p", currentPlayer.UserRank);
		WWW www = new WWW (webServiceLink + "/fuser.php", wwwform);

		yield return www;
		if (www.error == null) {
			Debug.Log (www.text);
			var serializer = new XmlSerializer (typeof(Player), new XmlRootAttribute ("Player"));
			using (TextReader reader = new StringReader (www.text)) {
				try {
					var fakePlayer = (Player)serializer.Deserialize (reader);

					foreach (Text text in ManuManager._instance.friendName) {
						string s = "الله";
						string s2 = "الّله";
						text.text = RTL.Convert (fakePlayer.PlayerName.Replace (s, s2) + " ", RTL.NumberFormat.Context, false);
					}

					foreach (Text text in ManuManager._instance.friendScore) {
						text.text = fakePlayer.TotalScore.ToString ();
					}

					foreach (Text text in ManuManager._instance.friendRank) {
						text.text = fakePlayer.UserRank.ToString ();
					}
						

					ManuManager._instance.mFriendResult.text = GetFakeScore (int.Parse (fakePlayer.UserRank)).ToString () + "/10";

					if (fakePlayer.Image != null && fakePlayer.Image != "") {
						StartCoroutine ("CoGetFUserImage", fakePlayer.PlayerID);
					} else {
						StartCoroutine ("CoStartMuliplayerlobby");

						foreach (RawImage image in ManuManager._instance.friendPic) {
							//if (www.text != "")
							image.texture = defultTex;
						}
					}

					reader.Close ();
					//StartGamePlay
				} catch {
					Debug.Log ("Xml Parsing Exception caught.");
					ManuManager._instance.ConnictionErrorPanel.SetActive (true);
					//Handle the interconiction error and restart the game
				}
			}
		} else {
			//TODO close the Loading screen & Player logined
			Debug.Log (www.error + " error");
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	public IEnumerator CoGetFUserImage (string id)
	{
		PlyaerPicTex = new  Texture2D (256, 256, TextureFormat.DXT1, false);
		string url = webServiceLink + "/fimg.php?p=" + id;
		Debug.Log (url);
		WWW www = new WWW (url);
		yield return www;
		if (www.error == null) {
			www.LoadImageIntoTexture (PlyaerPicTex);
			//Debug.Log (www.texture);
			foreach (RawImage image in ManuManager._instance.friendPic) {
				//if (www.text != "")
				image.texture = www.texture;
			}
			StartCoroutine ("CoStartMuliplayerlobby");

			//TODO Disable the Multiplayer Loading and wait fow sec to Start the game Scene

		} else {
			Debug.Log (www.error);
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	public IEnumerator CoSetscore (string qList)
	{
		if (GameEngine._instance.qList.Length > 0) {
			GameEngine._instance.qList = GameEngine._instance.qList.Substring (0, GameEngine._instance.qList.Length - 3);
		}
		WWWForm wwwForm = new WWWForm ();
		wwwForm.AddField ("p", this.userId.ToString () + "||." + DateTime.Now.ToString ().Replace ('/', '_').Replace (' ', '_')
		+ "||." + this.totalQ.ToString () + "||." + this.correctA.ToString () + "||." + this.score.ToString () + "||." + this.result.ToString () + "||." + type.ToString ());

		wwwForm.AddField ("q", qList);

		string url = webServiceLink + "/setscore.php";
		Debug.Log (url + " p= " + this.userId.ToString () + "||." + DateTime.Now.ToString ().Replace ('/', '_').Replace (' ', '_')
		+ "||." + this.totalQ.ToString () + "||." + this.correctA.ToString () + "||." + this.score.ToString () + "||." + this.result.ToString () + "||." + type.ToString () + " q= " + qList);
		WWW www = new WWW (url, wwwForm);
		yield return www;
		if (www.error == null) {
			Debug.Log ("Post Score Done ______________________");
			GameEngine._instance.qList = "";
		} else {
			Debug.Log (www.error);
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	public IEnumerator CoStartMuliplayerlobby ()
	{
		yield return new WaitForSeconds (2);
		ManuManager._instance.OpenLayout (LayoutStates._MultiPlayerLobbyPanel.ToString ());
		yield return new WaitForSeconds (4);
		//Start the Game and disable the Loading Screen
		ManuManager._instance.OpenLayout (LayoutStates._MultiPlayerPanel.ToString ());
		//yield return new WaitForSeconds (ManuManager._instance.manuLayouts [3].layoutGameobject.gameObject.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).length);
		GameEngine._instance.SetNewMQuestion ();
	}

	public IEnumerator CoGetTopTen ()
	{
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("p", currentPlayer.PlayerID + "||." + "20");
		WWW www = new WWW (webServiceLink + "/top10.php", wwwform);
		yield return www;

		if (www.error == null) {
			for (int i = 0; i < top10ContentParent.transform.childCount; i++) {
				Destroy (top10ContentParent.transform.GetChild (i).gameObject);
			}
			top10ContentParent.GetComponent<RectTransform> ().localPosition = Vector3.zero;

			var serializer = new XmlSerializer (typeof(Leaderboard), new XmlRootAttribute ("Leaderboard"));
			Debug.Log (www.text);
			using (TextReader reader = new StringReader (www.text)) {
				var playersList = (Leaderboard)serializer.Deserialize (reader);
				for (int i = 0; i < playersList.PlayerId.Count; i++) {
					GameObject obj = Instantiate (leaderboardContent);
					obj.transform.SetParent (top10ContentParent.transform);
					obj.transform.FindChild ("Rank").GetComponent<Text> ().text = playersList.PlayerRank [i];
					obj.transform.FindChild ("Score").GetComponent<Text> ().text = playersList.TotalScore [i];
					obj.transform.FindChild ("PlayerName").GetComponent<Text> ().text = playersList.PlayerName [i];

					top10ContentParent.GetComponent<RectTransform> ().localPosition = new Vector3 (top10ContentParent.GetComponent<RectTransform> ().localPosition.x,
						top10ContentParent.GetComponent<RectTransform> ().localPosition.y - 150, 0);
					
					if (playersList.PlayerId [i] == currentPlayer.PlayerID) {
						obj.transform.FindChild ("BG").gameObject.SetActive (true);
					} else {
						obj.transform.FindChild ("BG").gameObject.SetActive (false);
					}

					string url = webServiceLink + "/img.php?user=" + playersList.PlayerId [i];
//					WWW wwwImg = new WWW (url);
//					Debug.Log (wwwImg.url);
//					yield return wwwImg;
//					Rect rect = new Rect (0, 0, wwwImg.texture.width, wwwImg.texture.height);
					GetImage (url, obj.transform.FindChild ("PhotoBG").transform.FindChild ("Mask").transform.FindChild ("PlayerPhoto").GetComponent<Image> ());
//					.sprite = Sprite.Create (wwwImg.texture, rect, new Vector2 (0.5f, 0.5f));
				}
				StartCoroutine ("CoGetYourPlace");
			}
		} else {
			//TODO close the Loading screen & Player logined
			Debug.Log (www.error + " error");
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	public IEnumerator CoGetYourPlace ()
	{
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("p", currentPlayer.PlayerID + "||." + "10");
		WWW www = new WWW (webServiceLink + "/userleaderboard.php", wwwform);
		yield return www;

		if (www.error == null) {
			for (int i = 0; i < yourPlaceContentParent.transform.childCount; i++) {
				Destroy (yourPlaceContentParent.transform.GetChild (i).gameObject);
			}
			var serializer = new XmlSerializer (typeof(Leaderboard), new XmlRootAttribute ("Leaderboard"));
			Debug.Log (www.text);
			using (TextReader reader = new StringReader (www.text)) {
				var playersList = (Leaderboard)serializer.Deserialize (reader);
				for (int i = 0; i < playersList.PlayerId.Count; i++) {
					GameObject obj = Instantiate (leaderboardContent);
					obj.transform.SetParent (yourPlaceContentParent.transform);
					obj.transform.FindChild ("Rank").GetComponent<Text> ().text = playersList.PlayerRank [i];
					obj.transform.FindChild ("Score").GetComponent<Text> ().text = playersList.TotalScore [i];
					obj.transform.FindChild ("PlayerName").GetComponent<Text> ().text = playersList.PlayerName [i];

					yourPlaceContentParent.GetComponent<RectTransform> ().localPosition = new Vector3 (yourPlaceContentParent.GetComponent<RectTransform> ().localPosition.x,
						yourPlaceContentParent.GetComponent<RectTransform> ().localPosition.y - 150, 0);

					if (playersList.PlayerId [i] == currentPlayer.PlayerID) {
						obj.transform.FindChild ("BG").gameObject.SetActive (true);
					} else {
						obj.transform.FindChild ("BG").gameObject.SetActive (false);
					}

					string url = webServiceLink + "/img.php?user=" + playersList.PlayerId [i];
					GetImage (url, obj.transform.FindChild ("PhotoBG").transform.FindChild ("Mask").transform.FindChild ("PlayerPhoto").GetComponent<Image> ());

//					WWW wwwImg = new WWW (url);
//					Debug.Log (wwwImg.url);
//					yield return wwwImg;
//					Rect rect = new Rect (0, 0, wwwImg.texture.width, wwwImg.texture.height);
//					obj.transform.FindChild ("PhotoBG").transform.FindChild ("Mask").transform.FindChild ("PlayerPhoto").GetComponent<Image> ().sprite = Sprite.Create (wwwImg.texture, rect, new Vector2 (0.5f, 0.5f));
				}
					
				ManuManager._instance.OpenLayout (LayoutStates._leaderboardPanel.ToString ());
			}
		} else {
			//TODO close the Loading screen & Player logined
			Debug.Log (www.error + " error");
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	public IEnumerator CoGetPlayerStatistics ()
	{
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("p", currentPlayer.PlayerID + "||." + "2");
		WWW www = new WWW (webServiceLink + "/userdata.php", wwwform);
		yield return www;

		if (www.error == null) {
			var serializer = new XmlSerializer (typeof(PlayerStatistics), new XmlRootAttribute ("playerStatistics"));
			Debug.Log (www.text);
			using (TextReader reader = new StringReader (www.text)) {
				var statisticsList = (PlayerStatistics)serializer.Deserialize (reader);

				ManuManager._instance.NumberOfCorrectAnswers.text = statisticsList.CorrectAnswersNbr;
				ManuManager._instance.NumberOfWrongAnswers.text = statisticsList.WrongAnswersNbr;
				ManuManager._instance.NumberOfWinMatchesAnswers.text = statisticsList.TotalWonPlays;
				ManuManager._instance.NumberOflossMatchesAnswers.text = statisticsList.TotalLostPlays;
				ManuManager._instance.OpenLayout (LayoutStates._PlayerStatisticsPanel.ToString ());
			}
		} else {
			//TODO close the Loading screen & Player logined
			Debug.Log (www.error + " error");
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	//----------------------------------- Facebook Services ----------------------------------

	public IEnumerator CoGetFBUserData (List<string> _FBID)
	{
		WWWForm wwwform1 = new WWWForm ();
		wwwform1.AddField ("p", currentPlayer.PlayerID);
		WWW www1 = new WWW (webServiceLink + "/pendfbusers.php", wwwform1);
		yield return www1;

		if (www1.error == null) {
			Debug.Log (www1.text);
			var serializer = new XmlSerializer (typeof(PendingGuests), new XmlRootAttribute ("PendingGuests"));
			using (TextReader reader = new StringReader (www1.text)) {
				var pendingList = (PendingGuests)serializer.Deserialize (reader);
				pendingUsers = pendingList.FacebookId;
			}

		} else {
			Debug.Log (www1.error + " error");
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}

		for (int i = 0; i < ManuManager._instance.FriendsContentHolder.transform.childCount; i++) {
			Destroy (ManuManager._instance.FriendsContentHolder.transform.GetChild (i).gameObject);
		}
		ManuManager._instance.FriendsContentHolder.GetComponent<RectTransform> ().localPosition = Vector3.zero;

		foreach (string fbid in _FBID) {
			WWWForm wwwform = new WWWForm ();
			wwwform.AddField ("p", fbid);
			WWW www = new WWW (webServiceLink + "/fbuserdata.php", wwwform);
			yield return www;

			if (www.error == null) {
				Debug.Log (www.text);

				var serializer = new XmlSerializer (typeof(Player), new XmlRootAttribute ("Player"));
				using (TextReader reader = new StringReader (www.text)) {
					var player = (Player)serializer.Deserialize (reader);
					if (player.PlayerID != "-1") {
						FriendFeild friendsField = Instantiate (FriendsFeild).GetComponent<FriendFeild> ();
						friendsField.transform.SetParent (ManuManager._instance.FriendsContentHolder.transform);

						ManuManager._instance.FriendsContentHolder.GetComponent<RectTransform> ().localPosition = new Vector3 (ManuManager._instance.FriendsContentHolder.GetComponent<RectTransform> ().localPosition.x,
							ManuManager._instance.FriendsContentHolder.GetComponent<RectTransform> ().localPosition.y - 150, 0);

						friendsField.SetName (player.PlayerName);
						friendsField.SetRank (player.UserRank);
						friendsField.SetScore (player.TotalScore);
						friendsField.SetFBID (fbid);

						if (pendingUsers.Contains (fbid)) {
							friendsField.SetBtn (false);
						} else {
							friendsField.SetBtn (true);
						}

						FBGraph.GetFBImage (fbid, friendsField.Pic);
					}
				}
			} else {
				//TODO close the Loading screen & Player logined
				Debug.Log (www.error + " error");
				ManuManager._instance.ConnictionErrorPanel.SetActive (true);
			}
		}

		FBGraph.GetInvitableFriends ();

	}

	public IEnumerator CoGetFBQuestions (int _Qnum, string _PlayerFBID, string _FriendFBID, string _FBSessionID)
	{
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("p", _Qnum + "||." + _PlayerFBID + "||." + _FriendFBID + "||." + _FBSessionID);
		//string url = webServiceLink + "/fbsessionq.php?" + "p=" + _Qnum + "||." + _PlayerFBID + "||." + _FriendFBID + "||." + _FBSessionID;
		WWW www = new WWW (webServiceLink + "/fbsessionq.php", wwwform);
		yield return www;

		if (www.error == null) {
			Debug.Log (www.text);
			//Debug.Log ("http://89.108.164.28:8282/ksa/fbsessionq.php?p=" + _Qnum + "||." + _PlayerFBID + "||." + _FriendFBID + "||." + _FBSessionID);

			string s = "الله";
			string s2 = "الّله";
			string text = www.text.Replace (s, s2);

			var serializer = new XmlSerializer (typeof(MQuestion), new XmlRootAttribute ("MQuestion"));
			using (TextReader reader = new StringReader (text)) {
				var mQuestion = (MQuestion)serializer.Deserialize (reader);
				GameEngine._instance.questionList = mQuestion.Question;
				LastFBSessionID = mQuestion.Sessionid;
				reader.Close ();
				//StartGamePlay
				StartCoroutine ("CoGetQImage");
				yield return new WaitForSeconds (6);
				ManuManager._instance.OpenLayout (LayoutStates._MultiPlayerPanel.ToString ());
				GameEngine._instance.SetNewMQuestion ();
			}

		} else {
			Debug.Log (www.error + " error");
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	public IEnumerator CoPostFBResult (string _sesstionID, string _FBID, string _score)
	{
		Debug.Log ("CoPostFBResult" + "|||||||||||||||||||||||");
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("p", _sesstionID + "||." + _FBID + "||." + _score);
		WWW www = new WWW (webServiceLink + "/postfbsessionscore.php", wwwform);
		yield return www;

		if (www.error == null) {
			Debug.Log (www.text);

			yield return new WaitForSeconds (4);
			ManuManager._instance.OpenLayout (LayoutStates._FriendsListPanel.ToString ());
			ManuManager._instance.FBResultPanel.SetActive (false);
		} else {
			Debug.Log (www.error + " error");
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	public IEnumerator CoGetHostPendingMatches ()
	{
		Debug.Log ("CoGetHostPendingMatches" + "|||||||||||||||||||||||");
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("p", currentPlayer.PlayerID + "||." + 1);
		WWW www = new WWW (webServiceLink + "/fbpendsessions.php", wwwform);

		for (int i = 0; i < ManuManager._instance.MyFriendsRuquestsHolder.transform.childCount; i++) {
			Destroy (ManuManager._instance.MyFriendsRuquestsHolder.transform.GetChild (i).gameObject);
		}
		ManuManager._instance.MyFriendsRuquestsHolder.GetComponent<RectTransform> ().localPosition = Vector3.zero;

		yield return www;

		if (www.error == null) {
			Debug.Log (www.text);

			var serializer = new XmlSerializer (typeof(PlayerMatches), new XmlRootAttribute ("PlayerMatches"));
			using (TextReader reader = new StringReader (www.text)) {
				var playerMatches = (PlayerMatches)serializer.Deserialize (reader);

				for (int i = 0; i < playerMatches.Match.Count; i++) {

					MyRequestsFeild myFriendsField = Instantiate (MyRequestsFeild).GetComponent<MyRequestsFeild> ();
					myFriendsField.transform.SetParent (ManuManager._instance.MyFriendsRuquestsHolder.transform);

					ManuManager._instance.MyFriendsRuquestsHolder.GetComponent<RectTransform> ().localPosition = new Vector3 (ManuManager._instance.MyFriendsRuquestsHolder.GetComponent<RectTransform> ().localPosition.x,
						ManuManager._instance.MyFriendsRuquestsHolder.GetComponent<RectTransform> ().localPosition.y - 150, 0);

					myFriendsField.SetName (playerMatches.Match [i].OpUserName);
					myFriendsField.SetRank (playerMatches.Match [i].OpUserRank);
					myFriendsField.SetScore (playerMatches.Match [i].OpUserScore);
					myFriendsField.SetFBID (playerMatches.Match [i].OpUserFacebookId);
					myFriendsField.SetTime (Convert.ToDateTime (playerMatches.Match [i].Remainingtime));

					LastFBSessionID = playerMatches.Match [i].FacebookSession;

					FBGraph.GetFBImage (playerMatches.Match [i].OpUserFacebookId, myFriendsField.Pic);
				}

				//add The List is empty statment.
				if (playerMatches.Match.Count == 0) {
					GameObject myFriendsField = Instantiate (EmptyListField1);
					myFriendsField.transform.SetParent (ManuManager._instance.MyFriendsRuquestsHolder.transform);
				}

				reader.Close ();
			}

			//ManuManager._instance.OpenLayout (LayoutStates._FriendsListPanel.ToString ());
		} else {
			Debug.Log (www.error + " error");
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	public IEnumerator CoGetGuestPendingMatches (bool ShowPobUp)
	{
		
		Debug.Log ("CoGetGuestPendingMatches" + "|||||||||||||||||||||||");
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("p", currentPlayer.PlayerID + "||." + 2);
		WWW www = new WWW (webServiceLink + "/fbpendsessions.php", wwwform);

		for (int i = 0; i < ManuManager._instance.FriendsRequestsHolder.transform.childCount; i++) {
			Destroy (ManuManager._instance.FriendsRequestsHolder.transform.GetChild (i).gameObject);
		}
		ManuManager._instance.FriendsRequestsHolder.GetComponent<RectTransform> ().localPosition = Vector3.zero;

		yield return www;

		if (www.error == null) {
			Debug.Log (www.text);

			var serializer = new XmlSerializer (typeof(PlayerMatches), new XmlRootAttribute ("PlayerMatches"));
			using (TextReader reader = new StringReader (www.text)) {
				var playerMatches = (PlayerMatches)serializer.Deserialize (reader);

				for (int i = 0; i < playerMatches.Match.Count; i++) {

					FriendsRequestsFeild friendsRequestsFeild = Instantiate (FriendsRequestsFeild).GetComponent<FriendsRequestsFeild> ();
					friendsRequestsFeild.transform.SetParent (ManuManager._instance.FriendsRequestsHolder.transform);

					ManuManager._instance.FriendsRequestsHolder.GetComponent<RectTransform> ().localPosition = new Vector3 (ManuManager._instance.FriendsRequestsHolder.GetComponent<RectTransform> ().localPosition.x,
						ManuManager._instance.FriendsRequestsHolder.GetComponent<RectTransform> ().localPosition.y - 150, 0);

					friendsRequestsFeild.SetName (playerMatches.Match [i].OpUserName);
					friendsRequestsFeild.SetRank (playerMatches.Match [i].OpUserRank);
					friendsRequestsFeild.SetScore (playerMatches.Match [i].OpUserScore);
					friendsRequestsFeild.SetFBID (playerMatches.Match [i].OpUserFacebookId);
//					friendsRequestsFeild.SetTime (Convert.ToDateTime (playerMatches.Match [i].Expiry));
					friendsRequestsFeild.SetFBSessionID ((playerMatches.Match [i].FacebookSession));
					friendsRequestsFeild.SetTime (Convert.ToDateTime (playerMatches.Match [i].Remainingtime));
					 
					LastFBSessionID = playerMatches.Match [i].FacebookSession;

					foreach (Text text in ManuManager._instance.notificationsNum) {
						if (playerMatches.Match.Count > 0) {
							text.transform.parent.gameObject.SetActive (true);
							text.text = playerMatches.Match.Count.ToString ();

							if (ShowPobUp) {
								ManuManager._instance.NotificationsPupUp.SetActive (true);
							}
							//TODO PupUP for Notification
						} else {
							text.transform.parent.gameObject.SetActive (false);
						}
					}

					FBGraph.GetFBImage (playerMatches.Match [i].UserFacebookId, friendsRequestsFeild.Pic);

				}

				//add The List is empty statment.
				if (playerMatches.Match.Count == 0) {
					GameObject EmptyListFeild = Instantiate (EmptyListField2);
					EmptyListFeild.transform.SetParent (ManuManager._instance.FriendsRequestsHolder.transform);
				}

				reader.Close ();
			}
			ManuManager._instance.LoadingPupUp.SetActive (false);
		} else {
			Debug.Log (www.error + " error");
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}

	public IEnumerator CoGetMatchesHistory ()
	{
		ManuManager._instance.LoadingPupUp.SetActive (true);
		Debug.Log ("CoGetMatchesHistory" + "|||||||||||||||||||||||");
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("p", currentPlayer.PlayerID + "||." + 5);
		WWW www = new WWW (webServiceLink + "/fbresulthistory.php", wwwform);

		for (int i = 0; i < ManuManager._instance.ResultsContentHolder.transform.childCount; i++) {
			Destroy (ManuManager._instance.ResultsContentHolder.transform.GetChild (i).gameObject);
		}
		ManuManager._instance.ResultsContentHolder.GetComponent<RectTransform> ().localPosition = Vector3.zero;

		yield return www;

		if (www.error == null) {
			Debug.Log (www.text);

			var serializer = new XmlSerializer (typeof(PlayerMatchesResult), new XmlRootAttribute ("PlayerMatches"));
			using (TextReader reader = new StringReader (www.text)) {
				var playerMatchesResult = (PlayerMatchesResult)serializer.Deserialize (reader);

				for (int i = 0; i < playerMatchesResult.Play.Count; i++) {

					ResultFeild resultFeild = Instantiate (ResultFeild).GetComponent<ResultFeild> ();
					resultFeild.transform.SetParent (ManuManager._instance.ResultsContentHolder.transform);

					ManuManager._instance.ResultsContentHolder.GetComponent<RectTransform> ().localPosition = new Vector3 (ManuManager._instance.ResultsContentHolder.GetComponent<RectTransform> ().localPosition.x,
						ManuManager._instance.ResultsContentHolder.GetComponent<RectTransform> ().localPosition.y - 365, 0);

					if (playerMatchesResult.Play [i].HostFacebookId == currentPlayer.FbId) {
						resultFeild.PSetName (playerMatchesResult.Play [i].HostUsername);
						resultFeild.PSetRank (playerMatchesResult.Play [i].HostUserRank);
						resultFeild.PSetScore (playerMatchesResult.Play [i].HostUserScoreMulti);
						resultFeild.PSetFBID (playerMatchesResult.Play [i].HostFacebookId);
						FBGraph.GetFBImage (playerMatchesResult.Play [i].HostFacebookId, resultFeild.PPic);

						resultFeild.FSetName (playerMatchesResult.Play [i].GuestUsername);
						resultFeild.FSetRank (playerMatchesResult.Play [i].GuestUserRank);
						resultFeild.FSetScore (playerMatchesResult.Play [i].GuestUserScoreMulti);
						resultFeild.FSetFBID (playerMatchesResult.Play [i].GuestFacebookId);
						FBGraph.GetFBImage (playerMatchesResult.Play [i].GuestFacebookId, resultFeild.FPic);

						resultFeild.SetResult (int.Parse (playerMatchesResult.Play [i].HostUserScore), int.Parse (playerMatchesResult.Play [i].GuestUserScore));
					} else {
						resultFeild.FSetName (playerMatchesResult.Play [i].HostUsername);
						resultFeild.FSetRank (playerMatchesResult.Play [i].HostUserRank);
						resultFeild.FSetScore (playerMatchesResult.Play [i].HostUserScoreMulti);
						resultFeild.FSetFBID (playerMatchesResult.Play [i].HostFacebookId);
						FBGraph.GetFBImage (playerMatchesResult.Play [i].HostFacebookId, resultFeild.FPic);

						resultFeild.PSetName (playerMatchesResult.Play [i].GuestUsername);
						resultFeild.PSetRank (playerMatchesResult.Play [i].GuestUserRank);
						resultFeild.PSetScore (playerMatchesResult.Play [i].GuestUserScoreMulti);
						resultFeild.PSetFBID (playerMatchesResult.Play [i].GuestFacebookId);
						FBGraph.GetFBImage (playerMatchesResult.Play [i].GuestFacebookId, resultFeild.PPic);

						resultFeild.SetResult (int.Parse (playerMatchesResult.Play [i].GuestUserScore), int.Parse (playerMatchesResult.Play [i].HostUserScore));
					}
				}

				//add The List is empty statment.
				if (ManuManager._instance.ResultsContentHolder.transform.childCount == 0) {
					GameObject EmptyListFeild = Instantiate (EmptyListField3);
					EmptyListFeild.transform.SetParent (ManuManager._instance.ResultsContentHolder.transform);
				}

				reader.Close ();
			}

			//yield return new WaitForSeconds (0.5f);
			ManuManager._instance.LoadingPupUp.SetActive (false);

		} else {
			Debug.Log (www.error + " error");
			ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}



	public IEnumerator CoGetImage (string _picUrl, Image _img)
	{
		Debug.Log (_picUrl);
		WWW www = new WWW (_picUrl);
		//Debug.Log (www.url);
		yield return www;
		if (www.error == null) {
			//Debug.Log (www.texture);

			_img.sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), new Vector2 (0.5f, 0.5f));

			Debug.Log ("Image Loaded");
		} else {
			//ManuManager._instance.loadingLayout.SetActive (false);
			Debug.Log (www.error);
			//ManuManager._instance.ConnictionErrorPanel.SetActive (true);
		}
	}
	//-----------------------------------------------------------------------------------------

	public bool IsLoggedIn ()
	{
		return isLoggedIn;
	}

	public void PostScore (int userId, int totalQ, int correctA, int score, int result, int type, string qList)
	{
		this.userId = userId;
		this.totalQ = totalQ;
		this.correctA = correctA;
		this.score = score;
		this.result = result;
		this.type = type;
		StartCoroutine ("CoSetscore", qList);
	}

	public void GetImage (string _picUrl, Image _img)
	{
		StartCoroutine (CoGetImage (_picUrl, _img));
	}

	public void GetGuestPendingMatches (bool ShowPobUp)
	{
		StartCoroutine (CoGetGuestPendingMatches (ShowPobUp));
	}

	public void GetMatchesHistory ()
	{
		StartCoroutine (CoGetMatchesHistory ());
	}

	public void GetHostPendingMatches ()
	{
		StartCoroutine (CoGetHostPendingMatches ());
	}

	public void PostFBResult (string _sesstionID, string _FBID, string _score)
	{
		StartCoroutine (CoPostFBResult (_sesstionID, _FBID, _score));
	}

	public void GetFBQuestions (int _Qnum, string _PlayerFBID, string _FriendFBID, string _FBSessionID)
	{
		StartCoroutine (CoGetFBQuestions (_Qnum, _PlayerFBID, _FriendFBID, _FBSessionID));
	}

	public void GetFBUserData (List<string> _FBID)
	{
		StartCoroutine (CoGetFBUserData (_FBID));
	}

	public void GetplayerStatisticts ()
	{
		StartCoroutine ("CoGetPlayerStatistics");
	}

	public void GetTopTen ()
	{
		StartCoroutine ("CoGetTopTen");
	}

	public void GetFriendPic (string playerId, string DeviceId)
	{
		StartCoroutine (CoGetFriendImage (playerId, DeviceId));
	}

	public void Login (string _username, string _deviceId, string _FBID)
	{
		StartCoroutine (CoGetUser (_username, _deviceId, _FBID));
	}

	public void GetFakeUser ()
	{
		StartCoroutine ("CoGetFakeUser");
	}

	public void GetListOfMBQuestions (string _questionNum, string sessionNum)
	{
		StartCoroutine (CoGetListOfMBQuestions (_questionNum, sessionNum));
	}

	public void GetMBSessionNum (string _questionNum)
	{
		StartCoroutine (CoGetMBSessionNum (_questionNum));
	}


	public void GetListOfQuestions (string _questionNum, string _difficultyID, string _categoryID)
	{
		StartCoroutine (CoGetListOfQuestions (_questionNum, _difficultyID, _categoryID));
	}

	public int GetFakeScore (int rank)
	{
		int t = 0;

		if (rank <= 2) {
			t = UnityEngine.Random.Range (1, 3);
		}

		if (rank >= 3 && rank <= 5) {
			t = UnityEngine.Random.Range (4, 6);
		}

		if (rank >= 6 && rank <= 8) {
			t = UnityEngine.Random.Range (5, 7);
		}

		if (rank >= 9 && rank <= 11) {
			t = UnityEngine.Random.Range (6, 8);
		}

		if (rank >= 12) {
			t = UnityEngine.Random.Range (8, 9);
		}

		return t;
	}
}
