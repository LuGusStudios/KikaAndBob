using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayExitButton : MonoBehaviour 
{
	protected Button button;
	protected bool leavingScene = false;

	public void SetupLocal()
	{
		if (button == null)
			button = GetComponent<Button>();
		
		if (button == null)
			Debug.LogError("PlayExitButton: Missing button.");
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Update()
	{
		if (!leavingScene)
		{
			if (button.pressed)
			{
				StartCoroutine(LeaveRoutine());
			}
		}
	}

	protected IEnumerator LeaveRoutine()
	{
		leavingScene = true;

		ScreenFader.use.FadeOut(0.5f);

		yield return new WaitForSeconds(0.5f);

		Application.LoadLevel("MainMenu");

		yield break;
	}

}
