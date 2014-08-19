using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAuthorization : LugusSingletonRuntime<PlayerAuthorization>
{
	protected Transform checkingConnectionIcon = null;
	protected Transform connectionScreen = null;
	protected bool testingConnection = false;

	public void SetupLocal()
	{
		DontDestroyOnLoad(this.gameObject);

		if (connectionScreen == null)
		{
			connectionScreen = transform.FindChild("ConnectionScreen");
		}
		
		if (connectionScreen == null)
		{
			Debug.LogError(transform.Path() +": missing connection screen.");
		}

		if (checkingConnectionIcon == null)
		{
			checkingConnectionIcon = connectionScreen.FindChild("CheckingConnectionIcon");
		}
		
		if (checkingConnectionIcon == null)
		{
			Debug.LogError(transform.Path() +": missing connection search icon.");
		}

		connectionScreen.gameObject.SetActive(false);
	}
	
	public void SetupGlobal()
	{
		StartCoroutine(GameStartRoutine());
	}

	protected IEnumerator GameStartRoutine()
	{
		yield return null;	// delay one frame so localization etc. can be initialized properly

		if (PlayerAuthCrossSceneInfo.use.checkedAuthentication == true)	// only check this once at the beginning of the game
			yield break;

		yield return StartCoroutine(CheckConnection());

		yield return StartCoroutine(AskForAuthRoutine());

		PlayerAuthCrossSceneInfo.use.checkedAuthentication = true;
	}

	protected IEnumerator AskForAuthRoutine()
	{
		if (!PlayerAuthCrossSceneInfo.use.hasConnection)
			yield break;

		//PlayerAuthCrossSceneInfo.use.playerAuthenticated should be set to false if:
		// - there is no locally stored username or password
		// - if authentication with those stored values failed

		//PlayerAuthCrossSceneInfo.use.playerAuthenticated = LugusConfig.use.System.GetBool("KikaAndBob.player.authenticated", false);
		
		if (!PlayerAuthCrossSceneInfo.use.playerAuthenticated)
		{
			DialogueBox authorizeBox = DialogueManager.use.CreateBox(KikaAndBob.ScreenAnchor.Center, LugusResources.use.GetText("global.authmessage"));
			authorizeBox.boxType = DialogueBox.BoxType.ConfirmCancel;
			authorizeBox.onConfirmButtonClicked += OnAuthConfirmButtonClicked;
			authorizeBox.onCancelButtonClicked += OnAuthCancelButtonClicked;
			authorizeBox.blockInput = true;
			authorizeBox.Show();
		}

		yield break;
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
	}

	private IEnumerator CheckConnection() 
	{
		testingConnection = true;

		Ping pingKikaAndBob = new Ping("46.51.206.135");	//http://kikabob2.submarine.nl/

		Debug.Log("Checking internet connection. Pinging IP: "+ pingKikaAndBob.ip);

		connectionScreen.gameObject.SetActive(true);

		// sometimes the text mesh wrapper doesn't update immediately
		connectionScreen.GetComponentInChildren<TextMeshWrapper>().UpdateWrapping();

		float startTime = Time.time;

		while (!pingKikaAndBob.isDone && Time.time < startTime + 5.0f) 
		{
			checkingConnectionIcon.Rotate(new Vector3(0, 0, -360 * Time.deltaTime));
			yield return new WaitForEndOfFrame();
		}

		if(pingKikaAndBob.isDone) 
		{
			Debug.Log("Internet connection established. Ping was " + (Time.time - startTime).ToString());
			connectionScreen.gameObject.SetActive(false);
			testingConnection = false;
			PlayerAuthCrossSceneInfo.use.hasConnection = true;
		} 
		else 
		{
			Debug.Log("No internet connection found.");
			connectionScreen.gameObject.SetActive(false);

			DialogueBox missingInternetBox = DialogueManager.use.CreateBox(KikaAndBob.ScreenAnchor.Center, LugusResources.use.GetText("global.connection.failed"));
			missingInternetBox.boxType = DialogueBox.BoxType.Continue;
			missingInternetBox.onContinueButtonClicked += OnConnectionContinueButtonClicked;
			missingInternetBox.blockInput = true;
			missingInternetBox.Show();
		}

		while (testingConnection)
			yield return null;

		yield break;
	}

	protected void OnConnectionContinueButtonClicked(DialogueBox box)
	{
		box.onContinueButtonClicked -= OnAuthConfirmButtonClicked;
		testingConnection = false;
		box.Hide();
	}


	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start() 
	{
		SetupGlobal();
	}
}
