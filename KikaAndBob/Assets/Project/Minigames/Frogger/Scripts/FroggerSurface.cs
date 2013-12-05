using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public abstract class FroggerSurface : MonoBehaviour {

	private bool onSurface = false;

	public void Enter(FroggerCharacter character)
	{
		if (onSurface)
			return;
		
		onSurface = true;

		EnterSurfaceEffect(character);
	}

	public void Leave(FroggerCharacter character)
	{
		if (!onSurface)
			return;
		
		onSurface = false;

		LeaveSurfaceEffect(character);
	}

	public Vector2 GetSurfaceSize()
	{
		return GetComponent<BoxCollider2D>().size;
	}
	
	protected virtual void EnterSurfaceEffect(FroggerCharacter character)
	{

	}

	protected virtual void LeaveSurfaceEffect(FroggerCharacter character)
	{
	
	}
}
