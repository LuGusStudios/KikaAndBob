using UnityEngine;
using System.Collections;

public class FroggerLaneWater : FroggerLane 
{
	protected Transform splash = null;

	public override void SetUpLocal()
	{
		base.SetUpLocal();
		splash = transform.FindChild("Splash");

		if (splash == null)
		{
			Debug.LogError(name + ": Water lane is missing splash object.");
		}
	}

	protected override void EnterSurfaceEffect (FroggerCharacter character)
	{
		FroggerGameManager.use.LoseGame();
		LugusCoroutines.use.StartRoutine(SplashRoutine(character));
	}

	protected IEnumerator SplashRoutine(FroggerCharacter character)
	{
		if (splash == null)
			yield break;

//		if (enterSoundKeys.Count > 0)
//		{
//			LugusAudio.use.SFX().Play(LugusResources.use.Shared.GetAudio(enterSoundKeys[Random.Range(0, enterSoundKeys.Count)]));
//		}

		character.ShowCharacter(false);

		GameObject splashCopy = (GameObject) Instantiate(splash.gameObject);
		splashCopy.transform.position = character.transform.position + new Vector3(0, 0, -10);

		yield return new WaitForSeconds(0.8f);

		if (splashCopy != null)	// handy to check in case level was rebuilt
		{
			GameObject.Destroy(splashCopy);
		}
	}
}
