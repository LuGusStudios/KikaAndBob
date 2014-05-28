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
	public enum JoystickCardinalDirection
	{
		None = 0,
		Up = 1,
		Right = 2,
		Down = 3,
		Left = 4
	}

	public float joystickRadius = 50f;
	public float pushScaling = 0.8f;
	protected Vector2 originalScreenPos = Vector3.zero;
	protected Vector3 originalPosition = Vector3.zero;
	protected float originalScale = 1.0f;
	protected JoystickCardinalDirection currentDirection = JoystickCardinalDirection.None;

	/* Dead zone is a central area of customizable size where small movements are not registered */
	public Vector2 deadZone = new Vector2(0.2f, 0.2f);
	/* Normalize output after the dead-zone? */
	public bool normalize = false;
	/* Current tap count */
	public int tapCount = -1;

	
	/* Finger last used on this joystick */
	private int lastFingerId = -1;
	/* How much time there is left for a tap to occur */
	private float tapTimeWindow;
	private Vector2 fingerDownPos;
	private float firstDeltaTime;

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
	
	/* A static collection of all joysticks */
	private static List<Joystick> joysticks;
	/* Time allowed between taps */
	private static float tapTimeDelta = 0.3f;
	
	private void Awake()
	{
		joysticks = new List<Joystick>((Joystick[])FindObjectsOfType(typeof(Joystick)));

		originalPosition = transform.position; 
		originalScreenPos = LugusCamera.ui.WorldToScreenPoint(originalPosition);
		originalScale = transform.localScale.x;
	}

	
	public void ResetJoystick()
	{
		/* Release the finger control and set the joystick back to the default position */
		//gui.pixelInset = defaultRect;
		transform.position = originalPosition;
		transform.localScale = transform.localScale.x(originalScale);

		lastFingerId = -1;
		position = Vector2.zero;
		fingerDownPos = Vector2.zero;
	}
	
	
	private void Update()
	{
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


					Vector3 finalPosition = touch.position;
					float distance = Vector2.Distance(finalPosition, originalScreenPos);
					Vector2 direction = (touch.position - originalScreenPos).normalized;

					if (distance > joystickRadius)
					{
						finalPosition = originalScreenPos + (direction * joystickRadius);
					}

					transform.position = LugusCamera.ui.ScreenToWorldPoint(finalPosition.v3()).z(transform.position.z);
					
					if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
					{
						ResetJoystick();
					}
				}
			}
			
		}
		

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

		DetectDirection();
	}

	protected void DetectDirection()
	{
		currentDirection = JoystickCardinalDirection.None;

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
			currentDirection = JoystickCardinalDirection.Right;
		}
		else if (angle < -90 && angle > -180)
		{
			currentDirection = JoystickCardinalDirection.Down;
		}
		else if (angle > 0 && angle < 90)
		{
			currentDirection = JoystickCardinalDirection.Up;
		}
		else
		{
			currentDirection = JoystickCardinalDirection.Left;
		}
	}
}