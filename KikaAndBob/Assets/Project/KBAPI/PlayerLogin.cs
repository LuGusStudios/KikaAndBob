using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerLogin : MonoBehaviour 
{


	public void SetupLocal()
	{

	}
	
	public void SetupGlobal()
	{
		LugusCoroutines.use.StartRoutine(GameStartRoutine());
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
	
	}

	
	protected IEnumerator GameStartRoutine()
	{				
		if (PlayerAuthCrossSceneInfo.use.loggedIn == true)
		{
			yield return null;	// want to make sure MainMenu is properly initialized first
			DisplayOnline();
			Debug.Log("Already logged in.");
			yield break;
		}

		yield return null;	// delay one frame so localization etc. can be initialized properly

		string userName = LugusConfig.use.System.GetString("KBUsername", "");
		string password = LugusConfig.use.System.GetString("KBPassword", "");

		if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
		{
			Debug.Log("No local username or password.");

			if (LugusConfig.use.System.GetBool("KBPlayOffline", false) == true)
			{
				Debug.Log("Player declined to create an account before.");
				DisplayOffline();
				yield break;
			}
			else
			{
				Debug.Log("Asking if player wants to make an account.");
				yield return StartCoroutine(AskForAuthRoutine());
			}
		}
		else
		{
			yield return LugusCoroutines.use.StartRoutine(KBAPIConnection.use.CheckConnectionRoutine());

			if (KBAPIConnection.use.errorMessage != "")
			{ 
				DisplayOffline();
				yield break;
			}

			yield return LugusCoroutines.use.StartRoutine(KBAPIConnection.use.LoginRoutine(userName, password));

			if (KBAPIConnection.use.errorMessage == "")
			{
				Debug.Log("Login succeeded.");
				
				PlayerAuthCrossSceneInfo.use.loggedIn = true;

				DisplayOnline();
				
				yield break;
			}
			else
			{
				DialogueBox errorBox = DialogueManager.use.CreateBox(KikaAndBob.ScreenAnchor.Center, KBAPIConnection.use.errorMessage);
				errorBox.blockInput = true;
				errorBox.boxType = DialogueBox.BoxType.Continue;
				errorBox.onContinueButtonClicked += OnContinueButtonClicked;
				errorBox.Show();
			}
		}
	}

	protected IEnumerator AskForAuthRoutine()
	{
		yield return StartCoroutine(SetLanguageRoutine());


		DialogueBox authorizeBox = DialogueManager.use.CreateBox(KikaAndBob.ScreenAnchor.Center, LugusResources.use.GetText("global.authmessage"));
		authorizeBox.boxType = DialogueBox.BoxType.ConfirmCancel;
		authorizeBox.onConfirmButtonClicked += OnAuthConfirmButtonClicked;
		authorizeBox.onCancelButtonClicked += OnAuthCancelButtonClicked;
		authorizeBox.blockInput = true;
		authorizeBox.Show();

		
		yield break;
	}

	protected IEnumerator SetLanguageRoutine()
	{
		MainMenuManager.MainMenuTypes previousMenu = MainMenuManager.use.currentMenu;

		MenuStepLanguage langMenu = MainMenuManager.use.transform.FindChild("Language").GetComponent<MenuStepLanguage>();
		langMenu.doNotHighlightCurrent = true;

		GameObject exitButton = langMenu.transform.FindChild("ButtonExit").gameObject;
		exitButton.SetActive(false);

		languageChanged = false;

		MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Language);

		LugusResources.use.Localized.onResourcesReloaded += LanguageChanged;


		while (!languageChanged)
			yield return null;


		LugusResources.use.Localized.onResourcesReloaded -= LanguageChanged;


		exitButton.SetActive(true);
		langMenu.doNotHighlightCurrent = false;

		MainMenuManager.use.ShowMenu(previousMenu);
		
		yield break;
	}

	protected bool languageChanged = false;
	protected void LanguageChanged()
	{
		languageChanged = true;
	}


	protected void DisplayOffline()
	{
		MainMenuManager.use.SetLoginMessage(LugusResources.use.Localized.GetText("global.connection.offline"));
	}

	protected void DisplayOnline()
	{
		string message = LugusConfig.use.System.GetString("KBUsername", "");

		if (string.IsNullOrEmpty(message))
		{
			Debug.LogError("Trying to display account name, but none was stored locally! This should not happen. Displaying offline text instead.");
			message = LugusResources.use.Localized.GetText("global.connection.offline");
		}

		MainMenuManager.use.SetLoginMessage(message);
	}

	protected void OnContinueButtonClicked(DialogueBox box)
	{
		box.onContinueButtonClicked += OnContinueButtonClicked;
		box.Hide();
	}

	protected void OnAuthConfirmButtonClicked(DialogueBox box)
	{
		Debug.Log("Going to login screen.");
		box.onConfirmButtonClicked -= OnAuthConfirmButtonClicked;
		box.Hide();
		
		MainMenuManager.use.ShowMenu(MainMenuManager.MainMenuTypes.Login);
	}
	
	protected void OnAuthCancelButtonClicked(DialogueBox box)
	{
		Debug.Log("Skipping authentication."); // TODO: add warning
		box.onCancelButtonClicked -= OnAuthCancelButtonClicked;
		box.Hide();

		DisplayOffline();

		LugusConfig.use.System.SetBool("KBPlayOffline", true, true);	// this will prevent the game from asking if you want to log in next time
	}

}
