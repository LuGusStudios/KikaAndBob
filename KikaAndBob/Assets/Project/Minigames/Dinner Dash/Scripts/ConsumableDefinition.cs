using UnityEngine;
using System.Collections;

// Artist frontend to assign different textures for different states of a consumable
// is a separate object so we don't have to work with enums to indicate the types of different consumables for each level
// we just assign ConsumableDefinitions to various ConsumableProcessor's or ConsumableConsumers
public class ConsumableDefinition : ScriptableObject 
{
	public Sprite textureUnprocessed = null;
	public Sprite textureProcessed = null;
	public Sprite textureConsumed = null;

	public bool isPayment = false;

	public Sprite TextureForState(Lugus.ConsumableState state)
	{
		if( state == Lugus.ConsumableState.Unprocessed )
		{
			return textureUnprocessed;
		}
		else if( state == Lugus.ConsumableState.Processed )
		{
			return textureProcessed;
		}
		else if( state == Lugus.ConsumableState.Consumed )
		{
			return textureConsumed;
		}
		else
		{
			return null;
		}
	}
}
