using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanPickups : LugusSingletonExisting<PacmanPickups>
{
	public Dictionary<string, int> pickups = new Dictionary<string, int>();

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
	
	}

	public void RegisterPickup(string id)
	{
		if (!pickups.ContainsKey(id))
		{
			pickups.Add(id, 0);
			Debug.Log("Registered pickup: " + id);
			//ModifyPickupAmount(id, 0);	// modify amount once, to reset the display to 0
			PacmanGUIManager.use.UpdateKeyGUIItems();
		} 
	}

	public void ModifyPickupAmount(string id, int amount)
	{
		if (pickups.ContainsKey(id))
		{
			pickups[id] += amount;

			if (pickups[id] < 0)
			{
				pickups[id] = 0;
			}

			PacmanGUIManager.use.DisplayKeyAmount(id, pickups[id]);

			Debug.Log("Modified pickup: " + id + " by " + amount + " to " + pickups[id]);
		}
		else
		{
			Debug.LogError("Pickups list does not contain ID: " + id);
		}
	}

	public int GetPickupAmount(string id)
	{
		if (pickups.ContainsKey(id))
		{
			return pickups[id];
		}
		else
		{
			Debug.LogError("Pickups list does not contain ID: " + id);
			return 0;
		}
	}

	public void ClearPickups()
	{
		pickups.Clear();
		PacmanGUIManager.use.ClearKeyGUI();
	}
}
