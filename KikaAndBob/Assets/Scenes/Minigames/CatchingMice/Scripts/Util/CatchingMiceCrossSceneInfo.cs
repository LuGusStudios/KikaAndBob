using UnityEngine;
using System.Collections;

public class CatchingMiceCrossSceneInfo : MonoBehaviour {

	public int LevelToLoad
	{
		get
		{
			return levelToLoad;
		}

		set
		{
			levelToLoad = value;
		}
	}

	protected int levelToLoad = -1;

	public static CatchingMiceCrossSceneInfo use
	{
		get
		{
			CatchingMiceCrossSceneInfo info = (CatchingMiceCrossSceneInfo)GameObject.FindObjectOfType(typeof(CatchingMiceCrossSceneInfo));
			if (info == null)
			{
				GameObject container = new GameObject("CatchingMiceCrossSceneInfo");
				info = container.AddComponent<CatchingMiceCrossSceneInfo>();

				DontDestroyOnLoad(container);
			}

			return info;
		}
	}

	public void Destroy()
	{
		GameObject.Destroy(this.gameObject);
	}
}
