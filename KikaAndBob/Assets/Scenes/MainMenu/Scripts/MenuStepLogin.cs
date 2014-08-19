using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuStepLogin : MenuStepMain 
{
	protected EditableTextMesh usernameField = null;
	protected EditableTextMesh passwordField = null;
	protected Button confirmButton = null;
	protected Button registerButton = null;
	protected Button exitButton = null;

	public void SetupLocal()
	{
		if (exitButton == null)
			exitButton = transform.FindChild("ButtonExit").GetComponent<Button>();
		
		if (exitButton == null)
			Debug.LogError("MenuStepSettings: Missing exit button.");

		if (usernameField == null)
		{
			usernameField = transform.FindChild("TextFieldUsername").GetComponent<EditableTextMesh>();
		}
		
		if (passwordField == null)
		{
			passwordField = transform.FindChild("TextFieldPassword").GetComponent<EditableTextMesh>();
		}

		if (confirmButton == null)
		{
			confirmButton = transform.FindChild("ButtonConfirm").GetComponent<Button>();
		}

		if (confirmButton == null)
		{
			Debug.LogError(transform.Path() + " missing confirm button");
		}

		if (registerButton == null)
		{
			registerButton = transform.FindChild("ButtonRegister").GetComponent<Button>();
		}

		if (registerButton == null)
		{
			Debug.LogError(transform.Path() + " missing register button");
		}

	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
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
			// authenticate, then decide if authentication was succesful
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Main);
		}
		else if (registerButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Register);
		}
		else if (exitButton.pressed)
		{
			DialogueBox confirmExitBox = DialogueManager.use.CreateBox(KikaAndBob.ScreenAnchor.Center, LugusResources.use.GetText("global.connection.playoffline"));
			// CONTINUE HERE
		}
	}

	public override void Activate (bool animate)
	{
		activated = true;
		this.gameObject.SetActive(true);

		// TODO: add check for already authenticated (hide register button etc. maybe add log out button)
		
	}
	
	public override void Deactivate (bool animate)
	{
		activated = false;
		this.gameObject.SetActive(false);
	}
}
