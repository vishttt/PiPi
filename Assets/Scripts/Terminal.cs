/// Enable this in debug builds, uncomment in release builds

#define SWI_DEBUG



using UnityEngine;

using System.Collections.Generic;



/// <summary>

/// Put this MonoBehaviour in your scene to get a scrollable debug terminal overlay.

/// Debug.Log, Debug.LogWarning, and Debug.LogError automatically outputs to this terminal

/// as well as the built-in Unity terminal.

/// </summary>

/// <remarks>

/// Can be toggled with F12.

/// Don't call Debug.Log before Awake unless you have set this behaviour first in the execution order.

/// </remarks>

class Terminal : MonoBehaviour
{

	#region Public fields



	public int TotalLines = 10000;

	public int LinesDisplayed = 40;

	public int FontSize = 10;

	public bool IsVisible = true;

	public KeyCode ToggleKey = KeyCode.F12;

	public Color LogColor = Color.white;

	public Color WarningColor = new Color (0.8f, 0.8f, 0.0f);

	public Color ErrorColor = new Color (1.0f, 0.5f, 0.5f);

	public Color ExceptionColor = new Color (1.0f, 0.5f, 1.0f);

	public float ErrorShake_px = 10f;



	#endregion



	#region Private fields

    

	struct LogEntry
	{

		public string text;

		public LogType type;

	}

	int Offset = 0;

	LinkedList<LogEntry> logEntries;

	static Terminal instance = null;



	#if SWI_DEBUG

	GUIStyle style;

	#endif



	#endregion



	#region Public methods

    

	/// <summary>

	/// Print text to debug terminal.

	/// </summary>

	/// <note>Don't call before Start phase - it will crash. </note>

	/// <note>Is disabled in non-debug versions. </note>

	public static void Print (string text)
	{

		instance.addLogEntries (text, LogType.Log);

	}

	#endregion



	#region Private methods



	/// <summary>

	/// Callback for Unity Debug logging system.

	/// </summary>

	void handleLog (string logString, string stackTrace, LogType type)
	{

		addLogEntries (logString, type);

	}



	bool isOverflowed = false;



	void addLogEntries (string text, LogType type)
	{

		string[] lines = text.Split ('\n');

		bool scroll = false;



		/* example: entries count = 2, lines displayed = 2, offset = 0, adding one line

         * 

         * line 0  \_ shown

         * line 1  /  on screen

         * 

         * if offset >= count - lines displayed, scroll by adding 1

         * if 0 = 2 - 2, offset = 1

         */

		if (Offset >= MaxOffset) {

			scroll = true;

		}

		foreach (string line in lines) {

			if (logEntries.Count >= TotalLines) {

				logEntries.RemoveFirst ();

				if (!isOverflowed)
					isOverflowed = true;

			} else if (scroll) {

				Offset += 1;

			}

			logEntries.AddLast (new LogEntry { text = line, type = type });

		}

		Offset = Mathf.Clamp (Offset, 0, MaxOffset);

	}

	#endregion



	#region MonoBehaviour



	void Awake ()
	{

		instance = this;

#if SWI_DEBUG

		logEntries = new LinkedList<LogEntry> ();



#if ! (UNITY_IPHONE && UNITY_ANDROID)

		style = new GUIStyle ();

		style.fontSize = FontSize;

		style.normal.textColor = new Color (0.4f, 0.4f, 1.0f);

#endif

		//Print("==============================================================");

		//Print("   SWI Common: debug/Terminal.cs\n");

		//Print("   Output using Debug.Log, Debug.LogWarning, and Debug.LogError.");

		//Print("   [F12] : toggle log");

		//Print("   [PageUp/PageDown] : scroll");

		//Print("   [Home/End] : top/bottom");

		//Print("==============================================================\n");

		Application.logMessageReceived += handleLog;

		//Application.RegisterLogCallback(handleLog);

#endif

	}





	#if SWI_DEBUG

	void Update ()
	{

		// Should be checked every frame to avoid missing the keydown frame.

		if (Input.GetKeyDown (ToggleKey))
			IsVisible = !IsVisible;



		if (Input.GetKey (KeyCode.Home)) {
			Offset = 0;
		}

		if (Input.GetKey (KeyCode.PageDown)) {
			Offset += 3;
		}

		if (Input.GetKey (KeyCode.PageUp)) {
			Offset -= 3;
		}

		if (Input.GetKey (KeyCode.End)) {
			Offset = MaxOffset;
		}



		Offset = Mathf.Clamp (Offset, 0, MaxOffset);

	}



	private int MaxOffset { get { return Mathf.Max (0, logEntries.Count - LinesDisplayed); } }



	void OnGUI ()
	{

		if (IsVisible) {

			// read offset from scroll bar

			//GUI.VerticalScrollbar(new Rect(Screen.width - 24, 5, 20, Screen.height - 10),

			//  Offset, 0.5f, 0f, Mathf.Max(0f, logEntries.Count - LinesDisplayed));



			GUI.Box (new Rect (0, 0, Screen.width, Screen.height), "Debug Messages"

			+ (isOverflowed ? "[Overflow]" : ""));

			int line = 0;

			float xOffset = 0f;



			// Find entry #Offset

			int n = 0;

			LinkedListNode<LogEntry> entry = logEntries.First;

			for (n = 0; n < Offset; n++) {
				entry = entry.Next;
			}



			for (; n < (int)Offset + LinesDisplayed && n < logEntries.Count;

                 n++, entry = entry.Next) {

				switch (entry.Value.type) {

				case LogType.Log:

					style.normal.textColor = LogColor;

					xOffset = 0f;

					break;

				case LogType.Warning:

					style.normal.textColor = WarningColor;

					xOffset = Mathf.Sin (Time.time * Mathf.PI * 0.6f) * ErrorShake_px + ErrorShake_px;

					break;

				case LogType.Error:

					style.normal.textColor = ErrorColor;

					xOffset = Mathf.Sin (Time.time * Mathf.PI * 1.2f) * ErrorShake_px + ErrorShake_px;

					break;

				case LogType.Exception:

					style.normal.textColor = ExceptionColor;

					xOffset = Mathf.Sin (Time.time * Mathf.PI * 2.4f) * ErrorShake_px + ErrorShake_px;

					break;

				default:

					style.normal.textColor = Color.white;

					break;

				}

#if UNITY_IPHONE || UNITY_ANDROID

				GUI.Label (new Rect (xOffset, line * FontSize + 20, Screen.width, Screen.height), entry.Value.text);

#else

                GUI.Label(new Rect(xOffset, line * FontSize + 20, Screen.width, Screen.height), entry.Value.text, style);

#endif

				line += 1;

			}

		}

	}

	#endif



	#endregion

}



