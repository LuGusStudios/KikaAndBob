using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanScreenFader : LugusSingletonExisting<PacmanScreenFader>
{
	protected SpriteRenderer fadeRenderer = null;
	protected ILugusCoroutineHandle fadeRoutine = null;

	protected void Awake()
	{
		SetupLocal();
	}
	
	protected void Start () 
	{
		SetupGlobal();
	}

	public void SetupLocal()
	{
		if (fadeRenderer == null)
		{
			fadeRenderer = GetComponent<SpriteRenderer>();
		}
		if (fadeRenderer == null)
		{
			Debug.Log("PacmanScreenFader: Missing sprite renderer!");
		}
	}

	public void SetupGlobal()
	{
	}

	public void FadeInAndOut(float duration)
	{
		LugusCoroutines.use.StartRoutine(InAndOutFade(duration));
	}

	protected IEnumerator InAndOutFade(float duration)
	{
		FadeOut(duration * 0.5f);

		yield return new WaitForSeconds(duration * 0.5f);

		FadeIn(duration * 0.5f);
	}

	protected void FadeOut(float time)
	{
		Debug.Log("PacmanScreenFader: Fading out.");
		
		fadeRenderer.color = fadeRenderer.color.a(0.0f);
		
		if (fadeRoutine != null && fadeRoutine.Running)
		{
			fadeRoutine.StopRoutine();
		}
		
		fadeRoutine = LugusCoroutines.use.StartRoutine(FadeRoutine(1.0f, 0.5f));
	}
	
	protected void FadeIn(float time)
	{
		Debug.Log("PacmanScreenFader: Fading in.");
		
		fadeRenderer.color = fadeRenderer.color.a(1.0f);
		
		if (fadeRoutine != null && fadeRoutine.Running)
		{
			fadeRoutine.StopRoutine();
		}
		
		fadeRoutine = LugusCoroutines.use.StartRoutine(FadeRoutine(0.0f, 0.5f));
	}
	
	protected IEnumerator FadeRoutine(float targetAlpha, float duration)
	{
		fadeRenderer.enabled = true;
		
		if (duration <= 0)
		{
			fadeRenderer.color = fadeRenderer.color.a(targetAlpha);
			yield break;
		}
		
		float startAlpha = fadeRenderer.color.a;
		float timerStart = Time.realtimeSinceStartup;
		
		while ((Time.realtimeSinceStartup - timerStart) <= duration)
		{
			fadeRenderer.color = fadeRenderer.color.a( Mathf.Lerp(startAlpha, targetAlpha, (Time.realtimeSinceStartup - timerStart) / duration ));
			yield return null;
		}
		
		fadeRenderer.color = fadeRenderer.color.a(targetAlpha);	// this will ensure the fade always reaches perfect completion
		
		if (fadeRenderer.color.a <= 0)
		{
			fadeRenderer.enabled = false;
		}
		
		yield break;
	}

}
