using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IDinnerDashConfig : LugusSingletonRuntime<IDinnerDashConfig> 
{

	public List<ConsumableDefinition> CreateOrder(ConsumableDefinition one)
	{
		List<ConsumableDefinition> output = new List<ConsumableDefinition>();

		output.Add ( one );

		return output;
	}
	
	public List<ConsumableDefinition> CreateOrder(ConsumableDefinition one, ConsumableDefinition two)
	{
		List<ConsumableDefinition> output = new List<ConsumableDefinition>();
		
		output.Add ( one );
		output.Add ( two );
		
		return output;
	}
	
	public List<ConsumableDefinition> CreateOrder(ConsumableDefinition one, ConsumableDefinition two, ConsumableDefinition three)
	{
		List<ConsumableDefinition> output = new List<ConsumableDefinition>();
		
		output.Add ( one );
		output.Add ( two );
		output.Add ( three );
		
		return output;
	}
	
	public List<ConsumableDefinition> CreateOrder(ConsumableDefinition one, ConsumableDefinition two, ConsumableDefinition three, ConsumableDefinition four)
	{
		List<ConsumableDefinition> output = new List<ConsumableDefinition>();
		
		output.Add ( one );
		output.Add ( two );
		output.Add ( three );
		output.Add ( four );
		
		return output;
	}
	
	public List<ConsumableDefinition> CreateOrder(ConsumableDefinition one, ConsumableDefinition two, ConsumableDefinition three, ConsumableDefinition four, ConsumableDefinition five)
	{
		List<ConsumableDefinition> output = new List<ConsumableDefinition>();
		
		output.Add ( one );
		output.Add ( two );
		output.Add ( three );
		output.Add ( four );
		output.Add ( five ); 
		
		return output; 
	}

	public List<ConsumableDefinition> RandomOrder( List<ConsumableDefinition> pool, int orderLength = 3 )
	{
		List<ConsumableDefinition> output = new List<ConsumableDefinition>();

		orderLength = Mathf.Min( pool.Count, orderLength );

		while( output.Count < orderLength )
		{
			ConsumableDefinition chosen = null;

			do
			{
				chosen = pool[ Random.Range(0, pool.Count) ];
			}
			while( output.Contains(chosen) );

			output.Add ( chosen );
		}


		return output;
	}
}
