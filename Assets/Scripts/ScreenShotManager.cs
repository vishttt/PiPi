using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class ScreenShotManager : MonoBehaviour
{
	public Image screenShotImage;
	Texture2D _texture;
	// Use this for initialization
	void Awake ()
	{

	}

	void Start ()
	{
		Rect rect = new Rect (0, 0, GameEngine._instance._screenShotTexture.width, GameEngine._instance._screenShotTexture.height);
		screenShotImage.sprite = Sprite.Create (GameEngine._instance._screenShotTexture, rect, new Vector2 (0.5f, 0.5f));
	}

	public void HomeBtn ()
	{
		Destroy (GameEngine._instance.gameObject);
		Destroy (FaceBookManager._instance.gameObject);
		SceneManager.LoadScene ("Game");
	}

	public void ShareBtn ()
	{
		FaceBookManager._instance.FbShareImage ();
	}
}
