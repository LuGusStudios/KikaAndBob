using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DirectionPad : MonoBehaviour 
{
	protected Joystick.JoystickDirection currentDirection = Joystick.JoystickDirection.None;
	protected DirectionPadButton lastButtonUsed = null;
	protected List<DirectionPadButton> directionPadButtons = new List<DirectionPadButton>();


	public void SetupLocal()
	{
		if (directionPadButtons.Count == 0)
		{
			directionPadButtons.AddRange(GetComponentsInChildren<DirectionPadButton>(true));
		}
	}
	
	public void SetupGlobal()
	{
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	protected int lastFingerId = -1;


	
	protected void Update () 
	{
		DirectionPadButton currentButton = null;
		Transform hit = null;
		int currentTouchIndex = -1;

		if (Input.touchCount > 0)
		{
			if (lastFingerId == -1)
			{
				currentTouchIndex = 0;
				lastFingerId =  Input.GetTouch(currentTouchIndex).fingerId;
			}
			else
			{
				bool sameFingerFound = false;
				for (int i = 0; i < Input.touchCount; i++) 
				{
					if (Input.GetTouch(i).fingerId == lastFingerId)
					{
						currentTouchIndex = i;
						sameFingerFound = true;
						break;
					}
				}

				if (!sameFingerFound)
				{
					currentTouchIndex = 0;
					lastFingerId = Input.GetTouch(currentTouchIndex).fingerId;
				}
			}
		}
		else
		{
			lastFingerId = -1;
		}

		if (currentTouchIndex > -1)
		{
			hit = LugusInput.use.RaycastFromScreenPoint( Input.GetTouch(currentTouchIndex).position);
		}

		if (hit != null)
		{
			foreach(DirectionPadButton button in directionPadButtons)
			{
				if (hit == button.transform)
				{
					currentButton = button;
					break;
				}
			}
		}
		else
		{
			currentDirection = Joystick.JoystickDirection.None;
		}

		if (currentButton != lastButtonUsed)
		{
			if (currentButton != null)
			{
				currentDirection = currentButton.direction;
				currentButton.SetPressed(true);
			}

			if (lastButtonUsed != null)
			{
				lastButtonUsed.SetPressed(false);
			}
		}

		lastButtonUsed = currentButton;
	}

	public bool IsDirection(Joystick.JoystickDirection direction)
	{
		return currentDirection == direction;
	}

	public bool IsDirectionDown(Joystick.JoystickDirection direction)
	{
		return currentDirection == direction && LugusInput.use.down;
	}

	public bool IsDirectionUp(Joystick.JoystickDirection direction)
	{
		return currentDirection == direction && LugusInput.use.up;
	}
}
