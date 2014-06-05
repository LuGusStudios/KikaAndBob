using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class Boundary
{
	public Vector2 min = Vector2.zero;
	public Vector2 max = Vector2.zero;
}




public class Joystick : MonoBehaviour
{
	public enum JoystickDirection
	{
		None = 0,
		Up = 1,
		Right = 2,
		Down = 3,
		Left = 4
	}

	public float joystickRadius = 50f;
	//public float pushScaling = 0.8f;
	public bool cardinalDirectionsOnly = false;
	public bool movableJoystick = true;
	public bool returnSquareCoords = false;
	protected Vector2 originalScreenPos = Vector3.zero;
	protected Vector3 originalPosition = Vector3.zero;
	protected float originalScale = 1.0f;
	protected JoystickDirection currentDirection = JoystickDirection.None;
	protected Transform joystickPad = null;
	protected List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

	/* Dead zone is a central area of customizable size where small movements are not registered */
	public Vector2 deadZone = new Vector2(0.2f, 0.2f);
	/* Normalize output after the dead-zone? */
	public bool normalize = false;
	/* Current tap count */
	public int tapCount = -1;

	
	/* Finger last used on this joystick */
	protected int lastFingerId = -1;
	/* How much time there is left for a tap to occur */
	protected float tapTimeWindow;
	protected Vector2 fingerDownPos;
	protected float firstDeltaTime;
	protected IGameManager gameManager = null;

	public bool isFingerDown
	{
		get
		{
			return (lastFingerId != -1);
		}
	}
	
	public int latchedFinger
	{
		set
		{
			/* If another joystick has latched this finger, then we must release it */
			if (lastFingerId == value)
			{
				ResetJoystick();
			}
		}
	}
	
	/* The position of the joystick on the screen ([-1, 1] in x,y) for clients to read. */
	public Vector2 position
	{
		get;
		private set;
	}

	private static bool enumeratedJoysticks =  false;
	/* A static collection of all joysticks */
	private static List<Joystick> joysticks;
	/* Time allowed between taps */
	private static float tapTimeDelta = 0.3f;
	
	protected void Awake()
	{
		SetUpLocal();

	}

	public void SetUpLocal()
	{
		if (!enumeratedJoysticks)
		{
			joysticks = new List<Joystick>((Joystick[])FindObjectsOfType(typeof(Joystick)));
			enumeratedJoysticks = true;
		}

		if (gameManager == null)
		{
			gameManager = (IGameManager) FindObjectOfType(typeof(IGameManager));
		}

		if (gameManager == null)
		{
			Debug.Log("Joystick: No game manager in this scene!");
		}

		if (joystickPad == null)
		{
			joystickPad = transform.FindChild("JoystickPad");
		}

		if (joystickPad == null)
		{
			Debug.LogError("Joystick: Missing joystick pad. Disabling.");
			this.gameObject.SetActive(false);
		}

		originalPosition = transform.position; 
		originalScreenPos = LugusCamera.ui.WorldToScreenPoint(originalPosition);

		spriteRenderers.AddRange(GetComponentsInChildren<SpriteRenderer>());

		foreach (SpriteRenderer sr in spriteRenderers)
		{
			sr.color = sr.color.a(0.5f);
		}
	}

	
	public void ResetJoystick()
	{
		if (lastFingerId == -1)
			return;

		/* Release the finger control and set the joystick back to the default position */
		//gui.pixelInset = defaultRect;
		transform.position = originalPosition;
		joystickPad.localPosition = Vector3.zero.z(joystickPad.transform.localPosition.z);	// reset pad, but retain z coordinate

		lastFingerId = -1;
		position = Vector2.zero;
		fingerDownPos = Vector2.zero;

		position = Vector2.zero;

		foreach (SpriteRenderer sr in spriteRenderers)
		{
			sr.color = sr.color.a(0.5f);
		}
	}
	
	
	private void Update()
	{
		if (gameManager != null)
		{
			if (!gameManager.GameRunning)
			{
				return;
			}
		}
	
		int count = Input.touchCount;
		
		/* Adjust the tap time window while it still available */
		if (tapTimeWindow > 0)
		{
			tapTimeWindow -= Time.deltaTime;
		}
		else
		{
			tapCount = 0;
		}

		Vector2 lastTouchPosition = Vector2.zero;

		if (count == 0)
		{
			ResetJoystick();
		}
		else
		{
			if (LugusInput.use.down && movableJoystick)
			{
				foreach (SpriteRenderer sr in spriteRenderers)
				{
					sr.color = sr.color.a(1f);
				}

				originalScreenPos = Input.GetTouch(0).position;
				transform.position = LugusCamera.ui.ScreenToWorldPoint(originalScreenPos.v3()).z(transform.position.z);	// center joystick on first touch, but retain z coordinate
			}

			for (int i = 0; i < count; i++)
			{
				Touch touch = Input.GetTouch(i);

				bool shouldLatchFinger = false;

				if (Vector2.Distance(touch.position, originalScreenPos) <= joystickRadius )
				{
					shouldLatchFinger = true;
				}
				
				/* Latch the finger if this is a new touch */
				if (shouldLatchFinger && (lastFingerId == -1 || lastFingerId != touch.fingerId))
				{
					lastFingerId = touch.fingerId; 
					
					/* Accumulate taps if it is within the time window */
					if (tapTimeWindow > 0)
					{
						tapCount++;
					}
					else
					{
						tapCount = 1;
						tapTimeWindow = tapTimeDelta;
					}
					
					/* Tell other joysticks we've latched this finger */
					foreach (Joystick j in joysticks)
					{
						if (j == this)
						{
							continue;
						}

						j.latchedFinger = touch.fingerId;
					}
				}
				
				if (lastFingerId == touch.fingerId)
				{
					lastTouchPosition = touch.position;

					/*
                        Override the tap count with what the iOS SDK reports if it is greater.
                        This is a workaround, since the iOS SDK does not currently track taps
                        for multiple touches.
                    */
					if (touch.tapCount > tapCount)
					{
						tapCount = touch.tapCount;
					}

					Vector3 finalPosition = lastTouchPosition;	// touch position, adjusted for various parameters (clamp etc);

					if (returnSquareCoords)
					{
						DetectSquarePosition(lastTouchPosition);
						print (position);
					}


					float distance = Vector2.Distance(finalPosition, originalScreenPos);
					Vector2 direction = (touch.position - originalScreenPos).normalized;

					// clamp pad to joystick circle
					if (distance > joystickRadius)
					{
						finalPosition = originalScreenPos + (direction * joystickRadius);
					}

					if (cardinalDirectionsOnly)
					{
						float value1 = Mathf.Abs(finalPosition.x - originalScreenPos.x);
						float value2 = Mathf.Abs(finalPosition.y - originalScreenPos.y);

						if ( value1 > value2 )
						{
							if (finalPosition.x > originalScreenPos.x)
								finalPosition = originalScreenPos.xAdd(joystickRadius);
							else if (finalPosition.x < originalScreenPos.x)
								finalPosition = originalScreenPos.xAdd(-1 * joystickRadius);
						}
						else
						{
							if (finalPosition.y > originalScreenPos.y)
								finalPosition = originalScreenPos.yAdd(joystickRadius);
							else if (finalPosition.y < originalScreenPos.y)
								finalPosition = originalScreenPos.yAdd(-1 * joystickRadius);
						}
					}

					lastTouchPosition = finalPosition;

					joystickPad.position = LugusCamera.ui.ScreenToWorldPoint(finalPosition.v3()).z(joystickPad.position.z);

					if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
					{
						ResetJoystick();
					}
				}
			}
			
		}

		if (!returnSquareCoords)
		{
			DetectCircularPosition(lastTouchPosition);
			print (position);
		}

		DetectDirection();
	}

	protected void DetectCircularPosition(Vector2 lastTouchPosition)
	{
		position = Vector2.zero;
		
		if (lastTouchPosition != Vector2.zero)
		{
			position = new Vector2( 
			                       Mathf.Clamp( ( lastTouchPosition.x - originalScreenPos.x ) / joystickRadius, -1.0f, 1.0f ),
			                       Mathf.Clamp( ( lastTouchPosition.y - originalScreenPos.y ) / joystickRadius, -1.0f, 1.0f ));
			
		}
		
		/* Adjust for dead zone */
		float absoluteX = Mathf.Abs(position.x);
		float absoluteY = Mathf.Abs(position.y);
		
		if (absoluteX < deadZone.x)
		{
			/* Report the joystick as being at the center if it is within the dead zone */
			position = new Vector2(0, position.y);
		}
		else if (normalize)
		{
			/* Rescale the output after taking the dead zone into account */
			position = new Vector2(Mathf.Sign(position.x) * (absoluteX - deadZone.x) / (1 - deadZone.x), position.y);
		}
		
		if (absoluteY < deadZone.y)
		{
			/* Report the joystick as being at the center if it is within the dead zone */
			position = new Vector2(position.x, 0);
		}
		else if (normalize)
		{
			/* Rescale the output after taking the dead zone into account */
			position = new Vector2(position.x, Mathf.Sign(position.y) * (absoluteY - deadZone.y) / (1 - deadZone.y));
		}
	}

	protected void DetectSquarePosition(Vector2 lastTouchPosition)
	{
		position = Vector2.zero;



		if (lastTouchPosition != Vector2.zero)
		{
			lastTouchPosition = new Vector2(
				Mathf.Clamp(lastTouchPosition.x, originalScreenPos.x - joystickRadius, originalScreenPos.x + joystickRadius),
				Mathf.Clamp(lastTouchPosition.y, originalScreenPos.y - joystickRadius, originalScreenPos.y + joystickRadius));

			position = new Vector2( 
			                       Mathf.Clamp( ( lastTouchPosition.x - originalScreenPos.x ) / joystickRadius, -1.0f, 1.0f ),
			                       Mathf.Clamp( ( lastTouchPosition.y - originalScreenPos.y ) / joystickRadius, -1.0f, 1.0f ));
			
		}
		
		/* Adjust for dead zone */
		float absoluteX = Mathf.Abs(position.x);
		float absoluteY = Mathf.Abs(position.y);
		
		if (absoluteX < deadZone.x)
		{
			/* Report the joystick as being at the center if it is within the dead zone */
			position = new Vector2(0, position.y);
		}
		else if (normalize)
		{
			/* Rescale the output after taking the dead zone into account */
			position = new Vector2(Mathf.Sign(position.x) * (absoluteX - deadZone.x) / (1 - deadZone.x), position.y);
		}
		
		if (absoluteY < deadZone.y)
		{
			/* Report the joystick as being at the center if it is within the dead zone */
			position = new Vector2(position.x, 0);
		}
		else if (normalize)
		{
			/* Rescale the output after taking the dead zone into account */
			position = new Vector2(position.x, Mathf.Sign(position.y) * (absoluteY - deadZone.y) / (1 - deadZone.y));
		}
	}


	protected void DetectDirection()
	{
		currentDirection = JoystickDirection.None;

		if (position == Vector2.zero)
			return;

		float xDiff = position.x;
		float yDiff = -position.y;
		
		//Debug.DrawLine( mouse, this.transform.position );
		
		float angle = Mathf.Atan2( yDiff, xDiff )  * 180.0f / Mathf.PI;
		angle *= -1;
		angle -= 45.0f;
			

		if (angle < 0 && angle > -90)
		{
			currentDirection = JoystickDirection.Right;
		}
		else if (angle < -90 && angle > -180)
		{
			currentDirection = JoystickDirection.Down;
		}
		else if (angle > 0 && angle < 90)
		{
			currentDirection = JoystickDirection.Up;
		}
		else
		{
			currentDirection = JoystickDirection.Left;
		}
	}

	public bool IsInDirection(JoystickDirection direction)
	{
		return direction == currentDirection;
	}
}