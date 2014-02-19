using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class FroggerBackgroundClouds : MonoBehaviour 
{
	public bool goRight = true;			// Whether the clouds go from left to right, or vice versa
	public float scrollingSpeed = 1f;	// Scrolling speed of the clouds
	public float spawnRate = 0.05f;		// Rate at which clouds are spawned per second

	public SpriteRenderer cloudPrefab = null;

	protected List<SpriteRenderer> activeClouds = new List<SpriteRenderer>();

	public void SetupLocal()
	{
		if (cloudPrefab == null)
		{
			cloudPrefab = transform.FindChild("Cloud").GetComponent<SpriteRenderer>();

			if (cloudPrefab == null)
			{
				Debug.LogError("Could not find a cloud prefab.");
			}
			else
			{
				cloudPrefab.gameObject.SetActive(false);
			}
		}
	}
	
	public void SetupGlobal()
	{
		StartCoroutine(SpawnCloudsRoutine());
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
		Vector3 translation = Vector3.right.normalized * scrollingSpeed * Time.deltaTime;

		if (!goRight)
		{
			translation = translation * (-1f);
		}

		Bounds backgroundBounds = this.GetComponent<SpriteRenderer>().bounds;

		for (int i = 0; i < activeClouds.Count;)
		{
			SpriteRenderer cloud = activeClouds[i];

			cloud.transform.Translate(translation, Space.World);

			// If the cloud is off screen, then delete it
			bool destroy = false;
			if (goRight)
			{
				// Check if the cloud is at the right end of the screen
				float xpos = backgroundBounds.center.x + backgroundBounds.extents.x + cloud.bounds.extents.x;
				if (cloud.transform.position.x > xpos)
				{
					destroy = true;
				}
			}
			else
			{
				// Check if the cloud is at the left end of the screen
				float xpos = backgroundBounds.center.x - backgroundBounds.extents.x - cloud.bounds.extents.x;
				if (cloud.transform.position.x < xpos)
				{
					destroy = true;
				}
			}

			if (destroy)
			{
				activeClouds.RemoveAt(i);
				GameObject.Destroy(cloud.gameObject);
			}
			else
			{
				++i;
			}
		}
	}

	protected IEnumerator SpawnCloudsRoutine()
	{
		while (true)
		{
			if ((Random.value < spawnRate) && (cloudPrefab != null))
			{
				SpriteRenderer cloud = (SpriteRenderer)GameObject.Instantiate(cloudPrefab);
				cloud.gameObject.SetActive(true);
				cloud.transform.parent = this.transform;
				cloud.transform.position = new Vector3(0f, cloudPrefab.transform.position.y, cloudPrefab.transform.position.z);
				activeClouds.Add(cloud);

				Bounds backgroundBounds = this.GetComponent<SpriteRenderer>().bounds;

				float xpos = 0f;
				if (goRight)
				{
					// Place the cloud at the left of the screen
					xpos = backgroundBounds.center.x - backgroundBounds.extents.x - cloud.bounds.extents.x;
				}
				else
				{
					// Place the cloud at the right of the screen
					xpos = backgroundBounds.center.x + backgroundBounds.extents.x + cloud.bounds.extents.x;
				}

				cloud.transform.Translate(new Vector3(xpos, 0f, 0f), Space.World);
			}

			yield return new WaitForSeconds(1f);
		}
	}
}
