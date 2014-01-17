using UnityEngine;
using System.Collections;

public class PacmanInput : LugusSingletonRuntime<PacmanInput> 
{
	// TO DO: Add other input methods to trigger the same booleans
	public bool GetUp()
	{
		if (LugusInput.use.KeyDown(KeyCode.UpArrow))
			return true;

		return false;
	}

	public bool GetDown()
	{
		if (LugusInput.use.KeyDown(KeyCode.DownArrow))
			return true;

		return false;
	}

	public bool GetRight()
	{
		if (LugusInput.use.KeyDown(KeyCode.RightArrow))
			return true;

		return false;
	}

	public bool GetLeft()
	{
		if (LugusInput.use.KeyDown(KeyCode.LeftArrow))
			return true;

		return false;
	}

	public bool GetUpContinuous()
	{
		if (LugusInput.use.Key(KeyCode.UpArrow))
			return true;
		
		return false;
	}
	
	public bool GetDownContinuous()
	{
		if (LugusInput.use.Key(KeyCode.DownArrow))
			return true;
		
		return false;
	}
	
	public bool GetRightContinuous()
	{
		if (LugusInput.use.Key(KeyCode.RightArrow))
			return true;
		
		return false;
	}
	
	public bool GetLeftContinuous()
	{
		if (LugusInput.use.Key(KeyCode.LeftArrow))
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
