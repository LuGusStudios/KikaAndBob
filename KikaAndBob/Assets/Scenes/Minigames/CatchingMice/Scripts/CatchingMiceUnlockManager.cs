using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceUnlockManager : LugusSingletonRuntime<CatchingMiceUnlockManager>
{
	protected string unlockString = "MouseHuntUnlockIndex";

	public void CheckUnlock(LevelLoaderDefault levelLoader, IMinigameCrossSceneInfo crossSceneInfo)
	{
		if (levelLoader.IsHighestLevel(crossSceneInfo.GetLevelIndex()))
		{
			int currentMouseHuntUnlock = LugusConfig.use.User.GetInt(unlockString, 0);
			int newValue = currentMouseHuntUnlock + 1;

			if (newValue < 26)
			{
				LugusConfig.use.User.SetInt(unlockString, newValue, true);
				LugusConfig.use.User.SetBool("e00_catchingmice" + "_level_" + newValue.ToString(), true, true);	// basically, this is only done to not have to
			}																							// set up an entirely new level select screen for the mouse hunt game

		}
	}

	public void CheckUnlockDinnerDash(IDinnerDashConfig dinnerDashConfig, IMinigameCrossSceneInfo crossSceneInfo)
	{
		if (dinnerDashConfig.IsLastLevel(crossSceneInfo.GetLevelIndex()))
		{
			int currentMouseHuntUnlock = LugusConfig.use.User.GetInt(unlockString, 0);
			int newValue = currentMouseHuntUnlock + 1;
			
			if (newValue < 26)
			{
				LugusConfig.use.User.SetInt(unlockString, newValue, true);
				LugusConfig.use.User.SetBool("e00_catchingmice" + "_level_" + newValue.ToString(), true, true);	
			}	
		}
	}

	public void CheckUnlockRunner(IRunnerConfig runnerConfig, IMinigameCrossSceneInfo crossSceneInfo)
	{
		if (runnerConfig.IsLastLevel(crossSceneInfo.GetLevelIndex()))
		{
			int currentMouseHuntUnlock = LugusConfig.use.User.GetInt(unlockString, 0);
			int newValue = currentMouseHuntUnlock + 1;
			
			if (newValue < 26)
			{
				LugusConfig.use.User.SetInt(unlockString, newValue, true);
				LugusConfig.use.User.SetBool("e00_catchingmice" + "_level_" + newValue.ToString(), true, true);	
			}	
		}
	}
}
