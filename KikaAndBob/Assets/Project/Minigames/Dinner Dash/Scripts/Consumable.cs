using UnityEngine;
using System.Collections;

namespace Lugus
{
	public enum ConsumableState
	{
		Unprocessed = 0,
		Processed = 1,
		Consumed = 2,
		
		NONE = -1 // place at the bottom for nicer auto-complete in IDE
	}
}

public class Consumable : MonoBehaviour
{
	public ConsumableDefinition definition = null;
	public Lugus.ConsumableState state = Lugus.ConsumableState.Unprocessed;

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
		if( state == Lugus.ConsumableState.Unprocessed )
		{
			spriteRenderer.sprite = definition.textureUnprocessed;
		}
		else if( state == Lugus.ConsumableState.Processed )
		{
			spriteRenderer.sprite = definition.textureProcessed;
		}
		else if( state == Lugus.ConsumableState.Consumed )
		{
			spriteRenderer.sprite = definition.textureConsumed;
		}
	}

	public void Process()
	{
		state = Lugus.ConsumableState.Processed;

		UpdateSprite ();
	}

	public void Consume()
	{
		state = Lugus.ConsumableState.Consumed;
		
		UpdateSprite ();
	}
}
