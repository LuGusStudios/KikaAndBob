using UnityEngine;
using System.Collections;

public class PacmanInput : LugusSingletonRuntime<PacmanInput> 
{
	protected DirectionPad directionPad = null;

	protected void Start()
	{
		SetUpGlobal();
	}

	public void SetUpGlobal()
	{
		if (directionPad == null)
		{
			directionPad = GameObject.FindObjectOfType<DirectionPad>();
		}
		
		if (directionPad == null)
		{
			Debug.LogWarning("PacmanInput: No direction pad found. Continuing without.");
		}
	}

	// TO DO: Add other input methods to trigger the same booleans
	public bool GetUp()
	{
		if (LugusInput.use.KeyDown(KeyCode.UpArrow) || (directionPad != null && directionPad.IsDirection(Joystick.JoystickDirection.Up) ))
			return true;

		return false;
	}

	public bool GetDown()
	{
		if (LugusInput.use.KeyDown(KeyCode.DownArrow) || (directionPad != null && directionPad.IsDirection(Joystick.JoystickDirection.Down) ))
			return true;

		return false;
	}

	public bool GetRight()
	{
		if (LugusInput.use.KeyDown(KeyCode.RightArrow) || (directionPad != null && directionPad.IsDirection(Joystick.JoystickDirection.Right) ))
			return true;

		return false;
	}

	public bool GetLeft()
	{
		if (LugusInput.use.KeyDown(KeyCode.LeftArrow) || (directionPad != null && directionPad.IsDirection(Joystick.JoystickDirection.Left) ))
			return true;

		return false;
	}

	public bool GetUpContinuous()
	{
		if (LugusInput.use.Key(KeyCode.UpArrow) || (directionPad != null && directionPad.IsDirection(Joystick.JoystickDirection.Up) ))
			return true;
		
		return false;
	}
	
	public bool GetDownContinuous()
	{
		if (LugusInput.use.Key(KeyCode.DownArrow) || (directionPad != null && directionPad.IsDirection(Joystick.JoystickDirection.Down) ))
			return true;
		
		return false;
	}
	
	public bool GetRightContinuous()
	{
		if (LugusInput.use.Key(KeyCode.RightArrow) || (directionPad != null && directionPad.IsDirection(Joystick.JoystickDirection.Right) ))
			return true;
		
		return false;
	}
	
	public bool GetLeftContinuous()
	{
		if (LugusInput.use.Key(KeyCode.LeftArrow) || (directionPad != null && directionPad.IsDirection(Joystick.JoystickDirection.Left) ))
			return true;
		
		return false;
	}

	
 	//TO DO: Placeholder
	public bool GetAction1()
	{
		if (LugusInput.use.KeyDown(KeyCode.Space))
			return true;

		return false;
	}
}
