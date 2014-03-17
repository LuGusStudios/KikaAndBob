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

		character.ShowCharacter(false);
		StartCoroutine(SplashRoutine(character.transform.position + new Vector3(0f, 0f, -10f)));
	}

	public void DoSplashAnimation(Vector3 position)
	{
		StartCoroutine(SplashRoutine(position));
	}

	public void PlaySplashSFX()
	{
		if (enterSoundKeys.Count == 0)
		{
			return;
		}

		AudioClip splashSFX = LugusResources.use.Shared.GetAudio(enterSoundKeys[0]);
		if (splashSFX != LugusResources.use.errorAudio)
		{
			LugusAudio.use.SFX().Play(splashSFX);
		}
	}

	protected IEnumerator SplashRoutine(Vector3 position)
	{
		if (splash == null)
		{
			yield break;
		}

		GameObject splashCopy = (GameObject)GameObject.Instantiate(splash.gameObject);
		splashCopy.transform.position = position;

		yield return new WaitForSeconds(0.8f);

		if (splashCopy != null)
		{
			GameObject.Destroy(splashCopy.gameObject);
		}
	}
}
