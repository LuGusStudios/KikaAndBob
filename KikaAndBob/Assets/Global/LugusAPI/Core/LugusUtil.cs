using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LugusUtil 
{
	public static float UIWidth
	{
		get{ return 20.48f; } // TODO: change this per project depending on the scene UI setup (if not using NGUI or unity gui)
	}
	
	public static float UIHeight
	{
		get{ return 15.36f; } // TODO: change this per project depending on the scene UI setup (if not using NGUI or unity gui)
	}

	// Rect here is based at BOTTOM LEFT for 0,0 (instead of default TOP LEFT as in the unity3D docs)
	public static Rect UIScreenSize
	{
		get{ return new Rect(0, 0, LugusUtil.UIWidth, LugusUtil.UIHeight ); }
	}
	
	// Rect here is based at BOTTOM LEFT for 0,0 (instead of default TOP LEFT as in the unity3D docs)
	public static Rect UIScreenSizePixelPerfect
	{
		get{ return new Rect(0, 0, LugusUtil.UIWidth * 100, LugusUtil.UIHeight * 100 ); }
	}

	public static int ScreenWidth
	{
		get{ return Screen.width; }
	}
	
	public static int ScreenHeight
	{
		get{ return Screen.height; }
	}
	
	public static bool PointOnScreen(Vector3 screenPoint)
	{
		bool x = (screenPoint.x >= 0) && (screenPoint.x <= ScreenWidth);
		bool y = (screenPoint.y >= 0) && (screenPoint.y <= ScreenHeight);
		
		return x && y;
	}
	
	public static bool PointOnUI(Vector3 screenPoint)
	{
		bool x = (screenPoint.x >= 0) && (screenPoint.x <= UIWidth);
		bool y = (screenPoint.y >= 0) && (screenPoint.y <= UIHeight);
		
		return x && y;
	}
	
	public static Vector3 BottomOffscreen(Vector3 reference)
	{
		return (ScreenCenter(reference) - new Vector3(0, Screen.height, 0));
	}
	
	public static Vector3 TopOffscreen(Vector3 reference)
	{
		return (ScreenCenter(reference) + new Vector3(0, Screen.height, 0));
	}
	
	public static Vector3 ScreenCenter(Vector3 reference)
	{
		return new Vector3(0,0, reference.z);//new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, reference.z);
	}
	
	public static Vector3 DEFAULTVECTOR = new Vector3( float.MaxValue, float.MaxValue, float.MaxValue ); 
	public static Vector3 OFFSCREEN = new Vector3( -9999.0f, -9999.0f, -9999.0f ); 

}

public static class BoundsExtensions
{
	public static Rect ToRectXY(this Bounds bounds)
	{
		return new Rect( bounds.center.x , bounds.center.y, bounds.size.x, bounds.size.y);
	}

	public static Bounds Bounds(this BoxCollider2D collider)
	{
		return new UnityEngine.Bounds( collider.center, collider.size );
	}
}

public class MonoBehaviourExtensionsHelper
{
	
	public IEnumerator DisableRoutine(MonoBehaviour c, float delay)
	{
		yield return new WaitForSeconds(delay);
		
		if( c != null )
			c.enabled = false;
	}
}

public static class MonoBehaviourExtensions
{
	public static void Disable(this MonoBehaviour component, float delay = 0.0f )
	{
		MonoBehaviourExtensionsHelper helper = new MonoBehaviourExtensionsHelper();
		LugusCoroutines.use.StartRoutine( helper.DisableRoutine(component, delay) );
	}

}

public static class VectorExtensions
{
	// ex. vec = vec.y( value );
	public static Vector3 y(this Vector3 v, float val)
	{
		return new Vector3(v.x, val, v.z);
	}
	
	public static Vector3 x(this Vector3 v, float val)
	{
		return new Vector3(val, v.y, v.z);
	}
	
	public static Vector3 z(this Vector3 v, float val)
	{
		return new Vector3(v.x, v.y, val);
	}


	public static Vector2 x(this Vector2 v, float val)
	{
		return new Vector2(val, v.y);
	}
	
	public static Vector2 y(this Vector2 v, float val)
	{
		return new Vector2(v.x, val);
	}


	public static Vector2 v2(this Vector3 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static Vector3 v3(this Vector2 v)
	{
		return new Vector3(v.x, v.y, 0);
	}
	
	// ex. vec = vec.yAdd( -5.0f )
	// looks better than vec = vec.y( vec.y - 5.0f )
	public static Vector3 yAdd(this Vector3 v, float val)
	{
		return new Vector3(v.x, v.y + val, v.z);
	}
	
	public static Vector3 xAdd(this Vector3 v, float val)
	{
		return new Vector3(v.x + val, v.y, v.z);
	}
	
	public static Vector3 zAdd(this Vector3 v, float val)
	{
		return new Vector3(v.x, v.y, v.z + val);
	}
	
	// ex. vec = vec.yMul( -5.0f )
	// looks better than vec = vec.y( vec.y * -5.0f )
	public static Vector3 yMul(this Vector3 v, float val)
	{
		return new Vector3(v.x, v.y * val, v.z);
	}
	
	public static Vector3 xMul(this Vector3 v, float val)
	{
		return new Vector3(v.x * val, v.y, v.z);
	}
	
	public static Vector3 zMul(this Vector3 v, float val)
	{
		return new Vector3(v.x, v.y, v.z * val);
	}
}

// ADDED BY KASPER
public static class ColorExtensions
{
	public static Color r(this Color c, float val)
	{
		return new Color(val, c.g, c.b, c.a);
	}
	
	public static Color g(this Color c, float val)
	{
		return new Color(c.r, val, c.b, c.a);
	}
	
	public static Color b(this Color c, float val)
	{
		return new Color(c.r, c.g, val, c.a);
	}
	
	public static Color a(this Color c, float val)
	{
		return new Color(c.r, c.g, c.b, val);
	}

	public static Color Lerp(this Color c, Color target, float percentage )
	{
		Color output = new Color();
		output.r = Mathf.Lerp(c.r, target.r, percentage);
		output.g = Mathf.Lerp(c.g, target.g, percentage);
		output.b = Mathf.Lerp(c.b, target.b, percentage);
		output.a = Mathf.Lerp(c.a, target.a, percentage); 

		return output;
	}
}

public static class TransformExtensions
{
	public static List<Transform> FindChildrenRecursively(this Transform root, string name)
	{
		List<Transform> output = new List<Transform>();
		
		Component[] transforms = root.GetComponentsInChildren( typeof( Transform ), true );
		
		foreach( Transform t in transforms )
		{
			if( t.name == name )
				output.Add( t );
		}
		
		return output;
	}
	
	public static Transform FindChildRecursively(this Transform root, string name)
	{
		Component[] transforms = root.GetComponentsInChildren( typeof( Transform ), true );
		
		foreach( Transform t in transforms )
		{
			if( t.name == name )
				return t;
		}

		return null;
	}
	
	public static string Path(this Transform root)
	{
	    string path = "/" + root.name;
	    while (root.transform.parent != null)
	    {
	        root = root.transform.parent;
	        path = "/" + root.name + path;
	    }
	    return path;
	}
}

public static class AnimatorExtensions
{
	public static void FireBool(this Animator animator, MonoBehaviour localObject, string boolName)
	{
		localObject.StartCoroutine( FireBoolRoutine(animator, boolName) );
	}
	
	public static IEnumerator FireBoolRoutine(Animator animator, string boolName)
	{	
		//Debug.Log ("FireBool " + boolName + " -> " + animator.GetAnimatorTransitionInfo(0).nameHash );
		animator.SetBool( boolName, true );
		
		yield return null; // wait for 1 frame
		
		//yield return new WaitForSeconds(0.2f);
		
		animator.SetBool( boolName, false );
		//Debug.Log ("FireBool done " + boolName + " -> " + animator.GetAnimatorTransitionInfo(0).nameHash );
	}
	
	// set bool to a value until the next transition occurs
	public static void HoldBool(this Animator animator, MonoBehaviour localObject, string boolName)
	{
		localObject.StartCoroutine( HoldBoolRoutine(animator, boolName) );
	}
	
	public static IEnumerator HoldBoolRoutine(this Animator animator, string boolName)
	{	
		//Debug.Log ("HoldBool " + boolName + " -> " + animator.GetAnimatorTransitionInfo(0).nameHash + " / " + animator.IsInTransition(0) );
		animator.SetBool( boolName, true );
		
		//yield return null; // wait for 1 frame
		
		while( !animator.IsInTransition(0) )
			yield return null;
		
		// right after the transition has started
		
		animator.SetBool( boolName, false );
		
		while( animator.IsInTransition(0) )
			yield return null;
		
		//Debug.Log ("HoldBool done " + boolName + " -> " + animator.GetAnimatorTransitionInfo(0).nameHash+ " / " + animator.IsInTransition(0) );
	}
	
	// make sure the boolean is set as soon as possible and that the transition happens
	public static void ForceTransition(this Animator animator, MonoBehaviour localObject, string boolName)
	{
		localObject.StartCoroutine( ForceTransitionRoutine(animator, boolName) );
	}
	
	public static IEnumerator ForceTransitionRoutine(this Animator animator, string boolName)
	{
		// if currently in transition: wait untill arrived in state
		while( animator.IsInTransition(0) )
			yield return null;
		
		//Debug.Log ("ForceTransitionRoutine " + boolName + " -> " + animator.GetAnimatorTransitionInfo(0).nameHash + " / " + animator.IsInTransition(0) );
		
		animator.SetBool( boolName, true );
		
		//yield return null; // wait for 1 frame
		
		while( !animator.IsInTransition(0) )
			yield return null;

		// right after the transition has started
		
		animator.SetBool( boolName, false );
		
		//Debug.Log ("ForceTransitionRoutine DONE " + boolName + " -> " + animator.GetAnimatorTransitionInfo(0).nameHash + " / " + animator.IsInTransition(0) );
		
	}
	
}

public static class ListExtensions
{
	public static void Shuffle<T>(this IList<T> list)  
	{  
		System.Random rng = new System.Random();  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}
}
