using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class FroggerLaneItemIceBreakingSubmarine : FroggerLaneItemLethal
{
	public FroggerLaneItemIceHole iceHolePrefab = null;
	public SpriteRenderer periscope = null;
	public SpriteRenderer iceHole = null;
	public SpriteRenderer submarine = null;
	public Animator splash = null;

	public string icebreakingSFXName = "";


	public enum State
	{
		NONE = -1,
		UNDER = 1,
		PERISCOPE = 2,
		SUBMARINE = 3,
		DONE = 4
	}

	public State state = State.UNDER;

	public void Surface()
	{
		StartCoroutine(SubmarineAnimationRoutine());
	}

	public override void SetUpLocal()
	{
		base.SetUpLocal();

		state = State.UNDER;
	}
	
	public override void SetupGlobal()
	{
		base.SetupGlobal();

		if (periscope == null)
		{
			periscope = transform.FindChild("Periscope").GetComponent<SpriteRenderer>();
			if (periscope == null)
			{
				Debug.LogError("Could not find the sprite renderer for the periscope!");
			}
			else
			{
				periscope.gameObject.SetActive(false);
			}
		}

		if (iceHole == null)
		{
			iceHole = transform.FindChild("IceHole").GetComponent<SpriteRenderer>();
			if (iceHole == null)
			{
				Debug.LogError("Could not find the sprite renderer for the ice hole!");
			}
			else
			{
				iceHole.gameObject.SetActive(false);
			}
		}

		if (submarine == null)
		{
			submarine = transform.FindChild("Submarine").GetComponent<SpriteRenderer>();
			if (submarine == null)
			{
				Debug.LogError("Could not find the sprite renderer for the submarine!");
			}
			else
			{
				submarine.gameObject.SetActive(false);
			}
		}

		if (splash == null)
		{
			splash = transform.FindChild("Splash").GetComponent<Animator>();
			if (splash == null)
			{
				Debug.LogError("Could not find the splash animation!");
			}
			else
			{
				splash.gameObject.SetActive(false);
			}
		}

		GetComponent<BoxCollider2D>().enabled = false;
	}

	protected void Awake()
	{
		SetUpLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	private IEnumerator SubmarineAnimationRoutine()
	{
		state = State.PERISCOPE;

		iceHole.gameObject.SetActive(true);

		// Animate the periscope
		periscope.gameObject.SetActive(true);
		iTweener periscopeAnim = iTweenExtensions.MoveTo(periscope.gameObject, periscope.transform.position + new Vector3(0f, 1.5f, 0f)).Time(0.5f);
		periscopeAnim.Execute();

		yield return new WaitForSeconds(0.5f);
		periscope.transform.Rotate(new Vector3(0f, 1, 0f), 180f);
		yield return new WaitForSeconds(0.5f);
		periscope.transform.Rotate(new Vector3(0f, 1f, 0f), -180f);
		yield return new WaitForSeconds(0.5f);

		periscopeAnim = iTweenExtensions.MoveTo(periscope.gameObject, periscope.transform.position + new Vector3(0f, -1.5f, 0f)).Time(0.5f);
		periscopeAnim.Execute();

		yield return new WaitForSeconds(0.5f);

		periscope.gameObject.SetActive(false);

		// Animate the the ice hole's scale and let the submarine appear above the ice
		submarine.gameObject.SetActive(true);
		iTweener iceHoleAnim = iTweenExtensions.ScaleTo(iceHole.gameObject, iceHolePrefab.transform.localScale).Time(0.2f);
		iTweener submarineAnim = iTweenExtensions.MoveTo(submarine.gameObject, submarine.transform.position + new Vector3(0f, 3f, 0f)).Time(0.4f);
		iceHoleAnim.Execute();
		submarineAnim.Execute();

		StartCoroutine(SplashAnimationRoutine());

		yield return new WaitForSeconds(0.2f);

		// Set the this state, so that the lane knows the ice hole has been scaled to the proper size
		state = State.SUBMARINE;
		yield return new WaitForSeconds(0.8f);

		// Let the submarine disappear again
		submarineAnim = iTweenExtensions.MoveTo(submarine.gameObject, submarine.transform.position + new Vector3(0f, -3f, 0f)).Time(0.7f);
		submarineAnim.Execute();

		yield return new WaitForSeconds(1f);

		state = State.DONE;

		yield return new WaitForEndOfFrame();
	}

	private IEnumerator SplashAnimationRoutine()
	{
		if (splash == null)
		{
			yield break;
		}

		yield return new WaitForSeconds(0.1f);

		GameObject splashCopy = (GameObject)Instantiate(splash.gameObject);
		splashCopy.transform.position = this.transform.position;
		splashCopy.SetActive(true);

		yield return new WaitForSeconds(0.8f);

		if (splashCopy != null)	// handy to check in case level was rebuilt
		{
			GameObject.Destroy(splashCopy);
		}
	}
}
