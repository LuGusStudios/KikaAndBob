using UnityEngine;
using System.Collections;

public abstract class IConsumableUser : MonoBehaviour
{
	public abstract bool Use();

	// TODO:
	public Vector3 GetTarget()
	{
		return this.transform.position;
	}
}
