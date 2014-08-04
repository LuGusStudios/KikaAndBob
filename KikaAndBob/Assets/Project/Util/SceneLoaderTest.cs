using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneLoaderTest : MonoBehaviour {

	public float sceneCycleTime = 5;
	public bool automate = false;
	protected int currentIndex= 0;
	public List<string> scenes = new List<string>();
	protected float timer = 0;
	protected int loadCounter = 0;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);

		scenes.Add("e01_kenia");
		scenes.Add("e02_argentina");
		scenes.Add("e03_china");
		scenes.Add("e04_tasmania");
		scenes.Add("e05_Mexico");
		scenes.Add("e06_egypt");
		scenes.Add("e07_france");
		scenes.Add("e08_texas");
		scenes.Add("e09_Brazil");
		scenes.Add("e10_Swiss");
		scenes.Add("e11_vatican");
		scenes.Add("e12_newyork");
		scenes.Add("e13_pacific");
		scenes.Add("e14_buthan");
		scenes.Add("e15_india");
		scenes.Add("e16_israel");
		scenes.Add("e17_greenland");
		scenes.Add("e18_amsterdam");
		scenes.Add("e19_illinois");
		scenes.Add("e20_morocco");
		scenes.Add("e21_cuba");
		scenes.Add("e22_russia");
		scenes.Add("e23_england");
		scenes.Add("e24_japan");
		scenes.Add("e25_sicily");
		scenes.Add("e26_belgium");
		scenes.Add("e00_catchingmice");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (automate)
		{
			timer += Time.deltaTime;

			if (timer >= sceneCycleTime)
			{
				loadCounter ++;
				Debug.Log("Loading next scene automatically. Scenes loaded in a row:" + loadCounter);
				currentIndex = Random.Range (0, scenes.Count);
				LoadScene(currentIndex);
				timer = 0;
				
			}
		}
		else 
		{
			if (loadCounter > 0)
				loadCounter = 0;

			if (LugusInput.use.KeyDown(KeyCode.L))
			{
				LoadScene(currentIndex);

				currentIndex++;
				if (currentIndex >= scenes.Count)
					currentIndex = 0;
			}
		}
	}

	protected void LoadScene(int index)
	{
		StartCoroutine(LoadSceneRoutine(index));
	}

	protected IEnumerator LoadSceneRoutine(int index)
	{
		ScreenFader.use.FadeOut(0.5f);
		yield return new WaitForSeconds(0.5f);
		Resources.UnloadUnusedAssets();
		Application.LoadLevel(scenes[index]);
	}

	protected void OnGUI()
	{
		if (!LugusDebug.debug)
			return;

		GUILayout.BeginHorizontal();

		GUILayout.Space(300);

		if (GUILayout.Button("Cycle scenes: " + automate.ToString(), GUILayout.MinHeight(60)))
		{
			automate = !automate;
		}

		GUILayout.EndHorizontal();
//
//		automate = GUILayout.Toggle(automate, "Automate");
	}
	
}
