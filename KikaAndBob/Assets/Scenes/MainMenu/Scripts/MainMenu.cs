using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour 
{
	public List<string> levelNames = new List<string>();
	protected bool loading = false;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
//		if (Input.GetKeyDown(KeyCode.L))
//		{
//			string result = "";
//			foreach (string s in levelNames)
//			{
//				result += "levels.Add(\"" + s + "\");\n";
//			}
//			Debug.Log(result);
//		}
	}
	Vector2 scroll = Vector2.zero;
	protected void OnGUI()
	{
		if (loading)
			return;

		scroll = GUILayout.BeginScrollView(scroll, GUILayout.MinWidth(180));
		foreach(string s in levelNames)
		{
			if (GUILayout.Button(s, new GUILayoutOption[]{GUILayout.Width(150), GUILayout.Height(50)})    )
			{
				LugusCoroutines.use.StartRoutine(LoadRoutine(s));
			}
		}
		GUILayout.EndArea();
	}

	protected IEnumerator LoadRoutine(string levelName)
	{
		loading = true;

		ScreenFader.use.FadeOut(0.5f);

		yield return new WaitForSeconds(0.5f);

		Resources.UnloadUnusedAssets();

		Application.LoadLevel(levelName);

		yield break;
	}

}
