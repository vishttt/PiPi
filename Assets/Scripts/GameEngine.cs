using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using RTLService;
using System.IO;
using ArabicSupport;


public class GameEngine : MonoBehaviour
{

	public static GameEngine _instance;

	//public members
	public GameDataClass _playerData;
	private static string ScrrenShotPath = Application.persistentDataPath + "/Screenshot.png";

	public Question questionList;
	public int currentQuestionNum = 0;
	public int singlePlayerScore = 0;
	public int NumofCorrectAnswers = 0;
	public int correctanswer = 0;
	public int SelectedLevel = 0;
	public int SelectedCategoty = 1;
	public bool canAnswer = false;
	public bool isGameStart = false;
	public bool isMultiPlayer = false;
	public bool isFBMultiPlayer = false;
	public Texture2D _screenShotTexture;
	public GameObject PointsSys;
	public GameObject MPointsSys;
	public string qList = "";
	public List<object> Friends;

	//Multiplayer members
	//	public int player1Score = 0;
	//	public int player2Score = 0;
	public GameObject player1;

	//Private members
	public int TimerCounter = 10;
	private List<Text> wrongAnswers = new List<Text> ();


	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad (gameObject);

		Application.targetFrameRate = 60;
		Application.runInBackground = true;

		_instance = this;
		_playerData.LoadData (ref _playerData);
		CalculateTotalQuestions ();

	}

	void OnApplicationPause (bool isGamePause)
	{
		if (isGamePause) {
			//StopCoroutine ("CoStartQuestionTimer");
			//StopCoroutine ("CoMoveTimerBar");
			ManuManager._instance.timeBar.value = 0;
			ManuManager._instance.mTimeBar.value = 0;
			TimerCounter = 1;
		}
	}

	public void IntialzeTheNewData ()
	{
		for (int i = 0; i < 5; i++) {
			_playerData.TotalCorrectAnswers = 0;
			LevelClass obj = new LevelClass ();
			obj.levelNum = i;

			obj.Active = "false";
			_playerData.LevelsData.Add (obj);

			for (int j = 0; j < 7; j++) {
				ClassCategory newobj = new ClassCategory ();
				newobj.categotyName = (j + 1).ToString ();
				newobj.CorrectAnswers = 0;
				_playerData.LevelsData [i].categories.Add (newobj);
			}
		}
	}

	public void NextQuestion ()
	{
		if (currentQuestionNum < 9) {
			//Set the next Quetion
			currentQuestionNum++;
			if (isMultiPlayer) {
				StartCoroutine ("CoSetNewMQuestion", 1);
			} else {
				StartCoroutine ("CoSetNewQuestion", 1);
			}
		} else {
			//Game Over
			if (isMultiPlayer) {
				if (isFBMultiPlayer) {
					Debug.Log ("Game Over");
					FBMultiPlayerGameOver ();
				} else {
					MultiPlayerGameOver ();
				}
			} else {
				SinglePlayerGameOver ();
			}
		}
	}


	public IEnumerator CoMoveTimerBar ()
	{
		yield return new WaitForSeconds (0.3f);
		ManuManager._instance.timeBar.value = 10;
		ManuManager._instance.mTimeBar.value = 10;


		while (true) {
			yield return new WaitForSeconds (0.01f);
			ManuManager._instance.timeBar.value -= 0.0181f;
			ManuManager._instance.mTimeBar.value -= 0.0181f;
		}

		//yield return null;
	}

	public IEnumerator CoStartQuestionTimer ()
	{
		yield return new WaitForSeconds (0.3f);
		TimerCounter = 10;
		ManuManager._instance.timerText.text = TimerCounter + "s";
		ManuManager._instance.mTimerText.text = TimerCounter + "s";

		while (true) {
			
			if (TimerCounter <= 0) {
				//TODO Next Question
				GameEngine._instance.canAnswer = false;
				yield return new WaitForSeconds (1);
				if (!isMultiPlayer) {
					ManuManager._instance.QuestionPanelAnimator.SetBool ("CanAnswer", GameEngine._instance.canAnswer);
					ManuManager._instance.QuestionPanelAnimator.SetTrigger ("QuestionPanelHide");
				}
				if (isMultiPlayer) {
					ManuManager._instance.mQuestionPanelAnimator.SetBool ("CanAnswer", GameEngine._instance.canAnswer);
					ManuManager._instance.mQuestionPanelAnimator.SetTrigger ("QuestionPanelHide");
				}
					
				NextQuestion ();

				break;
			} else {
				yield return new WaitForSeconds (1f);
				TimerCounter--;
				ManuManager._instance.timerText.text = TimerCounter + "s";
				ManuManager._instance.mTimerText.text = TimerCounter + "s";
			}
		}
		yield return null;
	}

	public void SetNewQuestion ()
	{
		ClearQuestionText ();
		StartCoroutine ("CoSetNewQuestion", 1);
	}

	public IEnumerator CoSetNewQuestion (float waitingTime)
	{
		
		yield return new WaitForSeconds (waitingTime);
		ClearQuestionText ();
		canAnswer = true;
		ManuManager._instance.QuestionPanelAnimator.SetBool ("CanAnswer", GameEngine._instance.canAnswer);
		int questionNum = currentQuestionNum + 1;

		correctanswer = Random.Range (0, 4);

		try {
			if (questionList.Image [currentQuestionNum] != "") {
				ManuManager._instance.mainQuestion2.text = RTL.Convert (questionList.QuestionDesc [currentQuestionNum], RTL.NumberFormat.Arabic, false);
				ManuManager._instance.Qimage.gameObject.SetActive (true);
				Rect rect = new Rect (0, 0, BackEnd.instance.QImages [questionList.QuestionId [currentQuestionNum]].width, BackEnd.instance.QImages [questionList.QuestionId [currentQuestionNum]].height);
				ManuManager._instance.Qimage.sprite = Sprite.Create (BackEnd.instance.QImages [questionList.QuestionId [currentQuestionNum]], rect, new Vector2 (0.5f, 0.5f));
			} else {
				ManuManager._instance.mainQuestion.text = RTL.ConvertWordWrap (" " + questionList.QuestionDesc [currentQuestionNum] + " ", 150, null, RTL.NumberFormat.Arabic, false);
				//ManuManager._instance.mainQuestion.text = RTL.ConvertWordWrap ("زيبنازمهستيزمتسيمن منسيزمنساي نما مايسمنز اسيمن زمنسي ازمن ايسمنزاسنمزامنساز منسا مزناس منزنسا زمنسبا زمناسب زمناسبمنز ابنماز نمبازمن بسمنز سبنم " + questionList.QuestionDesc [currentQuestionNum] + " ", 150, null, RTL.NumberFormat.Arabic, false);
			}
		} catch {
			Debug.Log ("currentQuestionNum Error out of range");
		}
			
		wrongAnswers.Clear ();

		foreach (Text answer in ManuManager._instance.answers) {
			answer.transform.parent.GetComponent<Image> ().color = Color.white;
			if (answer.name == correctanswer.ToString ()) {
				answer.text = RTL.Convert (" " + questionList.CAnswer [currentQuestionNum] + " ", RTL.NumberFormat.Arabic, false);
			} else {
				wrongAnswers.Add (answer);
			}
		}
	
		Debug.Log (wrongAnswers.Count + "  wrongAnswers count");
		wrongAnswers [0].text = RTL.Convert (" " + questionList.FAnswer1 [currentQuestionNum] + " ", RTL.NumberFormat.Arabic, false);
		wrongAnswers [1].text = RTL.Convert (" " + questionList.FAnswer2 [currentQuestionNum] + " ", RTL.NumberFormat.Arabic, false);
		wrongAnswers [2].text = RTL.Convert (" " + questionList.FAnswer3 [currentQuestionNum] + " ", RTL.NumberFormat.Arabic, false);

		StopCoroutine ("CoStartQuestionTimer");
		StopCoroutine ("CoMoveTimerBar");
		ManuManager._instance.timeBar.value = 10;
		ManuManager._instance.mTimeBar.value = 10;

		PointsSys.GetComponent<QuestionPointSystem> ().NextPoint (currentQuestionNum);

		yield return new WaitForSeconds (1.5f);
		StartCoroutine ("CoStartQuestionTimer");
		StartCoroutine ("CoMoveTimerBar");
	}

	public void SinglePlayerGameOver ()
	{
		ManuManager._instance.GetTheResultWord ();
		ManuManager._instance.QuestionPanelAnimator.SetTrigger ("QuestionPanelHide");
		ManuManager._instance.OpenLayout (LayoutStates._SinglePlayerWinPanal.ToString ());
		ManuManager._instance.ResultNumberText.text = GameEngine._instance.NumofCorrectAnswers.ToString () + "/10";

		ManuManager._instance.WinScreenTotalScore.text = CalculateTotalQuestions ().ToString ();


		BackEnd.instance.PostScore (int.Parse (BackEnd.instance.currentPlayer.PlayerID), 10, GameEngine._instance.NumofCorrectAnswers
		, 0, 1, 1, GameEngine._instance.qList);

		StopCoroutine ("CoStartQuestionTimer");
		StopCoroutine ("CoMoveTimerBar");

		//Set the Total Score for the single player and set the final Result Text save it
		if (_playerData.LevelsData [SelectedLevel].categories [SelectedCategoty - 1].CorrectAnswers < NumofCorrectAnswers) {

			ManuManager._instance.ResultNumberAnim.text = "+" + (GameEngine._instance.NumofCorrectAnswers - _playerData.LevelsData [SelectedLevel].categories [SelectedCategoty - 1].CorrectAnswers).ToString ();
			ManuManager._instance.ResultNumberAnim.gameObject.SetActive (true);

			_playerData.LevelsData [SelectedLevel].categories [SelectedCategoty - 1].CorrectAnswers = NumofCorrectAnswers;
			_playerData.TotalCorrectAnswers = CalculateTotalQuestions ();
			_playerData.SaveData (_playerData);
		}
	}

	public int CalculateTotalQuestions ()
	{
		int score = 0;

		for (int i = 0; i < _playerData.LevelsData.Count; i++) {

			for (int j = 0; j < _playerData.LevelsData [i].categories.Count; j++) {
				score += _playerData.LevelsData [i].categories [j].CorrectAnswers;
				ManuManager._instance.CurrectAnserPerCategory [i].CorrectAnswersPerCategory [j].text = _playerData.LevelsData [i].categories [j].CorrectAnswers.ToString ();
			}

			Debug.Log (score + "  Score");
		}

		foreach (Text text in ManuManager._instance.singlePlayerScore) {
			text.text = score.ToString ();
		}

		return score;
	}

	public void StopTheGame ()
	{
		StopCoroutine ("CoStartQuestionTimer");
		StopCoroutine ("CoMoveTimerBar");
		GameEngine._instance.currentQuestionNum = 0;
		GameEngine._instance.TimerCounter = 10;
	}

	public void StopTimer ()
	{
		StopCoroutine ("CoStartQuestionTimer");
		StopCoroutine ("CoMoveTimerBar");
	}

	public void ClearQuestionText ()
	{
		foreach (Text answer in ManuManager._instance.answers) {
			answer.text = "";
		}
		ManuManager._instance.mainQuestion.text = "";
		ManuManager._instance.mainQuestion2.text = "";
		ManuManager._instance.Qimage.gameObject.SetActive (false);
	}

	//// ----------------- MultiPlayer functions ------------------

	public void SetNewMQuestion ()
	{
		MClearQuestionText ();
		StartCoroutine ("CoSetNewMQuestion", 1);
	}

	public void setMultiplayerQColor (int qNum)
	{
		Debug.Log ("qNum ======== " + qNum);
		ManuManager._instance.mQBackGround.color = ManuManager._instance.levelsColors [qNum];
		ManuManager._instance.mBarBG.color = new Color (ManuManager._instance.levelsColors [qNum].r, ManuManager._instance.levelsColors [qNum].g, ManuManager._instance.levelsColors [qNum].b, 0.3f);
		ManuManager._instance.mFillBarBG.color = ManuManager._instance.levelsColors [qNum];
	}

	public IEnumerator CoSetNewMQuestion (float WaitingTime)
	{
		yield return new WaitForSeconds (WaitingTime);

		canAnswer = true;
		ManuManager._instance.mQuestionPanelAnimator.SetBool ("CanAnswer", GameEngine._instance.canAnswer);
		//int questionNum = currentQuestionNum + 1;
		correctanswer = Random.Range (0, 4);

		setMultiplayerQColor (int.Parse (questionList.CategoryId [currentQuestionNum]) - 1);
		if (questionList.Image [currentQuestionNum] != "") {
			ManuManager._instance.mMainQuestion2.text = RTL.Convert (questionList.QuestionDesc [currentQuestionNum], RTL.NumberFormat.Context, false);
			ManuManager._instance.mMainQuestion.text = "";
			ManuManager._instance.mQimage.gameObject.SetActive (true);
			Rect rect = new Rect (0, 0, BackEnd.instance.QImages [questionList.QuestionId [currentQuestionNum]].width, BackEnd.instance.QImages [questionList.QuestionId [currentQuestionNum]].height);
			ManuManager._instance.mQimage.sprite = Sprite.Create (BackEnd.instance.QImages [questionList.QuestionId [currentQuestionNum]], rect, new Vector2 (0.5f, 0.5f));
		} else {
			ManuManager._instance.mQimage.gameObject.SetActive (false);
			ManuManager._instance.mMainQuestion2.text = "";
			ManuManager._instance.mMainQuestion.text = RTL.ConvertWordWrap (" " + questionList.QuestionDesc [currentQuestionNum] + " ", 150, null, RTL.NumberFormat.Arabic, false);
		}

		wrongAnswers.Clear ();

		foreach (Text answer in ManuManager._instance.mAnswers) {

			answer.transform.parent.GetComponent<Image> ().color = Color.white;

			if (answer.name == correctanswer.ToString ()) {
				answer.text = RTL.Convert (" " + questionList.CAnswer [currentQuestionNum] + " ", RTL.NumberFormat.Arabic, false);
			} else {
				wrongAnswers.Add (answer);
			}
		}

		Debug.Log (wrongAnswers.Count + "  wrongAnswers count");
		wrongAnswers [0].text = RTL.Convert (" " + questionList.FAnswer1 [currentQuestionNum] + " ", RTL.NumberFormat.Arabic, false);
		wrongAnswers [1].text = RTL.Convert (" " + questionList.FAnswer2 [currentQuestionNum] + " ", RTL.NumberFormat.Arabic, false);
		wrongAnswers [2].text = RTL.Convert (" " + questionList.FAnswer3 [currentQuestionNum] + " ", RTL.NumberFormat.Arabic, false);

		MPointsSys.GetComponent<QuestionPointSystem> ().NextPoint (currentQuestionNum);

		StopCoroutine ("CoStartQuestionTimer");
		StopCoroutine ("CoMoveTimerBar");
		StartCoroutine ("CoStartQuestionTimer");
		StartCoroutine ("CoMoveTimerBar");
	}

	public void MClearQuestionText ()
	{
		foreach (Text answer in ManuManager._instance.mAnswers) {
			answer.text = "";
		}
		ManuManager._instance.mMainQuestion.text = "";
		ManuManager._instance.mMainQuestion2.text = "";
		ManuManager._instance.mQimage.gameObject.SetActive (false);
	}

	public void FBMultiPlayerGameOver ()
	{
		ManuManager._instance.FBResultPanel.SetActive (true);
		ManuManager._instance.FBResultNum.text = GameEngine._instance.NumofCorrectAnswers.ToString () + "/10";
		//TODO Poset Result
		StopCoroutine ("CoStartQuestionTimer");
		StopCoroutine ("CoMoveTimerBar");
		BackEnd.instance.PostFBResult (BackEnd.instance.LastFBSessionID, BackEnd.instance.currentPlayer.FbId, GameEngine._instance.NumofCorrectAnswers.ToString ());
		MPointsSys.GetComponent<QuestionPointSystem> ().Rest ();
	}


	public void MultiPlayerGameOver ()
	{
		int player1Score = int.Parse (ManuManager._instance.mPlayerResult.text.Split ('/') [0]);
		int player2Score = int.Parse (ManuManager._instance.mFriendResult.text.Split ('/') [0]);

		if (player1Score == player2Score) {
			//You are draw , set the draw massage
			ManuManager._instance.mFinalResultWord.text = RTL.Convert ("تعادل", RTL.NumberFormat.Context, false);
			ManuManager._instance.mFinalResultWord.transform.parent.GetComponent<Image> ().color = Color.green;
			ManuManager._instance.mExtraScoreAmount.text = "1";
			BackEnd.instance.currentPlayer.TotalScore = (int.Parse (BackEnd.instance.currentPlayer.TotalScore) + 1).ToString ();

			foreach (Image playerResultbg in ManuManager._instance.mPlayerResultBG)
				playerResultbg.color = Color.green;

			foreach (Image friendResultbg in ManuManager._instance.mFriendResultBG)
				friendResultbg.color = Color.green;

			foreach (Text playerResultText in ManuManager._instance.mPlayerResultText)
				playerResultText.text = RTL.Convert ("", RTL.NumberFormat.Context, false);

			foreach (Text friendResultText in ManuManager._instance.mFriendResultText)
				friendResultText.text = RTL.Convert ("", RTL.NumberFormat.Context, false);

			//PlayEffect
			ManuManager._instance.mExtraScoreAmountAnim.text = "+1";
			ManuManager._instance.mExtraScoreAmountAnim.gameObject.SetActive (true);
			SoundManager._instance.PlaySound (SoundManager._instance.tie);

			//PostScore to the server
			BackEnd.instance.PostScore (int.Parse (BackEnd.instance.currentPlayer.PlayerID), 10, int.Parse (ManuManager._instance.mPlayerResult.text.Split ('/') [0])
				, 1, 2, 2, GameEngine._instance.qList);

		} else if (player1Score > player2Score) {
			//You are won , set the win massage
			ManuManager._instance.mFinalResultWord.text = RTL.Convert ("مبروك لقد ربحت", RTL.NumberFormat.Context, false);
			ManuManager._instance.mFinalResultWord.transform.parent.GetComponent<Image> ().color = Color.green;
			ManuManager._instance.mExtraScoreAmount.text = "3";
			BackEnd.instance.currentPlayer.TotalScore = (int.Parse (BackEnd.instance.currentPlayer.TotalScore) + 3).ToString ();

			foreach (Image playerResultbg in ManuManager._instance.mPlayerResultBG)
				playerResultbg.color = Color.green;

			foreach (Image friendResultbg in ManuManager._instance.mFriendResultBG)
				friendResultbg.color = Color.red;

			foreach (Text playerResultText in ManuManager._instance.mPlayerResultText)
				playerResultText.text = RTL.Convert ("الفائز", RTL.NumberFormat.Context, false);

			foreach (Text friendResultText in ManuManager._instance.mFriendResultText)
				friendResultText.text = RTL.Convert ("الخاسر", RTL.NumberFormat.Context, false);

			//PlayEffect
			ManuManager._instance.mExtraScoreAmountAnim.text = "+1";
			ManuManager._instance.mExtraScoreAmountAnim.gameObject.SetActive (true);
			SoundManager._instance.PlaySound (SoundManager._instance.win);

			//post Score to Server
			BackEnd.instance.PostScore (int.Parse (BackEnd.instance.currentPlayer.PlayerID), 10, int.Parse (ManuManager._instance.mPlayerResult.text.Split ('/') [0])
				, 3, 3, 2, GameEngine._instance.qList);
			
		} else {
			//You are Loss , set the loss massage
			ManuManager._instance.mFinalResultWord.text = RTL.Convert ("حظا موفقا المرة المقبلة", RTL.NumberFormat.Context, false);
			ManuManager._instance.mFinalResultWord.transform.parent.GetComponent<Image> ().color = Color.red;
			ManuManager._instance.mExtraScoreAmount.text = "0";

			foreach (Image playerResultbg in ManuManager._instance.mPlayerResultBG)
				playerResultbg.color = Color.red;

			foreach (Image friendResultbg in ManuManager._instance.mFriendResultBG)
				friendResultbg.color = Color.green;

			foreach (Text playerResultText in ManuManager._instance.mPlayerResultText)
				playerResultText.text = RTL.Convert ("الخاسر", RTL.NumberFormat.Context, false);

			foreach (Text friendResultText in ManuManager._instance.mFriendResultText)
				friendResultText.text = RTL.Convert ("الفائز", RTL.NumberFormat.Context, false);

			SoundManager._instance.PlaySound (SoundManager._instance.lose);
			BackEnd.instance.PostScore (int.Parse (BackEnd.instance.currentPlayer.PlayerID), 10, int.Parse (ManuManager._instance.mPlayerResult.text.Split ('/') [0])
				, 0, 1, 2, GameEngine._instance.qList);
		}
		ManuManager._instance.OpenLayout (LayoutStates._MultiPlayerWinPanel.ToString ());
		StopCoroutine ("CoStartQuestionTimer");
		StopCoroutine ("CoMoveTimerBar");
		StartCoroutine ("CoLoadScreenShotScene");
	}


	public IEnumerator CoLoadScreenShotScene ()
	{
		yield return new WaitForSeconds (2);
		yield return new WaitForEndOfFrame ();
		Debug.Log (ScrrenShotPath);
//		Application.CaptureScreenshot (ScrrenShotPath);
		_screenShotTexture = new Texture2D (Screen.width, Screen.height, TextureFormat.RGB24, true);

		_screenShotTexture.ReadPixels (new Rect (0f, 0f, Screen.width, Screen.height), 0, 0);

		_screenShotTexture.Apply ();

		yield return new WaitForSeconds (5);
		//TODO Close the Room And Dissconnect to the server
		GameNetworkManager._instance.LeaveRoom ();
		ManuManager._instance.OpenLayout (LayoutStates._loading.ToString ());
		yield return new WaitForSeconds (2);
		SceneManager.LoadScene ("Screenshot");

		//ManuManager._instance.OpenLayout (LayoutStates._MultiPlayerWin2Panel.ToString ());
	}


	// ---------------------------------------------------------------
}
