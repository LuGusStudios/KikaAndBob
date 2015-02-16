using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class KBAPIConnection : LugusSingletonRuntime<KBAPIConnection> 
{
	public bool hasConnection = false;
	public bool loggingIn = false;



	public string username = "";
	public string password = "";


	public string errorMessage = "";
	public string debugTxt = "";
	
	public string baseURL = "http://kikabob2.submarine.nl/api/"; // has to end with a / !!!
	

	public bool live = true; 

	// for now, these are the same, but you can envision testing the API Locally when everything is live so you don't risk breaking existing functionality in production
	public string staging_baseURL = "http://kikabob2.submarine.nl/api/"; 
	public string live_baseURL = "http://kikabob2.submarine.nl/api/";

	protected Transform connectionScreen = null;
	protected bool testingConnection = false;

	public void APILog(string message)
	{
		Debug.Log(message);
		debugTxt = message;
	}

	public void APILogError(string message)
	{
		Debug.LogError(message);
		//debugTxt += message;

		//errorMessage = message;
	}

	public void Login(string username, string password )
	{
		LugusCoroutines.use.StartRoutine( LoginRoutine (username, password) );
	}

	public IEnumerator LoginRoutine(string username, string password)
	{
		if( loggingIn )
		{
			APILogError("KBAPIConnection:Login : already logging in");
			yield break;
		}
		
		PlayerAuthCrossSceneInfo.use.loggedIn = false;
		loggingIn = true;
		
		errorMessage = "";
		
		this.username = username;
		this.password = password;

		
		APILog("KBAPIConnection:LoginRoutine start : " + username + " @ " + password);

		string url = baseURL + "authUser"; // no trailing / !!!, otherwhise we get 404

		WWWForm form = new WWWForm();
		form.AddField("userName", username);
		form.AddField("password", password);

		
		APILog("KBAPIConnection:LoginRoutine start : " + url);
		
		WWW www = new WWW(url, form);
		yield return www;
		
		
		APILog("KBAPIConnection:LoginRoutine returned : " + www.text + " // " + www.error);
		debugTxt = www.text;
		
		JSONObject objects = new JSONObject(www.text);
		
		loggingIn = false;

		if( objects.HasField("result") && objects.GetString("result") == "success" )
		{
			PlayerAuthCrossSceneInfo.use.loggedIn = true;
			PlayerAuthCrossSceneInfo.use.userId = objects.GetString("id");
	
			PlayerAuthCrossSceneInfo.use.userDataString = EncodingWrapper.Base64Decode( objects.GetString("userData") );
			PlayerAuthCrossSceneInfo.use.userDataObj = new JSONObject( PlayerAuthCrossSceneInfo.use.userDataString );
		}
		else
		{
			PlayerAuthCrossSceneInfo.use.loggedIn = false;

			APILogError("KBAPIConnection:LoginRoutine: something went wrong... " + www.text + " // " + www.error);

			if( objects.HasField("messageKey") && ( !string.IsNullOrEmpty(objects.GetString("messageKey")) ) )
			{
				errorMessage = LugusResources.use.Localized.GetText( objects.GetString("messageKey") );
			}
			else
			{
				errorMessage = "ERROR " + www.error;
			}
		}

		yield break;
	}

	public void Register(string username, string password, string userdata)
	{
		LugusCoroutines.use.StartRoutine( RegisterRoutine(username, password, userdata) );
	}

	public IEnumerator RegisterRoutine(string username, string password, string userdata)
	{
		if( loggingIn )
		{
			APILogError("KBAPIConnection:Login : already logging in or registering.");
			yield break;
		}

		loggingIn = true;

		errorMessage = "";
		
		APILog("KBAPIConnection:RegisterRoutine start : " + username + " @ " + password + " @ " + userdata);

		// need to make sure we only send valid json, otherwhise we'll be in trouble later
		JSONObject userDataJSONObj = null;
		try
		{
			userDataJSONObj = new JSONObject(userdata);
		}
		catch(Exception e)
		{
			APILogError("KBAPIConnection:RegisterRoutine start : error parsing userdata! " + e.Message);
			yield break;
		}
		
		
		string url = baseURL + "addUser"; // no trailing / !!!, otherwhise we get 404
		
		WWWForm form = new WWWForm();
		form.AddField("userName", username);
		form.AddField("password", password);
		form.AddField("userData", EncodingWrapper.Base64Encode(userdata) );
		
		
		APILog("KBAPIConnection:RegisterRoutine start : " + url);
		
		WWW www = new WWW(url, form);
		yield return www;
		
		
		APILog("KBAPIConnection:RegisterRoutine returned : " + www.text + " // " + www.error);
		debugTxt = www.text;
		
		JSONObject objects = new JSONObject(www.text);
		
		if( objects.HasField("result") && objects.GetString("result") == "success" )
		{
			PlayerAuthCrossSceneInfo.use.loggedIn = true;
			
			PlayerAuthCrossSceneInfo.use.userId = objects.GetString("id");

			this.username = username;
			this.password = password;
			PlayerAuthCrossSceneInfo.use.userDataString = userdata;

			PlayerAuthCrossSceneInfo.use.userDataObj = new JSONObject( userdata );
		}
		else
		{
			PlayerAuthCrossSceneInfo.use.loggedIn = false;
			// TODO: show proper error message to user!
			
			APILogError("KBAPIConnection:RegisterRoutine: something went wrong... " + www.text + " // " + www.error);
			
			if( objects.HasField("messageKey") && ( !string.IsNullOrEmpty(objects.GetString("messageKey")) ) )
			{
				errorMessage = LugusResources.use.Localized.GetText( objects.GetString("messageKey") );
			}
			else
			{
				errorMessage = "ERROR " + www.error;
			}
		}

		loggingIn = false;
		
		yield break;
	}

	public IEnumerator GetUserScoreRoutine(List<int> output, int gameId, int level, string userId)
	{
		errorMessage = "";

		APILog("KBAPIConnection:GetUserScoreRoutine start : " + gameId + " @ " + level + " @ " + userId);

		string url = baseURL + "get.php?url=getUserScore&gameId=" + gameId + "&level=" + level + "&userId=" + userId;

		APILog("KBAPIConnection:GetUserScoreRoutine start : " + url);

		WWW www = new WWW(url);
		yield return www;

		
		APILog("KBAPIConnection:GetUserScoreRoutine returned : " + www.text + " // " + www.error);
		debugTxt = www.text;


		JSONObject objects = new JSONObject(www.text);
		
		if( objects.HasField("result") && objects.GetString("result") == "success" )
		{
			string score = objects.GetString("score");	// this only returns one value, so just a string, not a JSON object
			int scoreInt = 0;

			if (int.TryParse(score, out scoreInt))
			{
				output.Add(scoreInt);
			}
		}
		else
		{
			APILogError("KBAPIConnection:GetUserScoreRoutine: something went wrong... " + www.text + " // " + www.error);
			
			if( objects.HasField("messageKey") && ( !string.IsNullOrEmpty(objects.GetString("messageKey")) ) )
			{
				errorMessage = LugusResources.use.Localized.GetText( objects.GetString("messageKey") );
			}
			else
			{
				errorMessage = "ERROR " + www.error;
			}
		}

	}
	

	public void GetScores(List<JSONObject> output, int gameId, int level, int from = 0, int length = 10)
	{
		LugusCoroutines.use.StartRoutine( GetScoresRoutine(output, gameId, level, from, length) );
	}

	// example of passing in a List<> to get the output of the function
	// NOTE: you can also use a custom datastructure instead of JSONObject of course, but usually probably not needed when just showing some data
	// other option would be using delegates, but that can be tedious with the custom callbacks and cross-scene stuff (delegates stay linked etc.)
	// see KetnetController for delegate examples
	public IEnumerator GetScoresRoutine(List<JSONObject> output, int gameId, int level, int from = 0, int length = 10)
	{
		errorMessage = "";
		
		APILog("KBAPIConnection:GetScoresRoutine start : " + gameId + " @ " + level + " @ " + from + " @ " + length);
		
		
		string url = baseURL + "get.php?url=getScore&gameId=" + gameId + "&level=" + level + "&from=" + from + "&length=" + length;

		APILog("KBAPIConnection:GetScoresRoutine start : " + url);
		
		WWW www = new WWW(url);
		yield return www;
		
		
		APILog("KBAPIConnection:GetScoresRoutine returned : " + www.text + " // " + www.error);
		debugTxt = www.text;
		
		JSONObject objects = new JSONObject(www.text);
		
		if( objects.HasField("result") && objects.GetString("result") == "success" )
		{
			JSONObject scores = objects.GetField("scores");

			foreach( JSONObject score in scores.list )
			{
				output.Add( score );

				Debug.Log("Score received : " + score.GetString("userName") + " @ " + score.GetString("score") );
			}
		}
		else
		{
			APILogError("KBAPIConnection:GetScoresRoutine: something went wrong... " + www.text + " // " + www.error);
			
			if( objects.HasField("messageKey") && ( !string.IsNullOrEmpty(objects.GetString("messageKey")) ) )
			{
				errorMessage = LugusResources.use.Localized.GetText( objects.GetString("messageKey") );
			}
			else
			{
				errorMessage = "ERROR " + www.error;
			}
		}
		
		yield break;
	}
	

//	public void AddScore(int gameId, int level, int score)
//	{
//		LugusCoroutines.use.StartRoutine( AddScoreRoutine(gameId, level, score) );
//	}

	public IEnumerator AddScoreRoutine(int gameId, int level, int score)
	{
		yield return LugusCoroutines.use.StartRoutine(AddScoreRoutine(gameId, level, (float) score));
	}


	public IEnumerator AddScoreRoutine(int gameId, int level, float score)
	{
		errorMessage = "";
		
		APILog("KBAPIConnection:AddScoreRoutine start : " + gameId + " @ " + level + " @ " + score);


		string url = baseURL + "addScore"; // no trailing / !!!, otherwhise we get 404

		WWWForm form = new WWWForm();
		form.AddField("gameId", gameId);
		form.AddField("level", level);
		form.AddField("score", score.ToString() );
		form.AddField("userId", PlayerAuthCrossSceneInfo.use.userId);
		
		
		APILog("KBAPIConnection:AddScoreRoutine start : " + url);
		
		WWW www = new WWW(url, form);
		yield return www;
		
		
		APILog("KBAPIConnection:AddScoreRoutine returned : " + www.text + " // " + www.error);
		debugTxt = www.text;
		
		JSONObject objects = new JSONObject(www.text);
		
		if( objects.HasField("result") && objects.GetString("result") == "success" )
		{
			// score added, nothing to do here :)
		}
		else
		{	
			if (objects.GetString("messageKey") == "scores.toolow")
			{
				APILog("Score was lower than current highscore for this player.");
			}
			else
			{
				APILogError("KBAPIConnection:AddScoreRoutine: something went wrong... " + www.text + " // " + www.error);
			}

			if( objects.HasField("error") && ( !string.IsNullOrEmpty(objects.GetString("messageKey")) ) )
			{
				errorMessage = LugusResources.use.Localized.GetText( objects.GetString("messageKey") );
			}
			else
			{
				errorMessage = "ERROR " + www.error;
			}
		}
		
		yield break;
	}

	
	
	public void GetGameId(List<int> gameId, string gameName)
	{
		LugusCoroutines.use.StartRoutine( GetGameIdRoutine( gameId, gameName ) );
		
	}
	
	public IEnumerator GetGameIdRoutine(List<int> gameId, string gameName)
	{
		errorMessage = "";
		
		APILog("KBAPIConnection:GetGameIdRoutine start : " + gameName );
		
		
		string url = baseURL + "get.php?url=getGameId&gameName=" + gameName ;
		
		APILog("KBAPIConnection:GetGameIdRoutine start : " + url);
		
		WWW www = new WWW(url);
		yield return www;
		
		
		APILog("KBAPIConnection:GetGameIdRoutine returned : " + www.text + " // " + www.error);
		debugTxt = www.text;
		
		JSONObject objects = new JSONObject(www.text);
		
		if( objects.HasField("result") && objects.GetString("result") == "success" )
		{
			string id = objects.GetString("id");

			gameId.Add( int.Parse(id) ); 
		}
		else
		{ 
			APILogError("KBAPIConnection:GetGameIdRoutine: something went wrong... " + www.text + " // " + www.error);
			
			if( objects.HasField("messageKey") && ( !string.IsNullOrEmpty(objects.GetString("messageKey")) ) )
			{
				errorMessage = LugusResources.use.Localized.GetText( objects.GetString("messageKey") );
			}
			else
			{
				errorMessage = "ERROR " + www.error;
			}
		}
		
		yield break;
	}

	public void UpdateUserData(string userData)
	{
		LugusCoroutines.use.StartRoutine( UpdateUserDataRoutine(userData) );
	}

	public IEnumerator UpdateUserDataRoutine(string userData)
	{
		errorMessage = "";
		
		APILog("KBAPIConnection:UpdateUserDataRoutine start : " + userData );
		
		
		string url = baseURL + "updateUserData"; // no trailing / !!!, otherwhise we get 404
		
		WWWForm form = new WWWForm();
		form.AddField("userData", EncodingWrapper.Base64Encode(userData) );
		form.AddField("userId", PlayerAuthCrossSceneInfo.use.userId );
		
		
		APILog("KBAPIConnection:UpdateUserDataRoutine start : " + url);
		
		WWW www = new WWW(url, form);
		yield return www;
		
		
		APILog("KBAPIConnection:UpdateUserDataRoutine returned : " + www.text + " // " + www.error);
		debugTxt = www.text;
		
		JSONObject objects = new JSONObject(www.text);
		
		if( objects.HasField("result") && objects.GetString("result") == "success" )
		{
			// score added, nothing to do here :) 
		}
		else
		{	
			APILogError("KBAPIConnection:UpdateUserDataRoutine: something went wrong... " + www.text + " // " + www.error);
			
			if( objects.HasField("error") && ( !string.IsNullOrEmpty(objects.GetString("messageKey")) ) )
			{
				errorMessage = LugusResources.use.Localized.GetText( objects.GetString("messageKey") );
			}
			else
			{
				errorMessage = "ERROR " + www.error;
			}
		}
		
		yield break;
	}

	public IEnumerator CheckConnectionRoutine() 
	{
		hasConnection = false;
		testingConnection = true;

		if (Application.internetReachability != NetworkReachability.NotReachable)
		{
			Ping pingKikaAndBob = new Ping("46.51.206.135");	//http://kikabob2.submarine.nl/
			
			Debug.Log("Checking internet connection. Pinging IP: "+ pingKikaAndBob.ip);

			
			float startTime = Time.time;

			Transform checkingConnectionIcon = connectionScreen.FindChild("CheckingConnectionIcon");

			while (!pingKikaAndBob.isDone && Time.time < startTime + 5.0f) 
			{
				if (Time.time - startTime > 0.5f)
				{
					if (connectionScreen.gameObject.activeSelf == false)
					{
						connectionScreen.gameObject.SetActive(true);
						connectionScreen.GetComponentInChildren<TextMeshWrapper>().UpdateWrapping();
					}
					
					checkingConnectionIcon.Rotate(new Vector3(0, 0, -360 * Time.deltaTime));
				}

				yield return new WaitForEndOfFrame();
			}

			if (pingKikaAndBob.isDone) 
			{
				Debug.Log("Internet connection established. Ping was " + (Time.time - startTime).ToString());
				connectionScreen.gameObject.SetActive(false);
				testingConnection = false;
				hasConnection = true;
				yield break;
			} 
		}
	
		Debug.Log("No internet connection found.");
		connectionScreen.gameObject.SetActive(false);
		
		DialogueBox missingInternetBox = DialogueManager.use.CreateBox(KikaAndBob.ScreenAnchor.Center, LugusResources.use.GetText("global.connection.failed"));
		missingInternetBox.boxType = DialogueBox.BoxType.Continue;
		missingInternetBox.onContinueButtonClicked += OnConnectionContinueButtonClicked;
		missingInternetBox.blockInput = true;
		missingInternetBox.Show();

		while (testingConnection)
			yield return null;
		
		yield break;
	}

	protected void OnConnectionContinueButtonClicked(DialogueBox box)
	{
		box.onContinueButtonClicked -= OnConnectionContinueButtonClicked;
		testingConnection = false;
		box.Hide();
	}

	public void SetupLocal()
	{
		if (connectionScreen == null)
		{
			connectionScreen = DialogueManager.use.transform.FindChild("ConnectionScreen");
		}
		
		if (connectionScreen == null)
		{
			Debug.LogError(transform.Path() +": missing connection screen.");
		}
	}
	
	public void SetupGlobal()
	{
		// do this in Global() to allow other debug scripts to switch the live status
		if( live )
		{
			baseURL = live_baseURL;
		}
		else
		{
			baseURL = staging_baseURL;
		}
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
	
	}
}
