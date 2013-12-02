using UnityEngine;
using System.Collections;

public abstract class PacmanLevelUpdater : MonoBehaviour {
	
	protected ILugusCoroutineHandle loopHandle = null;
	public abstract void Activate();
	public abstract void Deactivate();

}
