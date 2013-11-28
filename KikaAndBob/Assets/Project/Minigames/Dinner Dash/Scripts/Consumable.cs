using UnityEngine;
using System.Collections;
using System;
using Lugus;

namespace Lugus
{
	// a consumable is either unprocessed (raw food), processed (cooked food) or consumed (food is gone, only platter or empty glass is left)
	public enum ConsumableState 
	{
		Unprocessed = 0,
		Processed = 1,
		Consumed = 2,
		
		NONE = -1 // place at the bottom for nicer auto-complete in IDE
	}
}

// Most often: a piece of food or ingredient of a larger food platter
public class Consumable : MonoBehaviour
{
	public ConsumableDefinition definition = null;

	[SerializeField]
	protected Lugus.ConsumableState _state =  Lugus.ConsumableState.Unprocessed;

	public Lugus.ConsumableState State
	{
		get{ return _state; }
		set
		{
			_state = value;
			UpdateSprite();
		}
	}

	protected SpriteRenderer spriteRenderer = null;

	protected void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		if( spriteRenderer == null )
		{
			Debug.LogError(name + " : No SpriteRenderer attached to this consumable!");
			return;
		}

		if( definition == null )
		{
			Debug.LogError(name + " : No ConsumableDefinition attached to this consumable!");
			return;
		}

		UpdateSprite();
	}

	protected void UpdateSprite()
	{
		spriteRenderer.sprite = definition.TextureForState(this.State);
	}

	/*
	public void Process()
	{
		State = Lugus.ConsumableState.Processed;
	}

	public void Consume()
	{
		State = Lugus.ConsumableState.Consumed;
	}
	*/
}
