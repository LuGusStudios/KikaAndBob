using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuStepSettings : IMenuStep 
{
	Button exitButton = null;
	Button musicButton = null;
	Button soundButton = null;

	public void SetupLocal()
	{
		if (exitButton == null)
			exitButton = transform.FindChild("ButtonExit").GetComponent<Button>();
		
		if (exitButton == null)
			Debug.LogError("MenuStepSettings: Missing exit button.");


		if (musicButton == null)
			musicButton = transform.FindChild("ButtonMusic").GetComponent<Button>();
		
		if (musicButton == null)
			Debug.LogError("MenuStepSettings: Missing music button.");


		if (soundButton == null)
			soundButton = transform.FindChild("ButtonSound").GetComponent<Button>();
		
		if (soundButton == null)
			Debug.LogError("MenuStepSettings: Missing sound button.");
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
		if (exitButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Main);
		}
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
