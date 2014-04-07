using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MachineButton : LugusSingletonExisting<MachineButton> 
{
	public Sprite PressedState = null;

	public void Press()
	{
		this.GetComponent<SpriteRenderer>().sprite = PressedState;

		LugusAudio.use.SFX().Play( LugusResources.use.Shared.GetAudio("Button03") );
	}

	public void Awake()
	{
		if( PressedState == null )
		{ 
			Debug.LogError(this.name + " : no PressedState sprite assigned!");
		}
	}
}
