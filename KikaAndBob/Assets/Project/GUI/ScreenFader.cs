using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenFader : LugusSingletonExisting<ScreenFader>
{
	protected GameObject cameraFade = null;
	protected GUITexture fadeGUITexture = null;

	public void SetupLocal()
	{
	}
	
	public void SetupGlobal()
	{
		if (cameraFade == null)
		{
			cameraFade = iTween.CameraFadeAdd(iTween.CameraTexture(Color.white));
			cameraFade.layer = LayerMask.NameToLayer("GUI"); 
			fadeGUITexture = cameraFade.GetComponent<GUITexture>();
		}
	
		if (LugusCamera.ui.GetComponent<GUILayer>() == null)
		{
			LugusCamera.ui.gameObject.AddComponent<GUILayer>();		// GUICamera needs a GUILayer to render the Itween fade
		}

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

		fadeGUITexture.color = fadeGUITexture.color.a(0.0f);
		
		iTween.CameraFadeTo(1.0f, time);
	}

	public void FadeIn(float time)
	{
		Debug.Log("ScreenFader: Fading in.");

		fadeGUITexture.color = fadeGUITexture.color.a(1.0f);

		iTween.CameraFadeTo(0.0f, time);
	}
}
