using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PacmanBackgroundTiling : MonoBehaviour 
{
	public int ringCount = 1;

	protected PacmanPlayerCharacter player = null;
	protected SpriteRenderer backgroundSprite = null;

	public void SetupLocal()
	{
		PacmanLevelManager.use.onLevelBuilt += OnLevelBuilt;
	}
	
	public void SetupGlobal()
	{
	
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	protected void OnLevelBuilt()
	{
		Debug.Log("PacmanBackgroundTiling: reset");
		player = (PacmanPlayerCharacter) FindObjectOfType(typeof(PacmanPlayerCharacter));
		backgroundSprite = GetComponent<SpriteRenderer>();
		CreateTilingBackground(); // handy to call this again in case we ever do something like per-level backgrounds
	}

	protected void CreateTilingBackground()
	{
		// clear background clones
		// TO DO: avoid this if not necessary? only do once?
		for (int i = transform.childCount - 1; i >= 0; i--) 
		{
			#if UNITY_EDITOR
			DestroyImmediate(transform.GetChild(i).gameObject);
			#else
			Destroy(transform.GetChild(i).gameObject);
			#endif
		}

		Vector2 dimensions = backgroundSprite.bounds.size.v2();

		for (int x = -ringCount; x <= ringCount; x++) 
		{
			for (int y = -ringCount; y <= ringCount; y++) 
			{
				if (x == 0 && y == 0)
					continue;

				GameObject backgroundClone = new GameObject(gameObject.name);
				SpriteRenderer cloneSpriteRenderer = backgroundClone.AddComponent<SpriteRenderer>();
				cloneSpriteRenderer.sprite = backgroundSprite.sprite;

				backgroundClone.transform.parent = transform;

				backgroundClone.transform.localPosition = Vector3.zero;
				backgroundClone.transform.Translate(new Vector3(x * dimensions.x, y * dimensions.y, 0));
			}
		}
	}

	protected void Update () 
	{
		UpdateBackground();
	}

	protected void UpdateBackground()
	{
		if (player == null)
			return;

		if ((player.transform.position.x - transform.position.x) >= backgroundSprite.bounds.extents.x)
		{
			transform.Translate(Vector3.zero.x(backgroundSprite.bounds.size.x));
		}
		else if ((transform.position.x - player.transform.position.x) >= backgroundSprite.bounds.extents.x)
		{
			transform.Translate(Vector3.zero.x(- 1 * backgroundSprite.bounds.size.x));
		}

		if ((player.transform.position.y - transform.position.y) >= backgroundSprite.bounds.extents.y)
		{
			transform.Translate(Vector3.zero.y(backgroundSprite.bounds.size.y));
		}
		else if ((transform.position.y - player.transform.position.y) >= backgroundSprite.bounds.extents.y)
		{
			transform.Translate(Vector3.zero.y(- 1 * backgroundSprite.bounds.size.y));
		}
	}


}
