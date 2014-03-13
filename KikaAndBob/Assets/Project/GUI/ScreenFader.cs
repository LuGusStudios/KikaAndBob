using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenFader : LugusSingletonExisting<ScreenFader>
{
	public Sprite fadeImage = null;

	protected SpriteRenderer fadeRenderer = null;
	protected GameObject cameraFade = null;
	protected GUITexture fadeGUITexture = null;
	protected ILugusCoroutineHandle fadeRoutine = null;

	public void SetupLocal()
	{
	}
	
	public void SetupGlobal()
	{
		if (fadeRenderer == null)
		{
			GameObject fadeImageObject = new GameObject("Fader");
			fadeRenderer = fadeImageObject.AddComponent<SpriteRenderer>();
			fadeRenderer.sprite = fadeImage;
			fadeImageObject.transform.parent = LugusCamera.ui.transform;
			fadeImageObject.transform.localPosition = Vector3.zero.z(1);
			fadeImageObject.layer = LayerMask.NameToLayer("GUI");
		}

//		if (cameraFade == null)
//		{
//			cameraFade = iTween.CameraFadeAdd(iTween.CameraTexture(Color.white));
//			cameraFade.layer = LayerMask.NameToLayer("GUI"); 
//			fadeGUITexture = cameraFade.GetComponent<GUITexture>();
//		}
//	
//		if (LugusCamera.ui.GetComponent<GUILayer>() == null)
//		{
//			LugusCamera.ui.gameObject.AddComponent<GUILayer>();		// GUICamera needs a GUILayer to render the Itween fade
//		}

		FadeIn(0.5f);
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	public void FadeOut(float time)
	{
		Debug.Log("ScreenFader: Fading out.");

		fadeRenderer.color = fadeRenderer.color.a(0.0f);

		if (fadeRoutine != null && fadeRoutine.Running)
		{
			fadeRoutine.StopRoutine();
		}

		fadeRoutine = LugusCoroutines.use.StartRoutine(FadeRoutine(1.0f, 0.5f));
	}

	public void FadeIn(float time)
	{
		Debug.Log("ScreenFader: Fading in.");

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
