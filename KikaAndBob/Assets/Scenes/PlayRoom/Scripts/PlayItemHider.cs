using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayItemHider : MonoBehaviour 
{
	public int index = 0;
	protected int offset = 3;	// there's fewer reward than mouse hunt levels - the first three levels don't give rewards

	public void SetupLocal()
	{
		// assign variables that have to do with this class only
	}
	
	public void SetupGlobal()
	{
		Evaluate();
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start() 
	{
		SetupGlobal();
	}
	
	protected void Update() 
	{
	
	}

	public void Evaluate()
	{
		if (LugusConfig.use.User.GetBool("e00_catchingmice_level_" + (index + offset), false))
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	protected void Hide()
	{
		//gameObject.SetActive(false);
		foreach(SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
		{
			sprite.color = sprite.color.a(0.3f);
		}

		foreach(Collider coll in GetComponentsInChildren<Collider>())
		{
			coll.enabled = false;
		}

		foreach(Collider2D coll in GetComponentsInChildren<Collider2D>())
		{
			coll.enabled = false;
		}
	}

	protected void Show()
	{
		//gameObject.SetActive(true);
		foreach(SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
		{
			sprite.color = sprite.color.a(1);
		}

		foreach(Collider coll in GetComponentsInChildren<Collider>())
		{
			coll.enabled = true;
		}
	}

}
