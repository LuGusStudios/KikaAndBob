using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IDinnerDashConfig : MonoBehaviour 
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
}
