using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DirectionPad : MonoBehaviour 
{
	protected Joystick.JoystickDirection currentDirection = Joystick.JoystickDirection.None;
	protected DirectionPadButton lastButtonUsed = null;
	protected List<DirectionPadButton> directionPadButtons = new List<DirectionPadButton>();
	protected float activationTimer = 0.0f;

	public void SetupLocal()
	{
		if (directionPadButtons.Count == 0)
		{
			directionPadButtons.AddRange(GetComponentsInChildren<DirectionPadButton>(true));
		}


//		if (up == null)
//			up = transform.FindChild("Up");
//		if (up == null)
//			Debug.LogError("DirectionPad: Missing up transform");
//
//		if (right == null)
//			right = transform.FindChild("Right");
//		if (right == null)
//			Debug.LogError("DirectionPad: Missing right transform");
//
//		if (down == null)
//			down = transform.FindChild("Down");
//		if (down == null)
//			Debug.LogError("DirectionPad: Missing down transform");
//
//		if (left == null)
//			left = transform.FindChild("Left");
//		if (left == null)
//			Debug.LogError("DirectionPad: Missing left transform");
//
//		if (buttons.Count <= 0)
//		{
//			buttons.Add(up, Joystick.JoystickDirection.Up);
//			buttons.Add(right, Joystick.JoystickDirection.Right);
//			buttons.Add(down, Joystick.JoystickDirection.Down);
//			buttons.Add(left, Joystick.JoystickDirection.Left);
//		}
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
		currentDirection = Joystick.JoystickDirection.None;
		DirectionPadButton currentButton = null;
		Transform hit = null;

		if (Input.touchCount > 0)
		{
			hit = LugusInput.use.RaycastFromScreenPoint(Input.GetTouch(0).position);
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


		if (currentButton != lastButtonUsed)
		{
			if (currentButton != null)
			{
				currentDirection = currentButton.direction;
				currentButton.SetPressed(true);
			}

			if (lastButtonUsed != null)
				lastButtonUsed.SetPressed(false);
		}


		lastButtonUsed = currentButton;
	}
}
