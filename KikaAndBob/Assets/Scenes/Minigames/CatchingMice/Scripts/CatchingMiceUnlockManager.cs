using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceUnlockManager : LugusSingletonRuntime<CatchingMiceUnlockManager>
{
	protected string unlockString = "MouseHuntUnlockIndex";

	public void CheckUnlock(LevelLoaderDefault levelLoader, IMinigameCrossSceneInfo crossSceneInfo)
	{
		Debug.Log("CatchingMiceUnlockManager: Checking for game completion...");

		if (levelLoader.IsHighestLevel(crossSceneInfo.GetLevelIndex()))
		{
			if (LugusConfig.use.User.GetBool(Application.loadedLevelName + "_finished", false) == true)
			{
				Debug.Log("CatchingMiceUnlockManager: Already finished game: " + Application.loadedLevelName);
				return;
			}

			LugusConfig.use.User.SetBool(Application.loadedLevelName + "_finished", true, true);

			int currentMouseHuntUnlock = LugusConfig.use.User.GetInt(unlockString, 0);
			int newValue = currentMouseHuntUnlock + 1;

			if (newValue < 26)
			{
				Debug.Log("CatchingMiceUnlockManager: Unlocking new mousehunt level: " + newValue.ToString());
				HUDManager.use.LevelEndScreen.SetMessage(LugusResources.use.Localized.GetText("global.levelend.success.unlock"));
				LugusConfig.use.User.SetInt(unlockString, newValue, true);
				LugusConfig.use.User.SetBool("e00_catchingmice" + "_level_" + newValue.ToString(), true, true);	// basically, this is only done to not have to
			}																									// set up an entirely new level select screen for the mouse hunt game
			else
			{
				Debug.Log("CatchingMiceUnlockManager: Max mousehunt games unlocked.");
			}	
		}
		else
			Debug.Log("CatchingMiceUnlockManager: Not highest level.");
	}

	public void CheckUnlockDinnerDash(IDinnerDashConfig dinnerDashConfig, IMinigameCrossSceneInfo crossSceneInfo)
	{
		Debug.Log("CatchingMiceUnlockManager: Checking for game completion...");

		if (dinnerDashConfig.IsLastLevel(crossSceneInfo.GetLevelIndex()))
		{

			if (LugusConfig.use.User.GetBool(Application.loadedLevelName + "_finished", false) == true)
			{
				Debug.Log("CatchingMiceUnlockManager: Already finished game: " + Application.loadedLevelName);
				return;
			}
			
			LugusConfig.use.User.SetBool(Application.loadedLevelName + "_finished", true, true);


			int currentMouseHuntUnlock = LugusConfig.use.User.GetInt(unlockString, 0);
			int newValue = currentMouseHuntUnlock + 1;
			
			if (newValue < 26)
			{
				Debug.Log("CatchingMiceUnlockManager: Unlocking new mousehunt level: " + newValue.ToString());
				HUDManager.use.LevelEndScreen.SetMessage(LugusResources.use.Localized.GetText("global.levelend.success.unlock"));
				LugusConfig.use.User.SetInt(unlockString, newValue, true);
				LugusConfig.use.User.SetBool("e00_catchingmice" + "_level_" + newValue.ToString(), true, true);	
			}	
			else
			{
				Debug.Log("CatchingMiceUnlockManager: Max mousehunt games unlocked.");
			}
		}
		else
			Debug.Log("CatchingMiceUnlockManager: Not highest level.");
	}

	public void CheckUnlockRunner(IRunnerConfig runnerConfig, IMinigameCrossSceneInfo crossSceneInfo)
	{
		Debug.Log("CatchingMiceUnlockManager: Checking for game completion...");

		if (runnerConfig.IsLastLevel(crossSceneInfo.GetLevelIndex()))
		{

			if (LugusConfig.use.User.GetBool(Application.loadedLevelName + "_finished", false) == true)
			{
				Debug.Log("CatchingMiceUnlockManager: Already finished game: " + Application.loadedLevelName);
				return;
			}

			
			LugusConfig.use.User.SetBool(Application.loadedLevelName + "_finished", true, true);

			int currentMouseHuntUnlock = LugusConfig.use.User.GetInt(unlockString, 0);
			int newValue = currentMouseHuntUnlock + 1;
			
			if (newValue < 26)
			{
				Debug.Log("CatchingMiceUnlockManager: Unlocking new mousehunt level: " + newValue.ToString());
				HUDManager.use.LevelEndScreen.SetMessage(LugusResources.use.Localized.GetText("global.levelend.success.unlock"));
				LugusConfig.use.User.SetInt(unlockString, newValue, true);
				LugusConfig.use.User.SetBool("e00_catchingmice" + "_level_" + newValue.ToString(), true, true);	
			}	
			else
			{
				Debug.Log("CatchingMiceUnlockManager: Max mousehunt games unlocked.");
			}	
		}
		else
			Debug.Log("CatchingMiceUnlockManager: Not highest level.");
	}
}
