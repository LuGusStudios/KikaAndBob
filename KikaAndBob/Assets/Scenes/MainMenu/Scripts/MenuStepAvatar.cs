using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class MenuStepAvatar : IMenuStep 
{
	protected Button exitButton = null;
	protected Button leftButton = null;
	protected Button rightButton = null;
	protected BoneAnimation character = null;
	protected List<string> catVariations = new List<string>();
	protected int currentCatIndex = 0;

	public void SetupLocal()
	{
		if (exitButton == null)
			exitButton = transform.FindChild("ButtonExit").GetComponent<Button>();
		
		if (exitButton == null)
			Debug.LogError("MenuStepAvatar: Missing exit button.");

		if (leftButton == null)
			leftButton = transform.FindChild("ButtonLeft").GetComponent<Button>();
		
		if (leftButton == null)
			Debug.LogError("MenuStepAvatar: Missing left button.");

		if (rightButton == null)
			rightButton = transform.FindChild("ButtonRight").GetComponent<Button>();
		
		if (rightButton == null)
			Debug.LogError("MenuStepAvatar: Missing right button.");

		if (character == null)
			character = GetComponentInChildren<BoneAnimation>();

		if (character == null)
		{
			Debug.LogError("MenuStepAvatar: Missing character.");
		}
		else
		{
			if (catVariations.Count <= 0)
			{
				foreach(SmoothMoves.AnimationClipSM_Lite clip in character.mAnimationClips)
				{
					if (clip.animationName.Contains("Idle") && !clip.animationName.Contains("Tiger"))
					{
						catVariations.Add(clip.animationName);
					}
				}
			}
		}

	}
	
	public void SetupGlobal()
	{
		ModifyAvatar(0);
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
		if (exitButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Main);
		}

		if (rightButton.pressed)
		{
			ModifyAvatar(1);
		}
		else if (leftButton.pressed)
		{
			ModifyAvatar(-1);
		}
	}

	protected void ModifyAvatar(int amount)
	{
		currentCatIndex += amount;

		if (currentCatIndex >= catVariations.Count)
		{
			currentCatIndex = 0;
		}
		else if (currentCatIndex < 0)
		{
			currentCatIndex = catVariations.Count - 1;
		}

		character.Play(catVariations[currentCatIndex]);
	}

	public override void Activate (bool animate)
	{
		activated = true;
		this.gameObject.SetActive(true);
	}
	
	public override void Deactivate (bool animate)
	{
		activated = false;
		this.gameObject.SetActive(false);
	}
}
