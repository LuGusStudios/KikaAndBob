using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuStepReset : IMenuStep
{
	protected Button confirmButton = null;
	protected Button cancelButton = null;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		if (confirmButton == null)
		{
			confirmButton = transform.FindChild("ButtonConfirm").GetComponent<Button>();
		}

		if (confirmButton == null)
		{
			Debug.LogError("MenuStepReset: Missing confirm button.");
		}

		if (cancelButton == null)
		{
			cancelButton = transform.FindChild("ButtonCancel").GetComponent<Button>();
		}
		
		if (cancelButton == null)
		{
			Debug.LogError("MenuStepReset: Missing cancel button.");
		}
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start() 
	{
		SetupGlobal();
	}
	
	protected void Update() 
	{
		if (confirmButton.pressed)
		{
			LugusConfig.use.User.ClearAllData();
		}
		else if (cancelButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Settings);
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
