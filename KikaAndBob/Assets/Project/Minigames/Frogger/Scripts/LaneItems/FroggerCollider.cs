using UnityEngine;
using System.Collections;

public class FroggerCollider : FroggerLaneItem {

	public enum FroggerColliderType
	{
		None,
		All,
		Top,
		Bottom
	}

	public FroggerColliderType colliderType = FroggerColliderType.All;
}
