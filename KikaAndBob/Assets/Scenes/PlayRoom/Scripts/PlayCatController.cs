using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayCatController : MonoBehaviour {

	public float originalScale = 2;
	public float speed = 12.0f;

	protected IPlayItem targetPlayItem = null;
	protected ILugusCoroutineHandle moveRoutineHandle = null;
	public PlayCatAnimation catAnimation = null;
	
	public void SetupLocal()
	{
		if (catAnimation == null)
		{
			catAnimation = GetComponent<PlayCatAnimation>();
		}

		if (catAnimation == null)
		{
			Debug.LogError("PlayCatController: Missing cat animation!");
		}
	}
	
	public void SetupGlobal()
	{

	}
	
	protected void Awake()
	{
		SetupLocal();
	}
	
	protected void Start() 
	{
		SetupGlobal();
	}

	protected void Update () 
	{
		if (LugusInput.use.down)
		{
			Transform hit = LugusInput.use.RayCastFromMouseDown();

			if (hit != null)
			{
				IPlayItem playItem = (IPlayItem) hit.GetComponentInChildren( typeof(IPlayItem) );
			
				if (playItem != null && playItem != targetPlayItem)
				{
					if (targetPlayItem != null)
						targetPlayItem.Deactivate(this);

					targetPlayItem = playItem;

					transform.position = playItem.GetActionPoint().position;
					transform.localScale = Vector3.one * originalScale * targetPlayItem.scale;
					targetPlayItem.Activate(this);

					//MoveToTarget(playItem.GetActionPoint().position);
				}
			}
		}
	}

//	protected void MoveToTarget(Vector3 newPosition)
//	{
//		if (moveRoutineHandle != null && moveRoutineHandle.Running)
//		{
//			moveRoutineHandle.StopRoutine();
//		}
//
//		moveRoutineHandle = LugusCoroutines.use.StartRoutine(MoveToTargetRoutine(newPosition));
//	}
//
//	protected Vector3[] CreatePathTo(Vector3 newPosition)
//	{
//		Vector3 direction = (newPosition - transform.position).normalized;
//		float distance = Vector3.Distance(newPosition, transform.position);
//
//		RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1, direction, distance);
//		List<IPlayItem> avoidItems = new List<IPlayItem>();
//		List<RaycastHit> avoidHits = new List<RaycastHit>();
//		List<Vector3> path = new List<Vector3>();
//
////		foreach(RaycastHit hit in hits)
////		{
////			if (hit.transform == targetPlayItem.transform)
////				continue;
////
////			IPlayItem playItem = hit.transform.GetComponentInChildren<IPlayItem>();
////
////			if (playItem != null && playItem.avoidWhenWalking)
////			{
////				avoidItems.Add(playItem);
////				avoidHits.Add(hit);
////			}
////		}
////
////		
////	//	Debug.DrawRay(transform.position, direction, Color.white, 1f,);
////
////		List<Vector3> path = new List<Vector3>();
////
////		path.Add(transform.position);
////
////		foreach(RaycastHit hit in avoidHits)
////		{
////			Vector3 pointOnline = transform.position + direction * (Vector3.Distance(transform.position, hit.collider.transform.position));
////
////			Vector3 directionBetweenObstacleAndLinePoint = (hit.collider.transform.position - pointOnline).normalized;
////
////			path.Add(pointOnline + (directionBetweenObstacleAndLinePoint * Mathf.Max( hit.collider.bounds.extents.x, hit.collider.bounds.extents.y)));
////
////		}
//
//
//		foreach(RaycastHit hit in hits)
//		{
//			if (hit.transform == targetPlayItem.transform)
//				continue;
//
//			IPlayItem playItem = hit.transform.GetComponentInChildren<IPlayItem>();
//
//			if (playItem != null && playItem.avoidPoint != null)
//			{
//				avoidItems.Add(playItem);
//				avoidHits.Add(hit);
//			}
//		}
//
//		foreach(Vector3 point in path)
//		{
//			GameObject pathVis = GameObject.CreatePrimitive(PrimitiveType.Cube);
//			pathVis.transform.position = point;
//			pathVis.transform.localScale = pathVis.transform.localScale * 0.2f;
//		}
//
//
//		path.Add(newPosition);
//
//		print( path.Count);
//
//		return path.ToArray();			  
//	}



//	protected IEnumerator MoveToTargetRoutine(Vector3 newPosition)
//	{
//		iTween.Stop(gameObject);
//
//		Vector3[] path = CreatePathTo(newPosition);
//
//		gameObject.MoveTo(path).Speed(speed).MoveToPath(false).Execute();
//
//
//		float moveTime = Vector3.Distance(newPosition, transform.position) / speed;
//
//		catAnimation.PlayAnimation("LEFT/@Side_Walk");
//
//		yield return new WaitForSeconds(moveTime);
//
//		while (Vector3.Distance(transform.position.v2(), newPosition.v2()) > 0.001f)
//		{
//			yield return null;
//		}
//
//		if (targetPlayItem == null)
//		{
//			Debug.LogError("PlayCatController: Target item was null upon arrival!");
//		}
//		else
//		{
//			targetPlayItem.Activate();
//			catAnimation.PlayAnimation(targetPlayItem.catAnimationPath);
//		}
//	}


}
