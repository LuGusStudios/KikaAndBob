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

	// Basically gets the size of the collider on this sprite, which is usually the 'active' part of that sprite (i.e. interactive or not completely transparent)
	// NOTE: We have multiply the collider's size with scale to allow for sprites with various scales
	public Vector2 GetSurfaceSize()
	{
		Vector3 colliderSize = GetComponent<BoxCollider2D>().size;
		Vector3 localScale = transform.localScale;

		return new Vector2(colliderSize.x * Mathf.Abs(localScale.x), colliderSize.y * Mathf.Abs(localScale.y));
	}
	
	protected virtual void EnterSurfaceEffect(FroggerCharacter character)
	{

	}

	protected virtual void LeaveSurfaceEffect(FroggerCharacter character)
	{
	
	}
}
