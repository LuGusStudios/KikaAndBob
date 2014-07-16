using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceLogVisualizer : LugusSingletonExisting<CatchingMiceLogVisualizerDefault>
{

}

public class CatchingMiceLogVisualizerDefault : MonoBehaviour {

	public bool isVisible = true;
	public LogLevel logLevel = LogLevel.LOG;

	public enum LogLevel
	{
		LOG = 1,
		WARNING = 2,
		ERROR = 3,
		NONE = -1
	}

	protected int msgIndex = -1;
	protected List<Pair<LogLevel, string>> messages = new List<Pair<LogLevel, string>>();
	
	public void Log(string message, bool toConsole = true)
	{
		if (logLevel <= LogLevel.LOG)
		{
			messages.Add(new Pair<LogLevel, string>(LogLevel.LOG, message));
			msgIndex = messages.Count - 1;
		}

		if (toConsole)
		{
			Debug.Log(message);
		}
	}

	public void LogWarning(string message, bool toConsole = true)
	{
		if (logLevel <= LogLevel.WARNING)
		{
			messages.Add(new Pair<LogLevel, string>(LogLevel.WARNING, message));
			msgIndex = messages.Count - 1;
		}

		if (toConsole)
		{
			Debug.LogWarning(message);
		}
	}

	public void LogError(string message, bool toConsole = true)
	{
		if (logLevel <= LogLevel.ERROR)
		{
			messages.Add(new Pair<LogLevel, string>(LogLevel.ERROR, message));
			msgIndex = messages.Count - 1;
		}

		if (toConsole)
		{
			Debug.LogError(message);
		}
	}

	public virtual void SetupLocal()
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		isVisible = true;
#else
		isVisible = false;
#endif
	}

	public virtual void SetupGlobal()
	{

	}

	protected void Awake()
	{

	}

	protected void Start()
	{

	}

	protected void OnGUI()
	{
		if ((msgIndex < 0) || !isVisible)
		{
			return;
		}

		// Next and previous buttons to scroll between messages
		GUILayout.BeginArea(new Rect(10, Screen.height - 60, 100, 50));
		GUILayout.BeginVertical();
		if (messages.Count > 1)
		{
			if (msgIndex == 0)
			{
				GUILayout.Space(25);
				if (GUILayout.Button("Next"))
				{
					++msgIndex;
				}
			}
			else if (msgIndex == (messages.Count - 1))
			{
				if (GUILayout.Button("Back"))
				{
					--msgIndex;
				}
				GUILayout.Space(25);
			}
			else
			{
				if (GUILayout.Button("Back"))
				{
					--msgIndex;
				}
				if (GUILayout.Button("Next"))
				{
					++msgIndex;
				}
			}
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();

		// Textbox visualization
		GUILayout.BeginArea(new Rect(120, Screen.height - 60, Screen.width - 130, 50));
		GUILayout.BeginVertical();

		string message = "(" + (msgIndex + 1).ToString() + "/" + messages.Count.ToString() + ") " + messages[msgIndex].Second;

		Color currentColor = GUI.contentColor;
		
		switch(messages[msgIndex].First)
		{
			case LogLevel.LOG:
				GUI.contentColor = Color.white;
				break;
			case LogLevel.WARNING:
				GUI.contentColor = Color.yellow;
				break;
			case LogLevel.ERROR:
				GUI.contentColor = Color.red;
				break;
		}
		GUILayout.Box(message, GUILayout.ExpandHeight(true));

		GUI.contentColor = currentColor;

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
