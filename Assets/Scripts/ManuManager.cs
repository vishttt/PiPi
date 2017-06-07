using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Prime31;
using RTLService;
using Facebook.Unity;

public class ManuManager : MonoBehaviour
{
	public static ManuManager _instance;
	public LayoutStates currentState = LayoutStates._login;
	public List<Layout> manuLayouts;
	public RawImage InputprofilePic;
	public List<RawImage> profilePics;
	public Image profilePic;
	public Texture2D test;
	public Material profilePicMatl;
	public List<GameObject> levelsObjects;
	public Button nextLevelBtn;
	public Button backLevelBtn;
	//public GameObject loadingLayout;
	public GameObject ConnictionErrorPanel;

	//In Game UI
	public List<Text> answers;
	public Text mainQuestion;
	public Text mainQuestion2;
	public Image Qimage;
	public List<Text> correctAnswersNum;
	public List<Text> singlePlayerScore;
	public List<Text> playerName;
	public List<Text> playerRank;
	public List<Text> PlayerTotalScore;
	public Text timerText;
	public Slider timeBar;
	public Image ResultBG;
	//public Text questionNum;
	public List<Text> categoryNameText;
	public Text WinScreenTotalScore;
	public Text ResultNumberText;
	public Text ResultNumberAnim;
	public Text FinalResultText;
	public Animator QuestionPanelAnimator;
	public Image QBackGround;
	public List<Color> levelsColors;


	//Selection Menu UI
	public List<CorrectAnswersText> CurrectAnserPerCategory;
	public List<GameObject> LevelsLocks;

	//Manu Layout Animators
	public Animator SelectionPanel;
	public Animator SelectCategoryPanel;
	public Animator SinglePlayerPanel;
	public Animator SinglePlayerWinPanel;

	//MultiPlayerUI
	public List<RawImage> friendPic;
	public List<Text> friendName;
	public List<Text> friendRank;
	public List<Text> friendScore;
	public List<Text> mAnswers;
	public Text mMainQuestion;
	public Text mMainQuestion2;
	public Image mQimage;
	public Text mPlayerResult;
	public Text mFriendResult;
	public List<Image> mPlayerResultBG;
	public List<Image> mFriendResultBG;
	public List<Text> mPlayerResultText;
	public List<Text> mFriendResultText;
	public Text mFinalResultWord;
	public Text mExtraScoreAmount;
	public Text mTimerText;
	public Slider mTimeBar;
	public Image barBG;
	public Image fillBarBG;
	public Animator mQuestionPanelAnimator;
	public Text mExtraScoreAmountAnim;
	public Image mBarBG;
	public Image mFillBarBG;
	public Image mQBackGround;


	//LeaderboardUI
	public GameObject TopTenPanel;
	public GameObject YouplacePanel;
	public List<GameObject> topTenPlayers;
	public List<GameObject> yourPlacePlayers;

	//PlayerStatisticsUI
	public Text NumberOfCorrectAnswers;
	public Text NumberOfWrongAnswers;
	public Text NumberOfWinMatchesAnswers;
	public Text NumberOflossMatchesAnswers;
	public Image soundBtn;

	//Frineds List UI
	public List<GameObject> FriendsPanels;
	public GameObject MyRequestsPanel;
	public GameObject FriendsRequestsPanel;

	public GameObject FriendsContentHolder;
	public GameObject MyFriendsRuquestsHolder;
	public GameObject FriendsRequestsHolder;
	public GameObject ResultsContentHolder;

	public GameObject FBResultPanel;
	public Text FBResultNum;

	public List<Text> notificationsNum;

	//private members
	private string imagePath;

	public GameObject LoadingPupUp;
	public GameObject NotificationsPupUp;
	// Use this for initialization
	void Start ()
	{
		_instance = this;
		// Listen to image picker event so we can load the image into a texture later
		EtceteraManager.imagePickerChoseImageEvent += imagePickerChoseImage;
	}



	public void LoginBtn (Text userName)
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);

		if (userName.text != "") {
			Debug.Log ("login");
			BackEnd.instance.Login (userName.text, BackEnd.instance.deviceId, "");
		} else {
			Debug.Log ("User name can't be null");
		}
	}

	public void SetPlayerNameText ()
	{
		foreach (Text name in playerName) {
			name.text = BackEnd.instance.currentPlayer.PlayerName;//RTL.Convert (BackEnd.instance.currentPlayer.PlayerName, RTL.NumberFormat.Context, false);
		}

		foreach (Text score in PlayerTotalScore) {
			score.text = BackEnd.instance.currentPlayer.TotalScore;
		}

		foreach (Text rank in playerRank) {
			rank.text = BackEnd.instance.currentPlayer.UserRank;
		}
	}

	public void TakePic ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		EtceteraBinding.promptForPhoto (0.25f, PhotoPromptType.CameraAndAlbum, 1f, true);
	}

	// Texture loading delegates
	public void textureLoaded (Texture2D texture)
	{
		InputprofilePic.GetComponent<RawImage> ().enabled = true;
		Debug.Log (texture.width + " width " + texture.height);

		InputprofilePic.texture = texture;
		profilePicMatl.mainTexture = texture;

	}

	public void textureLoadFailed (string error)
	{
		var buttons = new string[] { "OK" };
		EtceteraBinding.showAlertWithTitleMessageAndButtons ("Error Loading Texture.  Did you choose a photo first?", error, buttons);
		Debug.Log ("textureLoadFailed: " + error);
	}

	void imagePickerChoseImage (string imagePath)
	{
		this.imagePath = imagePath;
		StartCoroutine (EtceteraManager.textureFromFileAtPath ("file://" + this.imagePath, textureLoaded, textureLoadFailed));
	}

	public void OpenLayout (string layout)
	{
		StartCoroutine ("CoOpenLayout", layout);
	}

	public IEnumerator CoOpenLayout (string layout)
	{
		float waitTime = 0;
		SoundManager._instance.PlayDisAppearLayoutSound (currentState.ToString ());
		foreach (Layout _layout in manuLayouts) {
			
			if (_layout.layoutGameobject.gameObject.activeSelf) {
				_layout.layoutGameobject.gameObject.GetComponent<Animator> ().SetTrigger ("Hide");
				waitTime = _layout.layoutGameobject.gameObject.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).length;
			}
		}

		yield return new WaitForSeconds (waitTime);
		SoundManager._instance.PlayAppearLayoutSound (layout);
		SwitchLayouts (layout);
	}

	public void GetTheResultWord ()
	{
		if (GameEngine._instance.NumofCorrectAnswers > 9) {
			//Exlent
			FinalResultText.text = RTL.Convert ("ممتاز", RTL.NumberFormat.Context, false);
			SoundManager._instance.PlaySound (SoundManager._instance.win);
		}

		if (GameEngine._instance.NumofCorrectAnswers > 7 && GameEngine._instance.NumofCorrectAnswers < 10) {
			//very good
			FinalResultText.text = RTL.Convert ("جيد جدا", RTL.NumberFormat.Context, false);
			SoundManager._instance.PlaySound (SoundManager._instance.tie);
		}

		if (GameEngine._instance.NumofCorrectAnswers > 5 && GameEngine._instance.NumofCorrectAnswers < 8) {
			//good
			FinalResultText.text = RTL.Convert ("جيد", RTL.NumberFormat.Context, false);
			SoundManager._instance.PlaySound (SoundManager._instance.tie);
		}

		if (GameEngine._instance.NumofCorrectAnswers < 6) {
			//bad
			FinalResultText.text = RTL.Convert ("لابأس", RTL.NumberFormat.Context, false);
			SoundManager._instance.PlaySound (SoundManager._instance.lose);
		}
	}

	public void SwitchLayouts (string layout)
	{
		foreach (Layout _layout in manuLayouts) {
			if (_layout.layoutName.ToString () == layout) {
				currentState = _layout.layoutName;
				_layout.layoutGameobject.gameObject.SetActive (true);
			} else {
				_layout.layoutGameobject.gameObject.SetActive (false);
			}
		}
	}

	public void OpenSinglePlayerLevelSelection ()
	{
		OpenLayout ("_selectCategory");
		GetCorrectQuestionNumPerLevel (0);

		//Check the Unlock Levels
		for (int i = 1; i <= LevelsLocks.Count; i++) {
			if (GameEngine._instance._playerData.LevelsData [i].Active == "true") {
				LevelsLocks [i - 1].SetActive (false);
			}
		}
	}


	public void StartSinglePlayerGame (string categoryName)
	{
		GameEngine._instance.isMultiPlayer = false;
		GameEngine._instance.StopTheGame ();
		//Rest the Game Values
		GameEngine._instance.currentQuestionNum = 0;
		GameEngine._instance.TimerCounter = 10;

		GameEngine._instance.isMultiPlayer = false;

		BackEnd.instance.GetListOfQuestions ("10", (GameEngine._instance.SelectedLevel + 1).ToString (), categoryName);
		foreach (Text text in ManuManager._instance.singlePlayerScore) {
			text.text = GameEngine._instance._playerData.TotalCorrectAnswers.ToString ();
		}

		GameEngine._instance.NumofCorrectAnswers = 0;
		//canAnswer = true;

		foreach (Text txet in correctAnswersNum) {
			txet.text = GameEngine._instance.NumofCorrectAnswers.ToString ();
		}

		//set the categry selected 
		GameEngine._instance.SelectedCategoty = int.Parse (categoryName);

		//Set the name of Category as a header
		string t = "";
		string s = ManuManager._instance.levelsObjects [GameEngine._instance.SelectedLevel].transform.FindChild ("HeaderName").GetComponent<Text> ().text;
		switch (categoryName) {
		case "1":
			t = RTL.Convert ("تاريخ", RTL.NumberFormat.Context, false) + " - " + s;
			QBackGround.color = levelsColors [int.Parse (categoryName) - 1];
			barBG.color = new Color (levelsColors [int.Parse (categoryName) - 1].r, levelsColors [int.Parse (categoryName) - 1].g, levelsColors [int.Parse (categoryName) - 1].b, 0.3f);
			fillBarBG.color = levelsColors [int.Parse (categoryName) - 1];
			ResultBG.color = levelsColors [int.Parse (categoryName) - 1];
			foreach (Text text in categoryNameText) {
				text.text = t;
			}
			break;
		case "2":
			t = RTL.Convert ("تراث", RTL.NumberFormat.Context, false) + " - " + s;
			QBackGround.color = levelsColors [int.Parse (categoryName) - 1];
			barBG.color = new Color (levelsColors [int.Parse (categoryName) - 1].r, levelsColors [int.Parse (categoryName) - 1].g, levelsColors [int.Parse (categoryName) - 1].b, 0.3f);
			fillBarBG.color = levelsColors [int.Parse (categoryName) - 1];
			ResultBG.color = levelsColors [int.Parse (categoryName) - 1];
			foreach (Text text in categoryNameText) {
				text.text = t;
			}
			break;
		case "3":
			t = RTL.Convert ("معالم", RTL.NumberFormat.Context, false) + " - " + s;
			QBackGround.color = levelsColors [int.Parse (categoryName) - 1];
			barBG.color = new Color (levelsColors [int.Parse (categoryName) - 1].r, levelsColors [int.Parse (categoryName) - 1].g, levelsColors [int.Parse (categoryName) - 1].b, 0.3f);
			fillBarBG.color = levelsColors [int.Parse (categoryName) - 1];
			ResultBG.color = levelsColors [int.Parse (categoryName) - 1];
			foreach (Text text in categoryNameText) {
				text.text = t;
			}
			break;
		case "4":
			t = RTL.Convert ("دين", RTL.NumberFormat.Context, false) + " - " + s;
			QBackGround.color = levelsColors [int.Parse (categoryName) - 1];
			barBG.color = new Color (levelsColors [int.Parse (categoryName) - 1].r, levelsColors [int.Parse (categoryName) - 1].g, levelsColors [int.Parse (categoryName) - 1].b, 0.3f);
			fillBarBG.color = levelsColors [int.Parse (categoryName) - 1];
			ResultBG.color = levelsColors [int.Parse (categoryName) - 1];
			foreach (Text text in categoryNameText) {
				text.text = t;
			}
			break;
		case "5":
			t = RTL.Convert ("ملوك", RTL.NumberFormat.Context, false) + " - " + s;
			QBackGround.color = levelsColors [int.Parse (categoryName) - 1];
			barBG.color = new Color (levelsColors [int.Parse (categoryName) - 1].r, levelsColors [int.Parse (categoryName) - 1].g, levelsColors [int.Parse (categoryName) - 1].b, 0.3f);
			fillBarBG.color = levelsColors [int.Parse (categoryName) - 1];
			ResultBG.color = levelsColors [int.Parse (categoryName) - 1];
			foreach (Text text in categoryNameText) {
				text.text = t;
			}
			break;
		case "6":
			t = RTL.Convert ("رياضة", RTL.NumberFormat.Context, false) + " - " + s;
			QBackGround.color = levelsColors [int.Parse (categoryName) - 1];
			barBG.color = new Color (levelsColors [int.Parse (categoryName) - 1].r, levelsColors [int.Parse (categoryName) - 1].g, levelsColors [int.Parse (categoryName) - 1].b, 0.3f);
			fillBarBG.color = levelsColors [int.Parse (categoryName) - 1];
			ResultBG.color = levelsColors [int.Parse (categoryName) - 1];
			foreach (Text text in categoryNameText) {
				text.text = t;
			}
			break;
		case "7":
			t = RTL.Convert ("صور", RTL.NumberFormat.Context, false) + " - " + s;
			QBackGround.color = levelsColors [int.Parse (categoryName) - 1];
			barBG.color = new Color (levelsColors [int.Parse (categoryName) - 1].r, levelsColors [int.Parse (categoryName) - 1].g, levelsColors [int.Parse (categoryName) - 1].b, 0.3f);
			fillBarBG.color = levelsColors [int.Parse (categoryName) - 1];
			ResultBG.color = levelsColors [int.Parse (categoryName) - 1];
			foreach (Text text in categoryNameText) {
				text.text = t;
			}
			break;
		}
	}


	public void NextLevelBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		nextLevelBtn.interactable = false;
		StartCoroutine ("CoNextLevel");
	}

	public IEnumerator CoNextLevel ()
	{
		if (GameEngine._instance.SelectedLevel < levelsObjects.Count - 1) {
			
			//yield return 
			if (!levelsObjects [GameEngine._instance.SelectedLevel].transform.FindChild ("Lock").gameObject.activeSelf) {
				levelsObjects [GameEngine._instance.SelectedLevel].GetComponent<Animator> ().SetTrigger ("NextHide");
				SoundManager._instance.PlaySound (SoundManager._instance.levelsDisappear);
			}
			
			yield return new WaitForSeconds (levelsObjects [GameEngine._instance.SelectedLevel].GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).length);
			backLevelBtn.interactable = true;
			levelsObjects [GameEngine._instance.SelectedLevel].SetActive (false);
			GameEngine._instance.SelectedLevel++;
			levelsObjects [GameEngine._instance.SelectedLevel].SetActive (true);

			if (levelsObjects [GameEngine._instance.SelectedLevel].transform.FindChild ("Lock").gameObject.activeSelf) {
				levelsObjects [GameEngine._instance.SelectedLevel].GetComponent<Animator> ().SetTrigger ("Empty");
				SoundManager._instance.PlaySound (SoundManager._instance.animAppearEnd);
			} else {
				levelsObjects [GameEngine._instance.SelectedLevel].GetComponent<Animator> ().SetTrigger ("NextShow");
				SoundManager._instance.PlaySound (SoundManager._instance.levelsAppear);
			}
		} 

		if (GameEngine._instance.SelectedLevel == levelsObjects.Count - 1) {
			nextLevelBtn.interactable = false;
		} else {
			nextLevelBtn.interactable = true;
		}

		yield return null;
	}

	public void BackLevelBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		backLevelBtn.interactable = false;
		StartCoroutine ("CoBacklevel");
	}

	public IEnumerator CoBacklevel ()
	{
		if (GameEngine._instance.SelectedLevel > 0) {
			if (!levelsObjects [GameEngine._instance.SelectedLevel].transform.FindChild ("Lock").gameObject.activeSelf) {
				levelsObjects [GameEngine._instance.SelectedLevel].GetComponent<Animator> ().SetTrigger ("BackHide");
				SoundManager._instance.PlaySound (SoundManager._instance.levelsDisappear);
			}
			
			yield return new WaitForSeconds (levelsObjects [GameEngine._instance.SelectedLevel].GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).length);
			nextLevelBtn.interactable = true;
			levelsObjects [GameEngine._instance.SelectedLevel].SetActive (false);
			GameEngine._instance.SelectedLevel--;
			levelsObjects [GameEngine._instance.SelectedLevel].SetActive (true);

			if (levelsObjects [GameEngine._instance.SelectedLevel].transform.FindChild ("Lock").gameObject.activeSelf) {
				levelsObjects [GameEngine._instance.SelectedLevel].GetComponent<Animator> ().SetTrigger ("Empty");
				SoundManager._instance.PlaySound (SoundManager._instance.animAppearEnd);
			} else {
				levelsObjects [GameEngine._instance.SelectedLevel].GetComponent<Animator> ().SetTrigger ("NextShow");
				SoundManager._instance.PlaySound (SoundManager._instance.levelsAppear);
			}
		}

		if (GameEngine._instance.SelectedLevel == 0) {
			backLevelBtn.interactable = false;
		} else {
			backLevelBtn.interactable = true;
		}
	}

	public int GetCorrectQuestionNumPerLevel (int _level)
	{
		int num = 0;
		Debug.Log (GameEngine._instance._playerData.LevelsData [_level].categories.Count);
		foreach (ClassCategory category in GameEngine._instance._playerData.LevelsData[_level].categories) {
			num += (int)category.CorrectAnswers;
		}
		return num;
	}

	public void AnswerBtn (Button answerBtn)
	{
		StartCoroutine ("CoAnswerBtn", answerBtn);
	}

	public IEnumerator CoAnswerBtn (Button answerBtn)
	{
		if (GameEngine._instance.canAnswer) {
			if (answerBtn.name.Split ('_') [1] == GameEngine._instance.correctanswer.ToString ()) {
				//correct Answer
				GameEngine._instance.canAnswer = false;
				ManuManager._instance.QuestionPanelAnimator.SetBool ("CanAnswer", GameEngine._instance.canAnswer);
				answerBtn.gameObject.GetComponent<Image> ().color = Color.green;
				SoundManager._instance.PlaySound (SoundManager._instance.correctAnswer);
				GameEngine._instance.NumofCorrectAnswers++;
				GameEngine._instance.StopTimer ();
				GameEngine._instance.qList += GameEngine._instance.questionList.QuestionId [GameEngine._instance.currentQuestionNum] + "," + 1 + "||.";
				yield return new WaitForSeconds (1);
				QuestionPanelAnimator.SetTrigger ("QuestionPanelHide");
				foreach (Text txet in correctAnswersNum) {
					txet.text = GameEngine._instance.NumofCorrectAnswers.ToString ();
				}
				Debug.Log ("True answer");
				GameEngine._instance.NextQuestion ();
			} else {
				//Wrong Answer
				GameEngine._instance.canAnswer = false;
				ManuManager._instance.QuestionPanelAnimator.SetBool ("CanAnswer", GameEngine._instance.canAnswer);
				answerBtn.gameObject.GetComponent<Image> ().color = Color.red;
				SoundManager._instance.PlaySound (SoundManager._instance.wrongAnswer);
				//GameEngine._instance.NumofCorrectAnswers++;
				GameEngine._instance.StopTimer ();
				GameEngine._instance.qList += GameEngine._instance.questionList.QuestionId [GameEngine._instance.currentQuestionNum] + "," + 0 + "||.";
				yield return new WaitForSeconds (1);
				ManuManager._instance.answers [GameEngine._instance.correctanswer].transform.parent.GetComponent<Image> ().color = Color.green;
				yield return new WaitForSeconds (1.5f);
				QuestionPanelAnimator.SetTrigger ("QuestionPanelHide");
				GameEngine._instance.NextQuestion ();
			}
		}
	}

	public void UnlockLevelBtn (string Btndata)
	{
		string levelnum = Btndata.Split ('_') [0];
		string questionAmount = Btndata.Split ('_') [1]; 

		if (GameEngine._instance._playerData.TotalCorrectAnswers >= int.Parse (questionAmount)) {
			
			GameEngine._instance._playerData.LevelsData [int.Parse (levelnum)].Active = "true";
			GameEngine._instance._playerData.SaveData (GameEngine._instance._playerData);

			foreach (Button btn in levelsObjects [GameEngine._instance.SelectedLevel].GetComponentsInChildren<Button>()) {
				if (btn.gameObject.name != "Lock") {
					Debug.Log (btn.gameObject.name);
					btn.gameObject.SetActive (true);
				}
			}
			//TODO The Animation Unlock

			LevelsLocks [int.Parse (levelnum) - 1].SetActive (false);
		}
	}

	public void HomeBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		GameEngine._instance.StopTimer ();
		OpenLayout (LayoutStates._selectGame.ToString ());
		GameEngine._instance.StopTheGame ();
	}

	public void BackBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		GameEngine._instance.StopTimer ();
		OpenLayout (LayoutStates._selectCategory.ToString ());
		GameEngine._instance.StopTheGame ();
	}

	public void PlayAgainBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		StartSinglePlayerGame (GameEngine._instance.SelectedCategoty.ToString ());
		Debug.Log ("SelectedLevel " + GameEngine._instance.SelectedCategoty);
	}

	public void ReloadGameBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		Destroy (GameEngine._instance.gameObject);
		Destroy (FaceBookManager._instance.gameObject);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	// ----------------- MultiPlayer functions ------------------
	public void MultiPlayerBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		GameEngine._instance.NumofCorrectAnswers = 0;
		GameEngine._instance.isMultiPlayer = true;
		GameEngine._instance.isFBMultiPlayer = false;
		GameEngine._instance.StopTheGame ();
		GameNetworkManager._instance.JoinFirstFoundMatch ();

	}


	public void MAnswerBtn (Button answerBtn)
	{
		StartCoroutine ("CoMAnswerBtn", answerBtn);
	}

	public IEnumerator CoMAnswerBtn (Button answerBtn)
	{
		if (GameEngine._instance.canAnswer) {
			if (answerBtn.name.Split ('_') [1] == GameEngine._instance.correctanswer.ToString ()) {
				//correct Answer
				GameEngine._instance.canAnswer = false;
				ManuManager._instance.mQuestionPanelAnimator.SetBool ("CanAnswer", GameEngine._instance.canAnswer);
				answerBtn.gameObject.GetComponent<Image> ().color = Color.green;
				SoundManager._instance.PlaySound (SoundManager._instance.correctAnswer);
				GameEngine._instance.NumofCorrectAnswers++;

				if (GameEngine._instance.isFBMultiPlayer)
					GameEngine._instance.StopTimer ();

				if (GameEngine._instance.player1.name.ToLower () == "player1") {
					GameEngine._instance.player1.GetComponent<NetworkPlayer> ().playerScore = GameEngine._instance.NumofCorrectAnswers;
					GameEngine._instance.player1.GetComponent<NetworkPlayer> ().AddScore (GameEngine._instance.NumofCorrectAnswers);
				} else {
					GameEngine._instance.player1.GetComponent<FakeGamePlayer> ().playerScore = GameEngine._instance.NumofCorrectAnswers;
					GameEngine._instance.player1.GetComponent<FakeGamePlayer> ().AddScore (GameEngine._instance.NumofCorrectAnswers);
				}

				Debug.Log ("True answer  " + GameEngine._instance.currentQuestionNum);
				GameEngine._instance.qList += GameEngine._instance.questionList.QuestionId [GameEngine._instance.currentQuestionNum] + "," + 1 + "||.";
			
				if (GameEngine._instance.isFBMultiPlayer) {
					yield return new WaitForSeconds (2);
					mQuestionPanelAnimator.SetTrigger ("QuestionPanelHide");
					GameEngine._instance.NextQuestion ();
				}
				
			} else {
				//Wrong Answers
				GameEngine._instance.canAnswer = false;

				if (GameEngine._instance.isFBMultiPlayer)
					GameEngine._instance.StopTimer ();
				
				ManuManager._instance.mQuestionPanelAnimator.SetBool ("CanAnswer", GameEngine._instance.canAnswer);
				Debug.Log ("Wrong answer");
				answerBtn.gameObject.GetComponent<Image> ().color = Color.red;
				SoundManager._instance.PlaySound (SoundManager._instance.wrongAnswer);
				yield return new WaitForSeconds (1);
				ManuManager._instance.mAnswers [GameEngine._instance.correctanswer].transform.parent.GetComponent<Image> ().color = Color.green;
				GameEngine._instance.qList += GameEngine._instance.questionList.QuestionId [GameEngine._instance.currentQuestionNum] + "," + 0 + "||.";
			
				if (GameEngine._instance.isFBMultiPlayer) {
					yield return new WaitForSeconds (2);
					mQuestionPanelAnimator.SetTrigger ("QuestionPanelHide");
					GameEngine._instance.NextQuestion ();
				}
			}
		}

		yield return null;
	}
	// ----------------- LeaderBoard functions ------------------
	public void LeaderBoardBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		OpenLayout (LayoutStates._loading.ToString ());
		BackEnd.instance.GetTopTen ();
	}

	public void TopTenBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		TopTenPanel.SetActive (true);
		YouplacePanel.SetActive (false);
	}

	public void YouPlaceBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		YouplacePanel.SetActive (true);
		TopTenPanel.SetActive (false);
	}

	//----------------- Manu functions -----------------
	public void MultiplayerFBBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		if (FB.IsLoggedIn) {
			OpenLayout (LayoutStates._loading.ToString ());

			FriendsBtn ();

		} else {
			FaceBookManager._instance.FacebookLogin ();
		}

		//ManuManager._instance.LoadingPupUp.SetActive (true);
		ManuManager._instance.OpenLayout (LayoutStates._FriendsListPanel.ToString ());
	}

	public void ChallangeFriendsBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		OpenLayout (LayoutStates._MultiplayerSelectionPanel.ToString ());
	}

	public void StatisticsBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		OpenLayout (LayoutStates._loading.ToString ());
		BackEnd.instance.GetplayerStatisticts ();
	}

	public void SittingsBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		OpenLayout (LayoutStates._SittingsPanel.ToString ());
	}

	public void LogoutBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		PlayerPrefs.SetString ("_username", "");
		OpenLayout (LayoutStates._login.ToString ());
	}

	public void SoundBtn ()
	{
		if (PlayerPrefs.GetString ("sound") == "on") {
			soundBtn.color = new Color (1, 1, 1, 0.4f);
			soundBtn.GetComponent<Animator> ().SetTrigger ("Play");
			PlayerPrefs.SetString ("sound", "off");
			SoundManager._instance.mute = true;
		} else {
			soundBtn.color = new Color (1, 1, 1, 1);
			soundBtn.GetComponent<Animator> ().SetTrigger ("Play");
			PlayerPrefs.SetString ("sound", "on");
			SoundManager._instance.mute = false;
			SoundManager._instance.PlaySound (SoundManager._instance.btn);
		}
	}

	public void CloseNotifiactionsPanelBtn ()
	{
		ManuManager._instance.NotificationsPupUp.SetActive (false);
	}

	public void OpenMatchesLayoutBtn ()
	{
		ManuManager._instance.NotificationsPupUp.SetActive (false);

		MultiplayerFBBtn ();
		//OpenLayout (LayoutStates._FriendsListPanel.ToString ());

		foreach (GameObject panel in FriendsPanels) {
			if (panel.name == "WaitingListPanel") {
				panel.SetActive (true);
			} else {
				panel.SetActive (false);
			}
		}

		FriendsRequestsBtn ();
	}
	// ---------------- FaceBook Login And Share -----------------

	public void FaceBookLoginBtn ()
	{
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
		FaceBookManager._instance.FacebookLogin ();
	}

	public void SetInvaitableFriendFaild (string _id, string _name, string _picUrl)
	{
		InvitableFriendsField friendsField = Instantiate (BackEnd.instance.InvitableFriendsField).GetComponent<InvitableFriendsField> ();
		friendsField.transform.SetParent (ManuManager._instance.FriendsContentHolder.transform);

		FriendsContentHolder.GetComponent<RectTransform> ().localPosition = new Vector3 (FriendsContentHolder.GetComponent<RectTransform> ().localPosition.x,
			FriendsContentHolder.GetComponent<RectTransform> ().localPosition.y - 150, 0);
		
		friendsField.SetName (_name);
		friendsField.SetFBID (_id);

		BackEnd.instance.GetImage (_picUrl, friendsField.Pic);
	}

	public void InviteFriends ()
	{
		FBRequest.RequestChallenge (null);
	}
	// ---------------- Challange Friends Panel Functions -----------------

	public void FriendsBtn ()
	{
		FBGraph.GetFriends ();

		foreach (GameObject panel in FriendsPanels) {
			if (panel.name == "FriendsPanel") {
				panel.SetActive (true);
			} else {
				panel.SetActive (false);
			}
		}

		BackEnd.instance.GetFBUserData (FBGraph.userFriendsIds);
		SoundManager._instance.PlaySound (SoundManager._instance.btn);
	}

	public void WaitingListBtn ()
	{
		ManuManager._instance.LoadingPupUp.SetActive (true);
		BackEnd.instance.GetHostPendingMatches ();
		BackEnd.instance.GetGuestPendingMatches (false);


		foreach (GameObject panel in FriendsPanels) {
			if (panel.name == "WaitingListPanel") {
				panel.SetActive (true);
			} else {
				panel.SetActive (false);
			}
		}

		SoundManager._instance.PlaySound (SoundManager._instance.btn);
	}

	public void ResultsListBtn ()
	{
		BackEnd.instance.GetMatchesHistory ();

		foreach (GameObject panel in FriendsPanels) {
			if (panel.name == "ResultsListPanel") {
				panel.SetActive (true);
			} else {
				panel.SetActive (false);
			}
		}

		SoundManager._instance.PlaySound (SoundManager._instance.btn);
	}

	public void FriendsRequestsBtn ()
	{
		MyRequestsPanel.SetActive (false);
		FriendsRequestsPanel.SetActive (true);

		SoundManager._instance.PlaySound (SoundManager._instance.btn);

		foreach (Text text in ManuManager._instance.notificationsNum) {
			text.transform.parent.gameObject.SetActive (false);
		}
	}

	public void MyRequestsBtn ()
	{
		FriendsRequestsPanel.SetActive (false);
		MyRequestsPanel.SetActive (true);

		SoundManager._instance.PlaySound (SoundManager._instance.btn);
	}
}
