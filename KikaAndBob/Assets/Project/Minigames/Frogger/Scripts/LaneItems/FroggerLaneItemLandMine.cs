using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLaneItemLandMine : FroggerLaneItemDestructible 
{
	public float detonationTimer = 1f;
	public DataRange blinkingSpeed = new DataRange(0.1f, 0.5f);
	public bool destroyByOther = true;

	public SpriteRenderer mineLightOff = null;
	public SpriteRenderer mineLightOn = null;
	public FroggerExplosion explosion = null;

	private enum MineState
	{
		NONE = -1,
		ACTIVE = 1,
		EXPLODING = 2
	}

	private MineState mineState = MineState.NONE;
	private float nextBlinkTime = 1f;
	private FroggerCharacter player = null;

	public void SetupLocal()
	{
		base.SetUpLocal();
		mineState = MineState.ACTIVE;
		nextBlinkTime = blinkingSpeed.to;
	}
	
	public void SetupGlobal()
	{
		if (mineLightOff == null)
		{
			mineLightOff = transform.FindChild("MineLightOff").GetComponent<SpriteRenderer>();
			if (mineLightOff == null)
			{
				Debug.LogError("Could not find the sprite renderer for the mine with light off!");
			}
			else
			{
				mineLightOff.enabled = true;
			}
		}

		if (mineLightOn == null)
		{
			mineLightOn = transform.FindChild("MineLightOn").GetComponent<SpriteRenderer>();
			if (mineLightOn == null)
			{
				Debug.LogError("Could not find the sprite renderer for the mine with light off!");
			}
			else
			{
				mineLightOn.enabled = false;
			}
		}

		if (explosion == null)
		{
			explosion = transform.FindChild("Explosion").GetComponent<FroggerExplosion>();
			if (explosion == null)
			{
				Debug.LogError("Could not find the explosion!");
			}
		}

		GetComponent<BoxCollider2D>().isTrigger = true;

		// Arm mine
		StartCoroutine(ActiveMineRoutine());
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
		// Update current blink time
		nextBlinkTime -= Time.deltaTime;

		// Check whether the mine should start blinking faster when it is triggered
		float time = float.PositiveInfinity;
		if (mineState == MineState.ACTIVE)
		{
			time = blinkingSpeed.to;
		}
		else if (mineState == MineState.EXPLODING)
		{
			time = blinkingSpeed.from;
		}

		if (time < nextBlinkTime)
		{
			nextBlinkTime = time;
		}

		// Let the mine blink its light
		if (nextBlinkTime < 0f)
		{
			Blink();
			nextBlinkTime = time;
		}
	}

	private void Blink()
	{
		mineLightOff.enabled = !mineLightOff.enabled;
		mineLightOn.enabled = !mineLightOn.enabled;
	}

	protected override void EnterSurfaceEffect(FroggerCharacter character)
	{
		mineState = MineState.EXPLODING;
		player = character;
	}

	protected override void LeaveSurfaceEffect(FroggerCharacter character)
	{
		player = null;
	}

	public override void Destruct()
	{
		if (destroyByOther && (mineState == MineState.ACTIVE))
		{
			mineState = MineState.EXPLODING;
			detonationTimer = 0f;
		}
	}

	private IEnumerator ActiveMineRoutine()
	{
		while (true)
		{
			if (mineState == MineState.EXPLODING)
			{
				yield return new WaitForSeconds(detonationTimer);
				StartCoroutine(ExplosionRoutine());
				yield break;
			}

			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator ExplosionRoutine()
	{
		mineLightOff.enabled = false;
		mineLightOn.enabled = false;

		explosion.Explode();

		while (true)
		{
			if (explosion.State == FroggerExplosion.ExplosionState.STOPPED)
			{
				GameObject.Destroy(gameObject);
				yield break;
			}

			yield return new WaitForEndOfFrame();
		}
	}
}
