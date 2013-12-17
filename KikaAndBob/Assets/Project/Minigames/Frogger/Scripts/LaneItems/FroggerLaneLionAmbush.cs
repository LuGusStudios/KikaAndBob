using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class FroggerLaneLionAmbush : FroggerLane {

	public FroggerCharacter lastCharToEnter = null;
	public float lionMoveSpeed = 1;
	public string lionIdleAnimation = "Lion_Idle";
	public string lionRunAnimation = "Lion_Running";
	public float minimumSwitchTime = 1;
	public float maximumSwitchTime = 5;
	protected bool windFromLeft = true;
	protected Dictionary <FroggerLaneItem, BoneAnimation> lionAnimations = new Dictionary<FroggerLaneItem, BoneAnimation>();
	protected DataRange delayTime;
	protected bool chasingPlayer = false;

	void Start()
	{
		windFromLeft = Random.value > 0.5f ? true : false;
		delayTime = new DataRange(minimumSwitchTime, maximumSwitchTime);
		LugusCoroutines.use.StartRoutine(SwitchDirection());
	}

	protected IEnumerator SwitchDirection()
	{
		while (Application.isPlaying)
		{
			if (!chasingPlayer)
				ToggleDirection();

			yield return new WaitForSeconds(delayTime.Random());
		}

		yield break;
	}

	protected void ToggleDirection()
	{
		windFromLeft = !windFromLeft;

		foreach(FroggerLaneItem item in staticSpawnedItems)
		{
			if (windFromLeft)
			{
				if (item.transform.localScale.x > 0)
				{
					item.transform.localScale = item.transform.localScale.x(- Mathf.Abs(item.transform.localScale.x));
				}
			}
			else
			{
				if (item.transform.localScale.x < 0)
				{
					item.transform.localScale = item.transform.localScale.x(Mathf.Abs(item.transform.localScale.x));
				}
			}
		}
	}

	protected override void EnterSurfaceEffect (FroggerCharacter character)
	{
		lastCharToEnter = character;
	}

	protected override void LeaveSurfaceEffect (FroggerCharacter character)
	{
		foreach(BoneAnimation ba in lionAnimations.Values)
		{
			ba.Play(lionIdleAnimation);
		}

		chasingPlayer = false;
	}

	// pretty much the same as the original, except for where it sets lions to idle at the start of the level
	protected override void FillStaticItems()
	{
		if (staticSpawnItems.Count < 1)
			return;
		
		foreach(KeyValuePair<float, FroggerLaneItem> item in staticSpawnItems)
		{
			GameObject spawned = (GameObject) Instantiate(item.Value.gameObject);
			
			spawned.transform.parent = this.transform;
			spawned.transform.localPosition = Vector3.zero;
			spawned.transform.localRotation = Quaternion.identity;
			
			if (item.Value.behindPlayer) // center transform, so first subtract half the lane size, then position between 0 - 1 lane Length
				spawned.transform.localPosition = new Vector3(-(laneSize.x * 0.5f) + ((laneSize.x * item.Key)), 0, -1);
			else
				spawned.transform.localPosition = new Vector3(-(laneSize.x * 0.5f) + ((laneSize.x * item.Key)), 0, -10);
			
			
			// make the height of the spawned item's collider equal to the lane's height - this way it will vertically cover the entire lane no matter what the height that was set
			BoxCollider2D itemCollider = spawned.GetComponent<BoxCollider2D>();
			itemCollider.size = itemCollider.size.y(height/itemCollider.transform.localScale.y); // compensate for potential sprite scaling !
			itemCollider.center = itemCollider.center.y(boxCollider2D.center.y);

			FroggerLaneItem laneItemScript = spawned.GetComponent<FroggerLaneItem>();
			staticSpawnedItems.Add(laneItemScript);

			BoneAnimation ba = spawned.GetComponentInChildren<BoneAnimation>();

			if (ba != null)
			{
				ba.Stop();
				ba.Play(lionIdleAnimation, PlayMode.StopAll);

				lionAnimations.Add(laneItemScript, ba);
			}
		}
	}


	void Update()
	{
		if (!onSurface || !FroggerGameManager.use.gameRunning)
			return;

		foreach(FroggerLaneItem item in staticSpawnedItems)
		{
			if (windFromLeft)
			{
				if (chasingPlayer)
				{
					item.transform.Translate(new Vector3(-lionMoveSpeed * Time.deltaTime, 0, 0), Space.World);
				}
				else
				{
					if (lastCharToEnter.transform.localPosition.x < item.transform.localPosition.x)
					{
						if (lionAnimations.ContainsKey(item) && !lionAnimations[item].IsPlaying(lionRunAnimation))
						{
							lionAnimations[item].Play(lionRunAnimation);
						}

						chasingPlayer = true;
					}
				}
			}
			else
			{
				if (chasingPlayer)
				{
					item.transform.Translate(new Vector3(lionMoveSpeed * Time.deltaTime, 0, 0), Space.World);
				}
				else
				{
					if (lastCharToEnter.transform.localPosition.x > item.transform.localPosition.x)
					{
						if (lionAnimations.ContainsKey(item) && !lionAnimations[item].IsPlaying(lionRunAnimation))
						{
							lionAnimations[item].Play(lionRunAnimation);
						}

						chasingPlayer = true;
					}
				}
			}
		}
	}
}
