using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FroggerLaneCanyon : FroggerLane 
{
	public ParticleSystem poof = null;

	public override void SetUpLocal()
	{
		if (poof == null)
		{
			base.SetUpLocal();
			poof = transform.FindChild("Poof").GetComponent<ParticleSystem>();
			if (poof == null)
			{
				Debug.LogError(name + ": Missing poof particle system.");
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
		float time = 0.3f;

		character.gameObject.MoveTo(character.transform.position.y(character.transform.position.y - 10f)).Time(time).Execute();

		yield return new WaitForSeconds(time);

		character.ShowCharacter(false);

		yield break;
	}
}
