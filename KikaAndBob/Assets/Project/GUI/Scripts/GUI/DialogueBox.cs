using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueBox : MonoBehaviour 
{
	public bool available = true;

	public Transform background = null;
	protected TextMeshWrapper textSmall = null;
	protected TextMeshWrapper textLarge = null;
	public string text = "";
	public SpriteRenderer icon = null;

	protected KikaAndBob.ScreenAnchor mainAnchor = KikaAndBob.ScreenAnchor.NONE;
	protected KikaAndBob.ScreenAnchor subAnchor = KikaAndBob.ScreenAnchor.NONE;

	protected Vector3 targetPosition = Vector3.zero;

	public void Reposition( KikaAndBob.ScreenAnchor mainAnchor = KikaAndBob.ScreenAnchor.Center, KikaAndBob.ScreenAnchor subAnchor = KikaAndBob.ScreenAnchor.Center )
	{
		this.mainAnchor = mainAnchor;
		this.subAnchor = subAnchor;

		//Vector2 basePos = KikaAndBob.ScreenAnchorHelper.GetQuadrantCenter( mainAnchor, LugusUtil.UIScreenSize );

		Rect mainContainer = KikaAndBob.ScreenAnchorHelper.GetQuadrant( mainAnchor, LugusUtil.UIScreenSize );
		Rect backgroundRect = new Rect( background.renderer.bounds.center.x , background.renderer.bounds.center.y, background.renderer.bounds.size.x * 100.0f, background.renderer.bounds.size.y * 100.0f );
		Debug.LogWarning("BACKGROUND RECT " + backgroundRect + " vs " + mainContainer );

		// add margin
		//mainContainer.height = mainContainer.height - (mainContainer.height / 10.0f);
		//mainContainer.width = mainContainer.width - (mainContainer.width / 10.0f);

		Vector2 basePos = KikaAndBob.ScreenAnchorHelper.ExtendTowards( subAnchor, backgroundRect, mainContainer, new Vector2(50.0f, 50.0f) );

		Debug.LogWarning ("REPOSITION " + mainAnchor + " + "  + subAnchor + " // " + (basePos / 100.0f).v3 ());
	
		if( this.icon.sprite != null )
		{
			this.textLarge.gameObject.SetActive( false );
			this.textSmall.gameObject.SetActive( true  );
			
			this.textSmall.textMesh.text = text; 
			this.textSmall.UpdateWrapping();
		}
		else
		{
			this.textSmall.gameObject.SetActive( false );
			this.textLarge.gameObject.SetActive( true  );

			this.textLarge.textMesh.text = text; 
			this.textLarge.UpdateWrapping();
		}

		targetPosition = (basePos / 100.0f).v3 ();

	}

	public void Reset()
	{
		mainAnchor = KikaAndBob.ScreenAnchor.NONE;
		subAnchor = KikaAndBob.ScreenAnchor.NONE;
		targetPosition = Vector3.zero; 
	}

	public void Show()
	{
		available = false;

		if( mainAnchor == KikaAndBob.ScreenAnchor.NONE )
		{
			Reposition( KikaAndBob.ScreenAnchor.Center );
		}

		this.transform.localPosition = targetPosition;
	}

	public void Hide()
	{
		available = true;
		
		this.transform.localPosition = new Vector3(9999.0f, 9999.0f, 9999.0f);
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
	
	}
}
