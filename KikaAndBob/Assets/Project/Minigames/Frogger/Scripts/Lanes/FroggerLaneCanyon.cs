using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLaneCanyon : FroggerLane 
{
	public ParticleSystem poof = null;
	public float cloudSpeed = 1f;
	public List<FroggerLaneItem> clouds = new List<FroggerLaneItem>();
	public string poofSFXName = "";

	protected List<FroggerLaneItem> spawnedClouds = new List<FroggerLaneItem>();
	protected float cloudSpawnDistance = 0f;
	protected float nextCloudInterval = 0f;

	public override void SetUpLocal()
	{
		base.SetUpLocal();
		
		if (poof == null)
		{
			poof = transform.FindChild("Poof").GetComponent<ParticleSystem>();
			if (poof == null)
			{
				Debug.LogError(name + ": Missing poof particle system.");
			}
		}
	}

	public override void SetUpLane()
	{
		base.SetUpLane();

		FillCloudItems();
	}

	protected override void Update()
	{
		base.Update();

		if (clouds.Count < 1 || cloudSpeed <= 0)
		{
			return;
		}

		if (cloudSpawnDistance >= nextCloudInterval)
		{
			FroggerLaneItem cloud = SpawnCloud();
			cloudSpawnDistance = 0f;
			nextCloudInterval = Random.Range(minGapDistance, maxGapDistance) + cloud.GetSurfaceSize().x;
		}

		float displacement = cloudSpeed * Time.deltaTime;
		cloudSpawnDistance += displacement;

		for (int i = spawnedClouds.Count - 1; i >= 0; i--)
		{
			FroggerLaneItem currentCloud = spawnedClouds[i];
			currentCloud.UpdateLaneItem(displacement);

			if (currentCloud.CrossedLevel())
			{
				spawnedClouds.Remove(currentCloud);
				Destroy(currentCloud.gameObject);
			}
		}
	}

	protected override void EnterSurfaceEffect(FroggerCharacter character)
	{
		StartCoroutine(VanishCharacterRoutine(character));

		poof.transform.position = poof.transform.position.x(character.transform.position.x);
		poof.Play();

		FroggerGameManager.use.LoseGame();
	}

	private IEnumerator VanishCharacterRoutine(FroggerCharacter character)
	{
		if (!string.IsNullOrEmpty(poofSFXName))
		{
			AudioClip poofSFX = LugusResources.use.Shared.GetAudio(poofSFXName);
			if (poofSFX != LugusResources.use.errorAudio)
			{
				LugusAudio.use.SFX().Play(poofSFX);
			}
		}

		float time = 0.3f;

		character.gameObject.MoveTo(character.transform.position.y(character.transform.position.y - 10f)).Time(time).Execute();

		yield return new WaitForSeconds(time);

		character.ShowCharacter(false);

		yield break;
	}

	protected void FillCloudItems()
	{
		if (clouds.Count < 1)
		{
			return;
		}

		float laneCompletion = 0;
		float lastItemWidth = 0;

		while (laneCompletion < laneSize.x)
		{
			FroggerLaneItem cloud = SpawnCloud();

			if (goRight)
			{
				cloud.transform.Translate(new Vector3(laneCompletion, 0, 0), Space.World);
			}
			else
			{
				cloud.transform.Translate(new Vector3(-laneCompletion, 0, 0), Space.World);
			}

			lastItemWidth = cloud.GetSurfaceSize().x;
			lastItemWidth += Random.Range(minGapDistance, maxGapDistance);

			laneCompletion += lastItemWidth;
		}

		nextCloudInterval = lastItemWidth;
	}

	protected FroggerLaneItem SpawnCloud()
	{
		int index = Random.Range(0, clouds.Count);

		GameObject cloudObj = (GameObject)Instantiate(clouds[index].gameObject);

		FroggerLaneItem cloud = cloudObj.GetComponent<FroggerLaneItem>();

		cloud.goRight = goRight;
		cloud.SetLaneDistance(GetSurfaceSize().x + cloud.GetSurfaceSize().x * 2f);

		BoxCollider2D cloudCollider = cloud.GetComponent<BoxCollider2D>();
		cloudCollider.size = cloudCollider.size.y(height / cloudCollider.transform.localScale.y);
		cloudCollider.center = cloudCollider.center.y(surfaceCollider.center.y);

		cloud.transform.parent = this.transform;
		cloud.transform.localPosition = Vector3.zero;
		cloud.transform.localRotation = Quaternion.identity;

		if (goRight)
		{
			cloud.transform.localPosition = new Vector3(-((laneSize.x * 0.5f) + cloud.GetSurfaceSize().x * 0.5f), 0f, -0.1f);
		}
		else
		{
			cloud.transform.localPosition = new Vector3(((laneSize.x * 0.5f) + cloud.GetSurfaceSize().x * 0.5f), 0f, -0.1f);
		}

		spawnedClouds.Add(cloud);

		return cloud;
	}
}
