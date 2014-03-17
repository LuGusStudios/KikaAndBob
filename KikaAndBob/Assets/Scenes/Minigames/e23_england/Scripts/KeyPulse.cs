using UnityEngine;
using System.Collections;

public class KeyPulse : MonoBehaviour
{
    protected float targetScale = 0.85f;
    protected float animationTime = 1.5f;
    protected void Awake()
    {
        SetUpLocal();
    }

    public virtual void SetUpLocal() 
    {
        gameObject.ScaleTo(Vector3.one * targetScale).Time(animationTime).EaseType(iTween.EaseType.easeInBack).Looptype(iTween.LoopType.pingPong).Execute();
    }

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
