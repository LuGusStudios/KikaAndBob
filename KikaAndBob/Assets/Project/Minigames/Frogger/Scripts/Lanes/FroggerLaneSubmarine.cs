using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLaneSubmarine : FroggerLane 
{
	public float submarineMoveSpeed = 5f;

	protected List<FroggerLaneItemIceBreakingSubmarine> submarines = new List<FroggerLaneItemIceBreakingSubmarine>();
	protected FroggerCharacter character = null;

	private bool createdIceHole = false;

	protected override void FillStaticItems()
	{
		if (staticSpawnItems.Count < 1)
		{
			return;
		}

#if UNITY_EDITOR
		// in editor, SetUpLocal() isn't called, which can give problems with surface collider still being null when its required below, so call it once
		if (surfaceCollider == null)
		{
			SetUpLocal();
		}
#endif

		foreach(KeyValuePair<float, FroggerLaneItem> item in staticSpawnItems)
		{
			SpawnItem(item);
		}

	}

	protected override void EnterSurfaceEffect(FroggerCharacter character)
	{
		base.EnterSurfaceEffect(character);
		this.character = character;
	}

	protected override void LeaveSurfaceEffect(FroggerCharacter character)
	{
		base.LeaveSurfaceEffect(character);
	}

	protected override void Update()
	{
		if (!FroggerGameManager.use.gameRunning || (character == null))
		{
			return;
		}

		if (submarines.Count < 1)
		{
			return;
		}

		// Only control one submarine at a time, so that not all submarines in a lane
		// are closing in on the character at once.
		FroggerLaneItemIceBreakingSubmarine sub = submarines[0];

		// If the submarine is under water, let it close in on the character
		if (sub.state == FroggerLaneItemIceBreakingSubmarine.State.UNDER)
		{
			float factor = 1f;
			if (sub.transform.position.x > character.transform.position.x)
			{
				factor = -1f;
			}

			sub.transform.Translate(new Vector3(submarineMoveSpeed * Time.deltaTime * factor, 0f, 0f), Space.World);

			// If the submarine gets close enough, then let it break through the ice
			BoxCollider2D subCollider = sub.GetComponent<BoxCollider2D>();
			if (Mathf.Abs(sub.transform.position.x - character.transform.position.x) < (subCollider.size.x /2))
			{
				sub.Surface();
			}
		}
		else if ((sub.state == FroggerLaneItemIceBreakingSubmarine.State.SUBMARINE) && (!createdIceHole))
		{
			// Create a ice hole where the submarine was
			if (sub.iceHolePrefab != null)
			{
				float pos = Mathf.InverseLerp(-(laneSize.x * 0.5f), laneSize.x * 0.5f, sub.transform.localPosition.x);
				KeyValuePair<float, FroggerLaneItem> iceHole = new KeyValuePair<float, FroggerLaneItem>(pos, sub.iceHolePrefab);
				SpawnItem(iceHole);
			}

			createdIceHole = true;
		}
		else if (sub.state == FroggerLaneItemIceBreakingSubmarine.State.DONE)
		{
			
			// Destroy the submarine
			submarines.RemoveAt(0);
			GameObject.Destroy(sub.gameObject);
			createdIceHole = false;

			if (!onSurface)
			{
				character = null;
			}
		}
	}

	private void SpawnItem(KeyValuePair<float, FroggerLaneItem> item)
	{
		GameObject spawned = (GameObject)Instantiate(item.Value.gameObject);

		spawned.transform.parent = this.transform;
		spawned.transform.localPosition = Vector3.zero;
		spawned.transform.localRotation = Quaternion.identity;

		if (item.Value.behindPlayer) // center transform, so first subtract half the lane size, then position between 0 - 1 lane Length
			spawned.transform.localPosition = new Vector3(-(laneSize.x * 0.5f) + ((laneSize.x * item.Key)), 0, -1);
		else
			spawned.transform.localPosition = new Vector3(-(laneSize.x * 0.5f) + ((laneSize.x * item.Key)), 0, -10);


		// make the height of the spawned item's collider equal to the lane's height - this way it will vertically cover the entire lane no matter what the height that was set
		BoxCollider2D itemCollider = spawned.GetComponent<BoxCollider2D>();

		itemCollider.size = itemCollider.size.y(height / itemCollider.transform.localScale.y); // compensate for potential sprite scaling !
		itemCollider.center = itemCollider.center.y(surfaceCollider.center.y);

		FroggerLaneItem laneItemScript = spawned.GetComponent<FroggerLaneItem>();
		staticSpawnedItems.Add(laneItemScript);

		FroggerLaneItemIceBreakingSubmarine submarine = spawned.GetComponent<FroggerLaneItemIceBreakingSubmarine>();

		if (submarine != null)
		{
			// Initialize submarine
			spawned.transform.localPosition = new Vector3(-(laneSize.x * 0.5f) + ((laneSize.x * item.Key)), 0, -0.5f);
			submarines.Add(submarine);
		}
	}

}
