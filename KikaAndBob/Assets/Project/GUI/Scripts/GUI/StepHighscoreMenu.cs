using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StepHighscoreMenu : IMenuStep 
{
	protected Vector3 originalPosition = Vector3.zero;
	protected Button leaveButton = null;
	protected TextMeshWrapper namesDisplay = null;
	protected TextMeshWrapper scoresDisplay = null;
	protected TextMeshWrapper yourScoreDisplay = null;
	protected ILugusCoroutineHandle scoreLoadRoutine = null;

	public void SetupLocal()
	{
		if (leaveButton == null)
		{
			leaveButton = transform.FindChild("LeaveButton").GetComponent<Button>();
		}
		if (leaveButton == null)
		{
			Debug.Log("StepHelpMenu: Missing leave button.");
		}

		if (namesDisplay == null)
		{
			namesDisplay = transform.FindChild("NamesDisplay").GetComponent<TextMeshWrapper>();
		}
		if (namesDisplay == null)
		{
			Debug.Log("StepHelpMenu: Missing names display.");
		}

		if (scoresDisplay == null)
		{
			scoresDisplay = transform.FindChild("ScoresDisplay").GetComponent<TextMeshWrapper>();
		}
		if (scoresDisplay == null)
		{
			Debug.Log("StepHelpMenu: Missing scores display.");
		}

		if (yourScoreDisplay == null)
		{
			yourScoreDisplay = transform.FindChild("YourScoreDisplay").GetComponent<TextMeshWrapper>();
		}
		if (yourScoreDisplay == null)
		{
			Debug.Log("StepHelpMenu: Missing your score display.");
		}
		
		originalPosition = transform.position;
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
		if (!activated)
			return;

		if (leaveButton.pressed)
		{
			MenuManager.use.ActivateMenu(MenuManagerDefault.MenuTypes.LevelMenu); 
		}
	}

	public override void Activate(bool animate = true)
	{
		activated = true;
		gameObject.SetActive(true);
		transform.localPosition = Vector3.zero;

		iTween.Stop(gameObject);
		transform.position = originalPosition + new Vector3(30, 0, 0);
		gameObject.MoveTo(originalPosition).Time(0.5f).EaseType(iTween.EaseType.easeOutBack).Execute();

		if (scoreLoadRoutine != null && scoreLoadRoutine.Running)
			scoreLoadRoutine.StopRoutine();

		scoreLoadRoutine = LugusCoroutines.use.StartRoutine(DisplayScores(MenuManager.use.currentSelectedLevel));	// set from StepLevelMenu
	}

	public override void Deactivate(bool animate = true)
	{
		activated = false;

		if (animate)
		{
			iTween.Stop(gameObject);
			gameObject.MoveTo(originalPosition + new Vector3(-30, 0, 0) ).Time(0.5f).EaseType(iTween.EaseType.easeOutBack).Execute();
		}
		else
		{
			gameObject.SetActive(false);
		}
	}

	protected void UpdateNamesAndScores(string newNamesText, string newScoresText)
	{
		namesDisplay.SetText(newNamesText);
		scoresDisplay.SetText(newScoresText);
	}

	public IEnumerator DisplayScores(int levelIndex)
	{
		// clear text
		UpdateNamesAndScores("", "");
		yourScoreDisplay.SetText("");

		// first check connection - if there is none, the KBAPIConnection will result in an error, but that's not currently translated
		// instead, we check connection and if there isn't one, we display an error that can be translated

		yield return StartCoroutine(KBAPIConnection.use.CheckConnectionRoutine());

		if (!KBAPIConnection.use.hasConnection)
			yield break;

		string namesOutput = "";
		string scoresOutput = "";

		List<int> foundGameIDs = new List<int>();

		yield return StartCoroutine(KBAPIConnection.use.GetGameIdRoutine(foundGameIDs, Application.loadedLevelName));

		if( foundGameIDs.Count >= 1 )
		{
			Debug.Log ("Received game-id " + foundGameIDs[0] + " for game " + Application.loadedLevelName);
		}
		else
		{
			Debug.LogError("No game id found for " + Application.loadedLevelName);

			if( !string.IsNullOrEmpty(KBAPIConnection.use.errorMessage))
				namesOutput += KBAPIConnection.use.errorMessage;

			UpdateNamesAndScores(namesOutput, "");
	
			yield break;
		}
		
		int gameID = foundGameIDs[0];
		
		List<JSONObject> scores = new List<JSONObject>();


		yield return StartCoroutine(KBAPIConnection.use.GetScoresRoutine(scores, gameID, levelIndex, 0, 10));


		if( scores.Count == 0 )
		{
			if( !string.IsNullOrEmpty(KBAPIConnection.use.errorMessage))
				namesOutput += KBAPIConnection.use.errorMessage;

			UpdateNamesAndScores(namesOutput, "");
			yield break;
		}
		else
		{	
			foreach( JSONObject score in scores )
			{
				namesOutput += (score.GetString("userName") + "\n");
				scoresOutput += (score.GetString("score") + "\n");
			}
		}

		UpdateNamesAndScores(namesOutput, scoresOutput);
		
		
		List<int> userScore = new List<int>();
		string userScoreMessage = "";

		yield return StartCoroutine(KBAPIConnection.use.GetUserScoreRoutine(userScore, gameID, levelIndex, 18));


		if( userScore.Count >= 1 )	// error or player doesn't have a score yet
		{	
			userScoreMessage += LugusResources.use.Localized.GetText("global.highscores.userscore") + "   " + userScore[0].ToString();
		}

		yourScoreDisplay.SetText(userScoreMessage);
	}
}
