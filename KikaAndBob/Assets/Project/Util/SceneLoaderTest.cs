using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneLoaderTest : MonoBehaviour {


	protected bool automate = false;
	protected int currentIndex= 0;
	protected List<string> scenes = new List<string>();
	protected float timer = 0;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);


		MainMenu mainMenuDebug = FindObjectOfType<MainMenu>();

		scenes = mainMenuDebug.levelNames;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (automate)
		{
			timer += Time.deltaTime;

			if (timer > 10)
			{
				currentIndex = Random.Range (0, scenes.Count);
				LoadScene(currentIndex);
				timer = 0;
				
			}
		}
		else 
		{
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
		Application.LoadLevel(scenes[index]);
	}

	protected void OnGUI()
	{
		if (!LugusDebug.debug)
			return;

		GUILayout.BeginHorizontal();

		GUILayout.Space(400);

		if (GUILayout.Button("Cycle scenes: " + automate.ToString()))
		{
			automate = !automate;
		}

		GUILayout.EndHorizontal();
//
//		automate = GUILayout.Toggle(automate, "Automate");
	}
	
}
