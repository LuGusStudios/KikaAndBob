using UnityEngine;
using System.Collections;

public class GarbageCollectionForce : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void OnLevelWasLoaded(int index)
	{
		Debug.Log("Releasing resources ------------------------------------------------------");
		Resources.UnloadUnusedAssets();


		ClearSingletons();
	}

	public void ClearSingletons()
	{
		Debug.Log("Clearing singletons ------------------------------------------------------");

		LugusInput.Change(null);
		LugusResources.Change(null);
		LugusCoroutines.Change(null);
		LugusAudio.Change(null);
		LugusConfig.Change(null);


		// tried nulling game managers - no use

	


		Resources.UnloadUnusedAssets();

		System.GC.Collect();

	}

}
