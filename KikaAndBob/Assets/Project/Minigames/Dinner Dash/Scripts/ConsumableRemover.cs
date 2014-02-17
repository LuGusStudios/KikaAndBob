using UnityEngine;
using System.Collections;

// removes all current consumables from the Mover
// example: garbage bin
public class ConsumableRemover : IConsumableUser
{
	public string removeSound = "";

	protected void Awake()
	{
	}
	
	public override bool Use()
	{
		// TODO: graphics update?
		// TODO: remove points for wasting food for consumables that didn't have Consumed state

		float scoreAmount = 0.0f;

		ConsumableMover mover = DinnerDashManager.use.Mover;
		foreach( Consumable consumable in mover.consumedItems )
		{
			GameObject.Destroy( consumable.gameObject );
			scoreAmount += 15.0f;
		}
		mover.consumedItems.Clear();

		bool hasScore = false;
		if( scoreAmount > 0.0f )
		{
			DinnerDashManager.use.moneyScore += scoreAmount;
			hasScore = true;

			ScoreVisualizer
				.Score ( KikaAndBob.CommodityType.Money, scoreAmount )
					.Audio ("Blob01")
					.Color ( Color.white )
					.Position( this.transform.position.yAdd ( 100.0f ) ) // 100 pixels
					.Text ( "" + Mathf.FloorToInt(scoreAmount) )
					.TitleKey("dinerdash.score.title.3")
					.Time(3.0f)
					.Execute();
		}

		scoreAmount = 0.0f;
		foreach( Consumable consumable in mover.unprocessedItems )
		{
			scoreAmount -= 10.0f;
			GameObject.Destroy( consumable.gameObject );
			// TODO: decrement score
		}
		mover.unprocessedItems.Clear();
		mover.unprocessedVisualizer.Hide ();

		foreach( Consumable consumable in mover.processedItems )
		{
			scoreAmount -= 10.0f;
			GameObject.Destroy( consumable.gameObject );
			// TODO: decrement score
		}
		mover.processedItems.Clear();
		mover.processedVisualizer.Hide();

		if( scoreAmount < 0.0f )
		{
			DinnerDashManager.use.moneyScore += scoreAmount;

			Vector3 position = this.transform.position.yAdd ( 100.0f );
			if( hasScore )
			{
				// if we had a positive score, spawn this score a little to the right to make sure we don't overlap
				position = position.xAdd( 200.0f ); 
			}

			ScoreVisualizer
				.Score ( KikaAndBob.CommodityType.Money, scoreAmount )
					.Audio ("BugSquash01")
					.MinValue( 0.0f )
					.Color ( Color.red )
					.Position( position ) // 100 pixels
					.Text ( "" + Mathf.FloorToInt(scoreAmount) )
					.TitleKey("dinerdash.score.title.dumpFood")
					.Time(3.0f)
					.Execute();
		}


		LugusAudio.use.SFX().Play ( LugusResources.use.Shared.GetAudio(removeSound) );
		
		return true;
	}
}
