using UnityEngine;
using System.Collections;

public abstract class IMenuStep : MonoBehaviour
{
	protected bool activated = false;
	
	public abstract void Activate();
	public abstract void Deactivate();
}

// Menu class template below
//public class NewMenuStep : IMenuStep 
//{
//	public void SetupLocal()
//	{
//	}
//	
//	public void SetupGlobal()
//	{
//	}
//	
//	protected void Awake()
//	{
//		SetupLocal();
//	}
//	
//	protected void Start () 
//	{
//		SetupGlobal();
//	}
//	
//	protected void Update () 
//	{
//	}
//	
//	public void Activate()
//	{
//		
//	}
//	
//	public void Deactivate()
//	{
//		
//	}
//}