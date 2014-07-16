using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuStepGames : IMenuStep 
{
	protected Button exitButton = null;

	public void SetupLocal()
	{
		if (exitButton == null)
			exitButton = transform.FindChild("ButtonExit").GetComponent<Button>();
		
		if (exitButton == null)
			Debug.LogError("MenuStepGames: Missing exit button.");
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

		LugusDebug.debug = true;
	
	}
	
	public override void Deactivate (bool animate)
	{
		activated = false;

		this.gameObject.SetActive(false);

		LugusDebug.debug = false;
	}
}
