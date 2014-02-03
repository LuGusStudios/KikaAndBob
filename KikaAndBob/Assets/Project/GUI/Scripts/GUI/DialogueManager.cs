using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace KikaAndBob
{
	/*
	public enum ScreenLocation
	{
		NONE = -1,

		TopLeft = 1,
		TopRight = 2,
		BottomLeft = 3,
		BottomRight = 4,

		Center = 5
	}
	*/

	//  if( (target & FR.Target.NUMERATOR) == FR.Target.NUMERATOR )  // TEST if we are this
	// quadrant = quadrant | (int) Bo.MovementQuadrant.LEFT; // append to existing int value

	[Flags]
	public enum ScreenAnchor
	{
		NONE = -1,

		Left = 1, // also CenterLeft
		Right = 2, // also CenterRight
		Center = 4, 
		Top = 8, // also TopCenter basically
		Bottom = 16, // also BottomCenter basically

		TopLeft = 9,
		TopRight = 10,

		BottomLeft = 17,
		BottomRight = 18
	}

	public class ScreenAnchorHelper
	{
		public static KikaAndBob.ScreenAnchor GetQuadrant(Transform worldObj)
		{
			Vector2 screenPoint = LugusCamera.game.WorldToScreenPoint( worldObj.position );
			float halfWidth = Screen.width / 2.0f;
			float halfHeight = Screen.height / 2.0f;
			
			if( screenPoint.x < halfWidth && screenPoint.y > halfHeight )
			{
				return KikaAndBob.ScreenAnchor.TopLeft;
			}
			else if( screenPoint.x > halfWidth && screenPoint.y > halfHeight )
			{
				return KikaAndBob.ScreenAnchor.TopRight;
			}
			else if( screenPoint.x < halfWidth && screenPoint.y < halfHeight )
			{
				return KikaAndBob.ScreenAnchor.BottomLeft;
			}
			
			else if( screenPoint.x > halfWidth && screenPoint.y < halfHeight )
			{
				return KikaAndBob.ScreenAnchor.BottomRight;
			}
			
			return KikaAndBob.ScreenAnchor.NONE;
		}

		public static Vector2 GetQuadrantCenter(KikaAndBob.ScreenAnchor quadrant, Rect container)
		{		
			// container is defined with origin BOTTOM LEFT

			float quartWidth  = container.width  / 4.0f;
			float quartHeight = container.height  / 4.0f;

			// default is center of the "screen"
			float x = 2 * quartWidth;
			float y = 2 * quartHeight;

			if( (quadrant & ScreenAnchor.Left) == ScreenAnchor.Left )
			{
				x = quartWidth;
			}

			if( (quadrant & ScreenAnchor.Right) == ScreenAnchor.Right )
			{
				x = 3 * quartWidth;
			}

			if( (quadrant & ScreenAnchor.Top) == ScreenAnchor.Top )
			{
				y = 3 * quartHeight;
			}
			
			if( (quadrant & ScreenAnchor.Bottom) == ScreenAnchor.Bottom )
			{
				y = quartHeight;
			}

			return new Vector2( x, y );
		}

		// rect is with origin at BOTTOM LEFT
		public static Rect GetQuadrant(KikaAndBob.ScreenAnchor quadrant, Rect container)
		{
			// TODO: when passed CENTER als quadrant, this will fail!!! (will return topRight...)

			float halfWidth  = container.width  / 2.0f;
			float halfHeight = container.height  / 2.0f;

			float x = halfWidth; // CENTER + RIGHT
			float y = halfHeight; // CENTER + TOP

			if( (quadrant & ScreenAnchor.Left) == ScreenAnchor.Left )
			{
				x = 0.0f;
			}
			
			if( (quadrant & ScreenAnchor.Bottom) == ScreenAnchor.Bottom )
			{
				y = 0.0f;
			}

			return new Rect( x, y, halfWidth, halfHeight );
		}

		// container is with origin at BOTTOM LEFT
		// subject is with origin at CENTER
		// returns a CENTER position for subject to be put upon
		public static Vector2 ExtendTowards(ScreenAnchor anchor, Rect subject, Rect container, Vector2 padding)
		{
			float subjectHalfWidth = subject.width / 2.0f;
			float subjectHalfHeight = subject.height / 2.0f;

			float containerHalfWidth = container.width / 2.0f;
			float containerHalfHeight = container.height / 2.0f;

			// center of the quadrant
			float xOffset = containerHalfWidth;
			float yOffset = containerHalfHeight;

			if( (anchor & ScreenAnchor.Left) == ScreenAnchor.Left )
			{
				xOffset = subjectHalfWidth + padding.x;
			}
			
			if( (anchor & ScreenAnchor.Right) == ScreenAnchor.Right )
			{
				xOffset = container.width - (subjectHalfWidth + padding.x);
			}

			
			if( (anchor & ScreenAnchor.Top) == ScreenAnchor.Top )
			{
				yOffset = container.height - (subjectHalfHeight + padding.y);
			}
			
			if( (anchor & ScreenAnchor.Bottom) == ScreenAnchor.Bottom )
			{
				yOffset = subjectHalfHeight + padding.y;
			}


			return new Vector2( container.x + xOffset, container.y + yOffset );
		}
	}
}

public class DialogueManager : LugusSingletonExisting<DialogueManager> 
{




	public DialogueBox CreateBox( string text, Sprite icon = null )
	{
		DialogueBox output = null;
		foreach( DialogueBox box in boxes )
		{
			if( box.available )
			{
				output = box;
				break;
			}
		}
		
		if( output == null )
		{
			output = (DialogueBox) GameObject.Instantiate( boxPrefab );
			boxes.Add( boxPrefab );
		}
		
		output.available = false;
		
		output.text = text;
		output.icon.sprite = icon;

		output.transform.parent = boxPrefab.transform.parent;

		return output;
	}

	public DialogueBox CreateBox( Transform avoid, string text, Sprite icon = null )
	{
		KikaAndBob.ScreenAnchor quadrant = KikaAndBob.ScreenAnchor.Center; 
		
		if( avoid != null )
		{
			KikaAndBob.ScreenAnchor avoidQuadrant = KikaAndBob.ScreenAnchorHelper.GetQuadrant( avoid );
			Debug.LogWarning("Avoiding " + avoid.Path() + " which is in quadrant " + avoidQuadrant );

			if( avoidQuadrant == KikaAndBob.ScreenAnchor.TopLeft || avoidQuadrant == KikaAndBob.ScreenAnchor.BottomLeft )
				quadrant = KikaAndBob.ScreenAnchor.TopRight;
		}
		
		return CreateBox( quadrant, text, icon );
	}
	
	public DialogueBox CreateBox( KikaAndBob.ScreenAnchor location, string text, Sprite icon = null )
	{
		DialogueBox output = CreateBox ( text, icon );

		output.Reposition(location);
		
		return output;
	}


	public DialogueBox boxPrefab = null;
	public List<DialogueBox> boxes = new List<DialogueBox>();

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
		if( boxPrefab == null )
		{
			boxPrefab = GameObject.FindObjectOfType<DialogueBox>();
		}

		if( boxPrefab == null )
		{
			Debug.LogError( transform.Path () + " : no BoxPrefab known for DialogueManager!");
		}
		else
		{
			boxes.Add( boxPrefab ); // use the prefab itself as well: no waste!
		}
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script

		// TEST 
		// TODO: Remove this!
		CreateBox(GameObject.Find ("Burger").transform, "Deze klant wil een stoofpotje!\nKlik op de groenten.", null ).Show ();

		
		DialogueBox box = CreateBox("Deze klant wil een stoofpotje 2!\nKlik op de groenten.", null );
		box.Reposition(KikaAndBob.ScreenAnchor.TopLeft, KikaAndBob.ScreenAnchor.TopLeft );
		box.Show();

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
