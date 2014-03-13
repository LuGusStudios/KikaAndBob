using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DartsRussiaTrainSound : MonoBehaviour 
{
	public void SetupGlobal()
	{
		LugusAudio.use.Music().Play(LugusResources.use.Shared.GetAudio("TrainRide01"), false, new LugusAudioTrackSettings().Loop(true).Volume(1.0f));
	}

	protected void Start () 
	{
		SetupGlobal();
	}
}
