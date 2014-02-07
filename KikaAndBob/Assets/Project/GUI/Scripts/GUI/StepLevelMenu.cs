﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StepLevelMenu : IMenuStep 
{
	protected Transform levelBarsParent = null;
	protected List<Transform> levelBars = new List<Transform>();
	protected List<Button> levelButtons = new List<Button>();
	protected Button buttonRight = null;
	protected Button buttonLeft = null;
	protected Button buttonLeave = null;
	protected Vector2 centerScreen =  new Vector2(10.24f, 7.68f);
	protected ILugusCoroutineHandle moveRoutine = null;
	protected float offScreenDistance = 20.0f;
	protected int pageCounter = 0;

	public void SetupLocal()
	{
		if (levelBarsParent == null)
		{
			levelBarsParent = transform.FindChild("LevelBars");
		}
		if (levelBarsParent == null)
		{
			Debug.LogError("StepLevelMenu: Missing level bars transform!");
		}

		for (int i = 1; i <= 5; i++) 
		{
			Transform t = levelBarsParent.transform.FindChild("LevelBar0" + i);

			if (t != null)
			{
				levelBars.Add(t);
				levelButtons.Add(t.FindChild("ButtonPlay").GetComponent<Button>());
			}
		}

		if (buttonLeft == null)
		{
			buttonLeft = transform.FindChild("ButtonLeft").GetComponent<Button>();
		}
		if (buttonLeft == null)
		{
			Debug.LogError("StepLevelMenu: Missing left button.");
		}

		if (buttonRight == null)
		{
			buttonRight = transform.FindChild("ButtonRight").GetComponent<Button>();
		}
		if (buttonRight == null)
		{
			Debug.LogError("StepLevelMenu: Missing right button.");
		}

		if (buttonLeave == null)
		{
			buttonLeave = transform.FindChild("ButtonLeave").GetComponent<Button>();
		}
		if (buttonLeave == null)
		{
			Debug.LogError("StepLevelMenu: Missing leave button.");
		}
	}
	
	public void SetupGlobal()
	{
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
		if (!activated)
			return;

		for (int i = 0; i < levelButtons.Count; i++) 
		{
			if (levelButtons[i].pressed)
			{
				Debug.Log("StepLevelMenu: Pressed button: " + i);
				// TO DO: Set relevant level data here. Index = (pageCounter * 5) + i;
				// Get right CrossSceneInfo for minigame type
				// Then: reload scene!
			}
		}

		if (buttonLeft.pressed)
		{
			if (pageCounter >= 1)
			{
				--pageCounter;
			}

			if (moveRoutine != null && moveRoutine.Running)
			{
				moveRoutine.StopRoutine();
			}

			LugusCoroutines.use.StartRoutine(SwitchPages(false));
		}

		if (buttonRight.pressed)
		{
			if (pageCounter < 1)		// TO DO: Figure out the maximum here based on nr of levels. Now set to 1 max.
			{
				++pageCounter;
			}

			if (moveRoutine != null && moveRoutine.Running)
			{
				moveRoutine.StopRoutine();
			}

			LugusCoroutines.use.StartRoutine(SwitchPages(true));
		}

		if (buttonLeave.pressed)
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.GameMenu);
		}
	}

	public override void Activate()
	{
		activated = true;
		gameObject.SetActive(true);
		FlyIn(true);
		LoadLevelData();
	}

	public override void Deactivate()
	{
		activated = false;
		gameObject.SetActive(false);
	}

	protected void GetCrossSceneInfo()	// TO DO: this will return CrossSceneInfo for relevant game. Will probably be moved elsewhere.
	{
	}

	protected void LoadLevelData()
	{
		// TO DO: Set data about levels here (name, description, etc.)
	}

	protected void EnableBars(int amount)
	{
		for (int i = 0; i < levelBars.Count; i++) 
		{
			if (i < amount)
				levelBars[i].gameObject.SetActive(true);
			else
				levelBars[i].gameObject.SetActive(false);
		}
	}

	protected IEnumerator SwitchPages(bool toRight)
	{
		FlyOut(toRight);

		yield return new WaitForSeconds(0.51f);	// a little longer than the iTween animation to prevent double iTweens

		FlyIn(toRight);
	}

	protected void FlyOut(bool toRight)
	{
		float delay = 0.0f;
		foreach(Transform t in levelBars)
		{
			iTween.Stop(t.gameObject);

			float nextPosition = 0;

			if (toRight)
				nextPosition = centerScreen.x - offScreenDistance;
			else
				nextPosition = centerScreen.x + offScreenDistance;

			t.gameObject.MoveTo(t.transform.localPosition.x(nextPosition)).Time(0.5f).IsLocal(true).Delay(delay).EaseType(iTween.EaseType.easeOutBack).Execute();
			delay += 0.1f;
		}
	}

	protected void FlyIn(bool toRight)
	{
		float delay = 0.0f;
		foreach(Transform t in levelBars)
		{
			iTween.Stop(t.gameObject);
			if (toRight)
				t.transform.localPosition = t.transform.localPosition.x(centerScreen.x + offScreenDistance);
			else
				t.transform.localPosition = t.transform.localPosition.x(centerScreen.x - offScreenDistance);
			
			t.gameObject.MoveTo(t.transform.localPosition.x(centerScreen.x)).Time(0.5f).IsLocal(true).Delay(delay).EaseType(iTween.EaseType.easeOutBack).Execute();
			delay += 0.1f;
		}
	}
}
