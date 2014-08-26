using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuStepRegister : MenuStepMain 
{
	protected EditableTextMesh usernameField = null;
	protected EditableTextMesh passwordField = null;
	protected Button confirmButton = null;
	protected Button exitButton = null;
	protected SpriteRenderer confirmSprite = null;
	protected bool locked = false;
	
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
			Debug.LogError("MenuStepRegister: Missing confirm button.");
		}
		else
		{
			confirmSprite = confirmButton.GetComponent<SpriteRenderer>();
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
		if (locked)
			return;

		if (usernameField.IsEmpty() || passwordField.IsEmpty())
		{
			if (confirmSprite.collider.enabled)
			{
				confirmSprite.color = confirmSprite.color.a (0.5f);
				confirmSprite.collider.enabled = false;
			}
		}
		else
		{
			if (!confirmSprite.collider.enabled)
			{
				confirmSprite.color = confirmSprite.color.a (1f);
				confirmSprite.collider.enabled = true;
			}
		}

		if (confirmButton.pressed)
		{
			LugusCoroutines.use.StartRoutine(Register());
		}
		else if (exitButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Login);
		}
	}


	protected IEnumerator Register()
	{
		if (KBAPIConnection.use.loggingIn)
			yield break;

		locked = true;
		
		KBAPIConnection.use.Register(usernameField.GetEnteredString(), passwordField.GetEnteredString(), "");
		
		while(KBAPIConnection.use.loggingIn)
			yield return new WaitForEndOfFrame();

		locked = false;

		if (KBAPIConnection.use.errorMessage != "")
		{
			DialogueBox errorBox = DialogueManager.use.CreateBox(KikaAndBob.ScreenAnchor.Center, KBAPIConnection.use.errorMessage);
			errorBox.blockInput = true;
			errorBox.boxType = DialogueBox.BoxType.Continue;
			errorBox.onContinueButtonClicked += OnContinueButtonClicked;
			errorBox.Show();
		}
		else
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Main);
		}
	}

	public void OnContinueButtonClicked(DialogueBox box)
	{
		box.onContinueButtonClicked += OnContinueButtonClicked;
		box.Hide();
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
