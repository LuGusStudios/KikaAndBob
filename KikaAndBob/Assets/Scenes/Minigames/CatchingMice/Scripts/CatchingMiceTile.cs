using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatchingMiceTile
{
    [Flags]
    public enum TileType
    {
        Ground = 1,
        Furniture = 2,
        Collide = 4,
        Trap  = 8,
        Hole  = 16,
        Cheese = 32,
		Obstacle = 64,

        Both = 3,
        GroundTrap = 9,
        FurnitureTrap = 10,
        None = -1 // place at the bottom for nicer auto-complete in IDE
    }
    
	public TileType tileType = TileType.None;

    public CatchingMiceFurniture furniture = null;
    public CatchingMiceTrap trap = null;
	public CatchingMiceCheese cheese = null;
	public CatchingMiceHole hole = null;
    public CatchingMiceWaypoint waypoint = null;
	public CatchingMiceObstacle obstacle = null;
    
	public Vector3 location;
    public Vector2 gridIndices;

	public int Cookies
	{
		get
		{
			return cookies;
		}
	}
	protected int cookies = 0;
	protected GameObject cookieObject = null;

    public CatchingMiceTile()
	{
		tileType = TileType.Ground;
        location = Vector3.zero;
		gridIndices = Vector2.zero;
	}
    
	public void AddCookies(int value)
	{
		if (value < 0)
		{
			CatchingMiceLogVisualizer.use.LogWarning("Adding negative amount of cookies should not be done.");
			return;
		}

		// If no cookies were present at the tile,
		// then we visualize a cookie at the tile location
		if ((cookies == 0)
			&& (value > 0)
			&& (CatchingMiceLevelManager.use.cookiePrefabs.Length > 0))
		{
			// Select a random cookie prefab
			int cookieIndex = LugusRandom.use.Uniform.NextInt(0, CatchingMiceLevelManager.use.cookiePrefabs.Length);

			GameObject cookiePrefab = CatchingMiceLevelManager.use.cookiePrefabs[cookieIndex];

			cookieObject = (GameObject)GameObject.Instantiate(cookiePrefab);
			cookieObject.transform.parent = CatchingMiceLevelManager.use.CookieParent;

			float yOffset = 0f;
			float zOffset = 0.1f;
			if (furniture != null)
			{
				yOffset = furniture.yOffset;
				zOffset += furniture.zOffset;
			}

			cookieObject.transform.localPosition = location.yAdd(yOffset).zAdd(-zOffset);
		}

		cookies += value;
	}

	// TODO: When the HUD is live, visualize the amount of cookies that have
	// been taken by the player, like the feathers in Kika&Bob
	public int TakeCookies(int value)
	{
		if (value < 0)
		{
			CatchingMiceLogVisualizer.use.LogWarning("Taking a negative amount of cookies shouldn't happen.");
			return 0;
		}

		int cookiesTaken = Mathf.Min(value, cookies);
		cookies -= cookiesTaken;

		// If there are no more cookies left, destroy the cookieObject
		if ((cookies == 0) && (cookieObject != null))
		{
			GameObject.Destroy(cookieObject);
			cookieObject = null;
		}

		return cookiesTaken;
	}

	public override string ToString ()
	{
		return "GameTile: " + gridIndices;
	}
}
