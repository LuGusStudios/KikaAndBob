using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CountdownScreen : MonoBehaviour 
{
	public float duration = 3.0f;
	public int countDownFrom = 3;

	public SpriteRenderer controlsHint = null;

	public TextMeshWrapper countText = null;
	public TextMeshWrapper countTextShadow = null;

	public Vector3 originalTextScale = Vector3.one;

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( controlsHint == null )
		{
			controlsHint = transform.FindChild("ControlsHint").GetComponent<SpriteRenderer>();
		}

		if( countText == null )
		{
			countText = transform.FindChild("CountTextContainer/CountText").GetComponent<TextMeshWrapper>();
		}
		if( countTextShadow == null )
		{
			countTextShadow = transform.FindChild("CountTextContainer/CountTextShadow").GetComponent<TextMeshWrapper>();
		}

		if( countText != null )
		{
			originalTextScale = countText.transform.parent.localScale; 
		}

		Hide ();
	}

	public void Hide()
	{
		this.transform.localPosition = new Vector3(9999.0f, 9999.0f, 9999.0f);
	}

	public void StartCountdown()
	{
		StartCountdown( this.countDownFrom, this.duration );
	}

	public void StartCountdown(int countDownFrom, float duration)
	{
		this.duration = duration;
		this.countDownFrom = countDownFrom;

		LugusCoroutines.use.StartRoutine( CountdownRoutine() );
	}

	protected IEnumerator CountdownRoutine()
	{
		this.transform.localPosition = Vector3.zero;

#if UNITY_IOS
		controlsHint.color = controlsHint.color.a(0.0f);
#elif
		controlsHint.color = Color.white;
#endif

		//Debug.LogError("Starting countdown routine " + countDownFrom + " // " + duration);

		int count = countDownFrom;
		float frameTime = duration / (float) countDownFrom; // ex. 3s / 3 = 1s per number. 2s / 5 =  0.4s per number

		while( count > 0 )
		{
			//Debug.LogWarning("Countdown it " + count + " ... " + frameTime );

			countText.SetText("" + count);
			countTextShadow.SetText("" + count);
			countText.transform.parent.localScale = originalTextScale;

			// extra -0.1f, otherwhise iTween sometimes fucked and ignored the next count... very weird
			countText.transform.parent.gameObject.ScaleTo( Vector3.zero ).Time ( frameTime - 0.1f ).Execute();

			// note: soundfile names aren't correct for the order we're using them in :)
			string sound = "CountOne01";
			if( count == 2 )
			{
				sound = "CountTwo01";
			}
			else if( count == 1 )
			{
				sound = "CountThree01";
			}

			LugusAudio.use.SFX().Play( LugusResources.use.Shared.GetAudio(sound) );

			--count;

			yield return new WaitForSeconds( frameTime );
			countText.transform.parent.gameObject.StopTweens();
		}

#if !UNITY_IOS
		// fade out the controls hint
		float hintTime = 3.0f;

		float timerStart = Time.time;
		while ((Time.time - timerStart) <= hintTime)
		{
			controlsHint.color = controlsHint.color.a( Mathf.Lerp(1.0f, 0.0f, (Time.realtimeSinceStartup - timerStart) / hintTime ));
			yield return null;
		}
		
		controlsHint.color = controlsHint.color.a(0.0f);	// this will ensure the fade always reaches perfect completion
#endif


		this.transform.localPosition = new Vector3(9999.0f, 9999.0f, 9999.0f);
	}

	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	public void SetControlsHint(string spriteKey)
	{
		controlsHint.sprite = LugusResources.use.GetSprite( spriteKey );
	}

	
	public void RepositionControlHints(KikaAndBob.ScreenAnchor mainAnchor, KikaAndBob.ScreenAnchor subAnchor = KikaAndBob.ScreenAnchor.NONE)
	{
		Rect container = LugusUtil.UIScreenSize; // if subAnchor == NONE, we just use the full screen
		
		if( subAnchor != KikaAndBob.ScreenAnchor.NONE )
		{
			container = KikaAndBob.ScreenAnchorHelper.GetQuadrantRect(mainAnchor, LugusUtil.UIScreenSize);
			mainAnchor = subAnchor;
		}



		Rect controlsRect = controlsHint.bounds.ToRectXY();
		
		Vector2 position = KikaAndBob.ScreenAnchorHelper.ExtendTowards(mainAnchor, controlsRect, container, new Vector2(0.1f,0.1f) ); 
		controlsHint.transform.localPosition = position;
	}

	
	protected void Update () 
	{
		/*
		//if( LugusInput.use.KeyDown( KeyCode.C ) )
		{
			RepositionControlHints( KikaAndBob.ScreenAnchor.BottomLeft, KikaAndBob.ScreenAnchor.BottomLeft);
			StartCountdown( 3, 3 );
		}
		*/
	
	}
}
