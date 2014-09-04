using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuStepLogin : IMenuStep 
{
	protected EditableTextMesh usernameField = null;
	protected EditableTextMesh passwordField = null;
	protected TextMeshWrapper usernameDisplay = null;
	protected Button confirmButton = null;
	protected SpriteRenderer confirmSprite = null;
	protected Button registerButton = null;
	protected Button exitButton = null;
	protected Button changeAccountButton = null;
	protected bool locked = false;

	public void SetupLocal()
	{
//		if (exitButton == null)
//			exitButton = transform.FindChild("ButtonExit").GetComponent<Button>();
//		
//		if (exitButton == null)
//			Debug.LogError("MenuStepSettings: Missing exit button.");

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
		else
		{
			confirmSprite = confirmButton.GetComponent<SpriteRenderer>();
		}

		if (registerButton == null)
		{
			registerButton = transform.FindChild("ButtonRegister").GetComponent<Button>();
		}

		if (registerButton == null)
		{
			Debug.LogError(transform.Path() + " missing register button");
		}

		if (usernameDisplay == null)
		{
			usernameDisplay = transform.FindChild("AccountDisplayName").GetComponent<TextMeshWrapper>();
		}

		if (usernameDisplay == null)
		{
			Debug.LogError(transform.Path() + " missing user name display.");
		}

		if (exitButton == null)
			exitButton = transform.FindChild("ButtonExit").GetComponent<Button>();
		
		if (exitButton == null)
			Debug.LogError("MenuStepSettings: Missing exit button."); 


		if (changeAccountButton == null)
		{
			changeAccountButton = transform.FindChild("ButtonChangeAccount").GetComponent<Button>();
		}
		
		if (changeAccountButton == null)
		{
			Debug.LogError(transform.Path() + " missing change account");
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
		 	LugusCoroutines.use.StartRoutine(Login());
		}
		else if (registerButton.pressed)
		{
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Register);
		}
		else if (exitButton.pressed)
		{
			if (string.IsNullOrEmpty(LugusConfig.use.System.GetString("KBUsername", "")))
			{
				LugusConfig.use.System.SetBool("KBPlayOffline", true, true);
				MainMenuManager.use.SetLoginMessage(LugusResources.use.Localized.GetText("global.connection.offline"));
			}

			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Main);
		}
		else if (changeAccountButton.pressed)		
		{
			LugusConfig.use.System.SetBool("KBPlayOffline", false, true);

			LugusConfig.use.System.SetString("KBUsername", "", true);
			LugusConfig.use.System.SetString("KBPassword", "", true);

			PlayerAuthCrossSceneInfo.use.loggedIn = false;

			MainMenuManager.use.SetLoginMessage(LugusResources.use.Localized.GetText("global.connection.offline"));

			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Login);
		}
	}

	protected IEnumerator Login()
	{
		if (KBAPIConnection.use.loggingIn)
			yield break;

		locked = true;

		KBAPIConnection.use.Login(usernameField.GetEnteredString(), passwordField.GetEnteredString());

		while(KBAPIConnection.use.loggingIn)
			yield return new WaitForEndOfFrame();

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
			PlayerAuthCrossSceneInfo.use.loggedIn = true;

			LugusConfig.use.System.SetBool("KBPlayOffline", false, true);

			LugusConfig.use.System.SetString("KBUsername", usernameField.GetEnteredString(), true);
			LugusConfig.use.System.SetString("KBPassword", passwordField.GetEnteredString(), true);
			
			MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Main);

			MainMenuManager.use.SetLoginMessage(usernameField.GetEnteredString());
		}

		locked = false;
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


		if (PlayerAuthCrossSceneInfo.use.loggedIn)
		{
			usernameField.gameObject.SetActive(false);
			passwordField.gameObject.SetActive(false);
			confirmButton.gameObject.SetActive(false);
			registerButton.gameObject.SetActive(false);

			usernameDisplay.gameObject.SetActive(true);
			changeAccountButton.gameObject.SetActive(true);

			transform.FindChild("Title").gameObject.SetActive(false);
			transform.FindChild("TitleLoggedIn").gameObject.SetActive(true);

			usernameDisplay.SetText(LugusConfig.use.System.GetString("KBUsername", ""));
		}
		else
		{
			usernameField.gameObject.SetActive(true);
			passwordField.gameObject.SetActive(true);
			confirmButton.gameObject.SetActive(true);
			registerButton.gameObject.SetActive(true);

			usernameDisplay.gameObject.SetActive(false);
			changeAccountButton.gameObject.SetActive(false);

			transform.FindChild("Title").gameObject.SetActive(true);
			transform.FindChild("TitleLoggedIn").gameObject.SetActive(false);

			usernameField.Reset();
			passwordField.Reset();
		}
	}
	
	public override void Deactivate (bool animate)
	{
		activated = false;
		this.gameObject.SetActive(false);
	}
}
