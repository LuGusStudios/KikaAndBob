using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SmoothMoves;

public class SmoothMovesUtil 
{
	// handy to reset color if the coroutine below was terminated before finishing
	public static void SetColor(BoneAnimation[] animationContainers, Color color)
	{
		foreach( BoneAnimation container in animationContainers )
			container.SetMeshColor( color );
	}

	public static IEnumerator Blink(BoneAnimation[] animationContainers, Color color, float duration, int blinkCount)
	{
		Color originalColor = Color.white;
		
		// we want the running animation to start playing before the invulnerability (red blinking) is done
		//LugusCoroutines.use.StartRoutine( HitAnimationRoutine (0.3f) );

		float partDuration = duration / (float) blinkCount;
		
		for( int i = 0; i < blinkCount; ++i )
		{
			float percentage = 0.0f;
			float startTime = Time.time;
			bool rising = true;
			Color newColor = new Color();
			
			while( rising )
			{
				percentage = (Time.time - startTime) / (partDuration / 2.0f);
				newColor = originalColor.Lerp (color, percentage);
				
				foreach( BoneAnimation container in animationContainers )
					container.SetMeshColor( newColor );
				
				if( percentage >= 1.0f )
					rising = false;
				
				yield return null;
			}
			
			percentage = 0.0f;
			startTime = Time.time;
			
			while( !rising )
			{
				percentage = (Time.time - startTime) / (partDuration / 2.0f);
				newColor = color.Lerp (originalColor,percentage);
				
				foreach( BoneAnimation container in animationContainers )
					container.SetMeshColor( newColor );
				
				if( percentage >= 1.0f )
					rising = true;
				
				yield return null;
			}
		}
		
		foreach( BoneAnimation container in animationContainers )
			container.SetMeshColor( originalColor );
	}
}
