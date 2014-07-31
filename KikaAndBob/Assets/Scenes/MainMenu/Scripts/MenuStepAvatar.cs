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
	protected int currentCatIndex = 1;
	protected EditableTextMesh characterNameField = null;

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

		if (characterNameField == null)
		{
			characterNameField = GetComponentInChildren<EditableTextMesh>();
		}

		if (characterNameField == null)
		{
			Debug.LogError("MenuStepAvatar: Missing character name field.");
		}

	}
	
	public void SetupGlobal()
	{
		currentCatIndex = LugusConfig.use.User.GetInt("CatIndex", 1);
		
		Debug.Log("MenuStepAvatar: Current cat index = " + currentCatIndex.ToString());
		
		string catName = LugusConfig.use.User.GetString("CatName", "");
		
		if (!string.IsNullOrEmpty(catName))
		{
			Debug.Log("MenuStepAvatar: Setting player name: " + catName);
			characterNameField.SetEnteredString(catName);
		}

		PlayIdleAnim(currentCatIndex);
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
			LugusConfig.use.User.SetInt("CatIndex", currentCatIndex, true);
			Debug.Log("MenuStepAvatar: Saved cat index: " + currentCatIndex.ToString());

			if (!string.IsNullOrEmpty(characterNameField.GetEnteredString()) && !characterNameField.IsDefaultValue())
			{
				LugusConfig.use.User.SetString("CatName", characterNameField.GetEnteredString(), true);
				Debug.Log("MenuStepAvatar: Saved cat name: " + characterNameField.GetEnteredString());
			}
			else
			{
				Debug.Log("MenuStepAvatar: Cat name was not yet entered. Not saving it.");
			}

			LugusConfig.use.SaveProfiles();

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

		if (currentCatIndex >= 5)
		{
			currentCatIndex = 1;
		}
		else if (currentCatIndex < 1)
		{
			currentCatIndex = 4;
		}

		PlayIdleAnim(currentCatIndex);
	}

	protected void PlayIdleAnim(int index)
	{
		character.Play("Cat0" + index.ToString() + "Side_Idle");
	}

	public override void Activate (bool animate)
	{
		activated = true;
		this.gameObject.SetActive(true);

		currentCatIndex = LugusConfig.use.User.GetInt("CatIndex", 1);
		
		Debug.Log("MenuStepAvatar: Current cat index = " + currentCatIndex.ToString());
		
		string catName = LugusConfig.use.User.GetString("CatName", "");
		
		if (!string.IsNullOrEmpty(catName))
		{
			Debug.Log("MenuStepAvatar: Setting player name: " + catName);
			characterNameField.SetEnteredString(catName);
		}
		else
		{
			characterNameField.Reset();
		}
		
		
		PlayIdleAnim(currentCatIndex);
	}
	
	public override void Deactivate (bool animate)
	{
		activated = false;
		this.gameObject.SetActive(false);
	}
}
