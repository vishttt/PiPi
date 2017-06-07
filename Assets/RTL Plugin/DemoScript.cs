using RTLService;
using UnityEngine;

public class DemoScript : MonoBehaviour
{

	#region Unity Inspector variables

	// The original text
	public string InputText = "السلام العلیک - سلام خوش آمدید - שלום - ہیلو - העלא";

	// Number format in converted text (1,2,3 or ١,٢,٣)
	public RTL.NumberFormat NumberFormat = RTL.NumberFormat.Arabic;

	// Right to left text element width
	public int TextWidth = 460;

	// True value means the main context is in English (any left to right language) including some Arabic (any right to left) words inside.
	public bool LtrContext = false;

	// Used only for demo project
	public GUISkin Skin;

	#endregion

	// Variable to store converted text
	private string convertedText = "";

	float y = 0;
	// layuot gui element
	Rect rtlRect = new Rect ();
	// Rect used to draw rtl text label

	bool prevCheckboxState = false;
	// states for check if need to refresh the converted text
	string prevInputText = "";
	int prevWidth = 0;

	void Update ()
	{
		// In general we'd prefer to use calculation and conversion methods inside Update() method rather than OnGUI().
		// If you're not changing the converted RTL text's font-size or its GUI element's width at runtime, it's highly recommended to use 
		// RTL method here in Update() method for resource usage benefits.
		// You may use GUI.changed property (like this demo) to prevent calling convert method in each OnGUI() call.
		// However in Unity 5.x you would use GUI event handlers to convert the text as needed.
	}

	void OnGUI ()
	{
		// Set GUI Skin
		GUI.skin = Skin;

		//Skin.customStyles[0] -> Style for right to left texts in this demo
		//Skin.customStyles[1] -> Style for left to right texts in this demo

		GUI.color = Color.white;

		// Draw plugin version info
		y = 4;
		GUI.Label (new Rect (6, y, 600, 20), "RTL Plugin 5.2.3 | Right to left text converter (Arabic, Hebrew, Farsi, Urdu, Yiddish, Kurdish and Pashto)", Skin.customStyles [1]);

		y += 32;

		GUI.Label (new Rect (6, y, 200, 25), "Unity:", Skin.customStyles [1]);

		// The original text input
		InputText = GUI.TextArea (new Rect (76, y, 460, 150), InputText);


		y += 160;
		GUI.Label (new Rect (6, y, 200, 25), "RTL:", Skin.customStyles [1]);
		rtlRect = new Rect (76, y, TextWidth, 150);

		if (GUI.changed || LtrContext != prevCheckboxState || prevInputText != InputText || prevWidth != TextWidth) {
			// Don't overkill CPU and call convert function olny when needed!
			UpdateRTLText ();
		}


		GUI.Box (rtlRect, "");
		GUI.Box (rtlRect, "");

		//Draw the converted text
		GUI.Label (rtlRect, convertedText, Skin.customStyles [0]);


		y += 160;

		// Draw RTL options on the screen to let users change RTL settings in game view mode at runtime
		GUI.Label (new Rect (6, y, 200, 25), "Options:", Skin.customStyles [1]);


		GUI.Label (new Rect (104, y + 2, 400, 25), "Number Format:", Skin.customStyles [1]);
		if (GUI.Button (new Rect (226, y, 100, 25), "Arabic (١٢٣)")) {
			NumberFormat = RTL.NumberFormat.Arabic;
			UpdateRTLText ();
		}
		if (GUI.Button (new Rect (330, y, 100, 25), "English (123)")) {
			NumberFormat = RTL.NumberFormat.English;
			UpdateRTLText ();
		}
		if (GUI.Button (new Rect (434, y, 100, 25), "Context")) {
			NumberFormat = RTL.NumberFormat.Context; 
			UpdateRTLText ();
		}

		y += 30;
		GUI.Label (new Rect (104, y + 2, 400, 25), "Text Width:", Skin.customStyles [1]);
		string newW = GUI.TextField (new Rect (226, y, 150, 25), TextWidth.ToString ());
		int.TryParse (newW, out  TextWidth);

		y += 30;
		LtrContext = GUI.Toggle (new Rect (222, y, 25, 25), LtrContext, "");
		GUI.Label (new Rect (240, y, 400, 25), " Main context is in English", Skin.customStyles [1]);

	}

	private void UpdateRTLText ()
	{
		/*********************************************************************************************************************************
              * Different methods to get the desired RTL text :
              * 
              * 1 - convertedText = RTL.Convert(InputText);
              *     Get RTL text (no word-wrapping)
              *     
              *     
              * 2 - convertedText = RTL.ConvertWordWrap(InputText, width , rtlStyle , RTL.NumberFormat.KeepOriginalFormat, isLTRContext);
              *     Get RTL wordwrapped text (word-wrapping)
              *     
         ***********************************************************************************************************************************/


		//Remember to set wordWrap property to true if you're using ConvertWordWrap() method!
		//Skin.customStyles [0].wordWrap = true;

		//RTL conversion happens here :
		convertedText = RTL.ConvertWordWrap (InputText, TextWidth, Skin.customStyles [0], NumberFormat, LtrContext);

		// Update states
		prevWidth = TextWidth;
		prevInputText = InputText;
		prevCheckboxState = LtrContext;
	}
}
