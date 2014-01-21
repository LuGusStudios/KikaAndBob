using UnityEngine;
using System.Collections;

public class FroggerLaneItemCrocodile : FroggerLaneItemLethal {

	public float openTime = 1f;
	protected Transform open = null;
	protected Transform closed = null;
	protected Transform originalParent = null;
	protected ILugusCoroutineHandle coroutine = null;

	private void Start()
	{
		open = transform.FindChild("Open");
		closed = transform.FindChild("Closed");

		open.gameObject.SetActive(false);
		closed.gameObject.SetActive(true);
	}

	protected override void EnterSurfaceEffect (FroggerCharacter character)
	{
		originalParent = character.transform.parent;
		character.transform.parent = this.transform;
		coroutine = LugusCoroutines.use.StartRoutine(OpenMouthRoutine());
	}

	protected override void LeaveSurfaceEffect(FroggerCharacter character)
	{
		coroutine.StopRoutine();
		character.transform.parent = originalParent;
	}

	protected IEnumerator OpenMouthRoutine()
	{

		yield return new WaitForSeconds(openTime);

		FroggerGameManager.use.LoseGame();
	}


}
