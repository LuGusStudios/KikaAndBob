using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueBox : MonoBehaviour 
{
	public enum BoxType
	{
		NONE = -1,

		Notification = 1, // no buttons, auto-hide or hidden by script
		Continue = 2, // just an OK button
	}

	public bool available = true;

	public Transform background = null;
	protected Vector3 originalBackgroundSize = Vector3.one;
	protected TextMeshWrapper textSmall = null;
	protected TextMeshWrapper textLarge = null;
	public string text = "";
	public SpriteRenderer icon = null;

	protected KikaAndBob.ScreenAnchor mainAnchor = KikaAndBob.ScreenAnchor.NONE;
	protected KikaAndBob.ScreenAnchor subAnchor = KikaAndBob.ScreenAnchor.NONE;

	protected Vector3 targetPosition = Vector3.zero;

	public Vector2 margin = new Vector2(50.0f, 50.0f); // in PIXELS
	public Vector2 backgroundPadding = new Vector2(0.0f, 40.0f); // in PIXELS 

	public BoxType boxType = BoxType.Notification;
	public Button ContinueButton = null;

	public delegate void OnContinueButtonClicked(DialogueBox box);
	public OnContinueButtonClicked onContinueButtonClicked;

	public void Reposition( KikaAndBob.ScreenAnchor mainAnchor = KikaAndBob.ScreenAnchor.Center, KikaAndBob.ScreenAnchor subAnchor = KikaAndBob.ScreenAnchor.Center )
	{
		this.mainAnchor = mainAnchor;
		this.subAnchor = subAnchor;

		// update the text-wrapping
		TextMeshWrapper chosenText = null;
		if( this.icon.sprite != null )
		{
			this.textLarge.gameObject.SetActive( false );
			this.textSmall.gameObject.SetActive( true  );
			
			this.textSmall.textMesh.text = text; 
			this.textSmall.UpdateWrapping();

			chosenText = this.textSmall;
		}
		else
		{
			this.textSmall.gameObject.SetActive( false );
			this.textLarge.gameObject.SetActive( true  );
			
			this.textLarge.textMesh.text = text; 
			this.textLarge.UpdateWrapping();
			
			chosenText = this.textLarge;
		}

		// fit the background around the text
		// for now, only scale in height, assume width is already correct by setup
		// * 100.0f because texture is imported at 100 pixels/unit scale
		float newHeight = Mathf.Max( originalBackgroundSize.y, (chosenText.renderer.bounds.size.y + (backgroundPadding.y / 100.0f)) * 100.0f );
		background.transform.localScale = background.transform.localScale.y( newHeight);


		//Vector2 basePos = KikaAndBob.ScreenAnchorHelper.GetQuadrantCenter( mainAnchor, LugusUtil.UIScreenSize );

		Rect mainContainer = KikaAndBob.ScreenAnchorHelper.GetQuadrantRect( mainAnchor, LugusUtil.UIScreenSizePixelPerfect );
		Rect backgroundRect = background.renderer.bounds.ToRectXY();//new Rect( background.renderer.bounds.center.x , background.renderer.bounds.center.y, background.renderer.bounds.size.x * 100.0f, background.renderer.bounds.size.y * 100.0f );
		backgroundRect.width = backgroundRect.width * 100;
		backgroundRect.height = backgroundRect.height * 100;

		//Debug.LogWarning("BACKGROUND RECT " + backgroundRect + " vs " + mainContainer );

		// add margin
		//mainContainer.height = mainContainer.height - (mainContainer.height / 10.0f);
		//mainContainer.width = mainContainer.width - (mainContainer.width / 10.0f);

		Vector2 basePos = KikaAndBob.ScreenAnchorHelper.ExtendTowards( subAnchor, backgroundRect, mainContainer, margin );

		Debug.LogWarning ("REPOSITION " + mainAnchor + " + "  + subAnchor + " // " + (basePos / 100.0f).v3 ());
	
		targetPosition = (basePos / 100.0f).v3 ();
	}

	public void Reset()
	{
		mainAnchor = KikaAndBob.ScreenAnchor.NONE;
		subAnchor = KikaAndBob.ScreenAnchor.NONE;
		targetPosition = Vector3.zero; 
	}

	protected ILugusCoroutineHandle autoHideHandle = null;

	protected void RepositionButtons()
	{
		Debug.LogError("RepositionButtons " + boxType);

		if( boxType == BoxType.NONE )
		{
			ContinueButton.transform.parent.gameObject.SetActive(false);
		}
		else if( boxType == BoxType.Notification )
		{
			ContinueButton.transform.parent.gameObject.SetActive(false);

		}
		else if( boxType == BoxType.Continue )
		{
			// we need to move the playButton down so it fits beneath the box
			// the background will probably have been scaled to fit the text
			ContinueButton.transform.parent.gameObject.SetActive(true);

			Transform continueButtonContainer = ContinueButton.transform.parent;

			// find out the bottom of the background
			float backgroundBottom = background.renderer.bounds.center.y - background.renderer.bounds.extents.y;
			//continueButtonContainer.localPosition = continueButtonContainer.localPosition.y( backgroundBottom );
			continueButtonContainer.position = continueButtonContainer.position.y( backgroundBottom );
			
			//Debug.LogError("REpositioning buttonz " + backgroundBottom + " // " + background.renderer.bounds.center + " vs " + background.localPosition);
		}
	}


	public void Show(float autoHideDelay, bool hideOthers = true)
	{

		autoHideHandle = LugusCoroutines.use.StartRoutine( AutoHideRoutine(autoHideDelay) );
		Show ( hideOthers ); 
	}

	protected IEnumerator AutoHideRoutine(float autoHideDelay)
	{
		yield return new WaitForSeconds( autoHideDelay );

		Hide ();
	}

	public void Show(bool hideOthers = true)
	{
		if( hideOthers )
		{
			DialogueManager.use.HideOthers(this);
		}
				
		available = false;

		if( mainAnchor == KikaAndBob.ScreenAnchor.NONE )
		{
			Reposition( KikaAndBob.ScreenAnchor.Center );
		}

		RepositionButtons();

		//Debug.LogError(transform.Path () + " : Repositioning to local coords " + targetPosition);
		this.transform.localPosition = targetPosition;
	}

	public void Hide()
	{
		if( autoHideHandle != null && autoHideHandle.Running )
		{
			autoHideHandle.StopRoutine();
		}

		autoHideHandle = null;

		boxType = BoxType.Notification;
		available = true;
		
		this.transform.position = new Vector3(9999.0f, 9999.0f, 9999.0f);
		Reset ();
	}

	public void Resize()
	{
		//Vector2 maxDimensions, Vector2 padding = new Vector2(0.05f, 0.05f) 

		// eerder: maxDim is 1 quadrant * padding
		// dan kijken hoeveel plaats tekst inneemt met deze max afmetingen
		// vooral height moduleren tot het er mooi in past

		// calculatedWidth / height
		// reposition: zoveel mogelijk naar onder/boven verschuiven (of toch mooi in het midden houden?)

		//Vector2 paddedScreen = LugusUtil.UIScreenSize - (LugusUtil.UIScreenSize * padding);
		//float halfWidth = paddedScreen.x / 2.0f;
		//float halfHeight = paddedScreen.y / 2.0f;

		//if( maxDimensions == Vector2.zero )
		//	maxDimensions = new Vector2( halfWidth, halfHeight );
	}

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( background == null )
		{
			background = this.transform.FindChild("Background");
		}
		if( background == null )
		{
			Debug.LogError( transform.Path () + " : No background found!" );
		}
		else
		{
			originalBackgroundSize = background.localScale.v2 ();
		}

		if( textSmall == null )
		{
			textSmall = this.transform.FindChild("TextSmall").GetComponent<TextMeshWrapper>();
		}
		if( textSmall == null )
		{
			Debug.LogError( transform.Path () + " : No textSmall found!" );
		}
		
		if( textLarge == null )
		{
			textLarge = this.transform.FindChild("TextLarge").GetComponent<TextMeshWrapper>();
		}
		if( textLarge == null )
		{
			Debug.LogError( transform.Path () + " : No textLarge found!" );
		}

		if( icon == null )
		{
			icon = this.transform.FindChild("Icon").GetComponent<SpriteRenderer>();
		}
		if( icon == null )
		{
			Debug.LogError( transform.Path () + " : No icon found!" );
		}
		
		if( ContinueButton == null )
		{
			ContinueButton = this.transform.GetComponentInChildren<Button>();
		}
		if( ContinueButton == null )
		{
			Debug.LogError( transform.Path () + " : No ContinueButton found!" );
		}

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
	
	protected void Update () 
	{
		if( available ) // we're not currently in active use: no interaction allowed
			return;

		if( ContinueButton.pressed )
		{
			if( onContinueButtonClicked != null )
				onContinueButtonClicked(this);
		}
	}
}
