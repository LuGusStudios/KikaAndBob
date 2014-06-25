using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuStepAvatar : IMenuStep 
{
	protected Button exitButton = null;
	protected Button leftButton = null;
	protected Button rightButton = null;

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
			ModifyAvatar(-1)
		}
	}

	protected void ModifyAvatar(int amount)
	{

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
