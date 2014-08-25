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
		DialogueBox authorizeBox = DialogueManager.use.CreateBox(KikaAndBob.ScreenAnchor.Center, LugusResources.use.GetText("global.authmessage"));
		authorizeBox.boxType = DialogueBox.BoxType.ConfirmCancel;
		authorizeBox.onConfirmButtonClicked += OnAuthConfirmButtonClicked;
		authorizeBox.onCancelButtonClicked += OnAuthCancelButtonClicked;
		authorizeBox.blockInput = true;
		authorizeBox.Show();

		
		yield break;
	}

	protected void DisplayOffline()
	{

	}

	protected void DisplayOnline()
	{
		
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

		LugusConfig.use.System.SetBool("KBPlayOffline", true, true);	// this will prevent the game from asking if you want to log in next time
	}

}
