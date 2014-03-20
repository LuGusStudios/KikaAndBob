using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLaneItemIceHole : FroggerLaneItemLethal 
{

	protected Transform splash = null;

	public void SetupLocal()
	{
		base.SetUpLocal();
		splash = transform.FindChild("Splash");

		if (splash == null)
		{
			Debug.LogError(name + ": Ice hole is missing splash object.");
		}
		else
		{
			splash.gameObject.SetActive(false);
		}
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected override void EnterSurfaceEffect(FroggerCharacter character)
	{
		FroggerGameManager.use.LoseGame();
		LugusCoroutines.use.StartRoutine(SplashRoutine(character));
	}

	protected IEnumerator SplashRoutine(FroggerCharacter character)
	{
		if (splash == null)
		{
			yield break;
		}

		character.ShowCharacter(false);

		GameObject splashCopy = (GameObject)Instantiate(splash.gameObject);
		splashCopy.transform.position = this.transform.position;
		splashCopy.SetActive(true);

		yield return new WaitForSeconds(0.8f);

		if (splashCopy != null)	// handy to check in case level was rebuilt
		{
			GameObject.Destroy(splashCopy);
		}
	}
}
