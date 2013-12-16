using UnityEngine;
using System.Collections;

public class FroggerTimer : LugusSingletonRuntime<FroggerTimer> 
{
	protected float timer = 0;
	protected float maxTime = 0;
	protected bool running = false;
	protected bool finished = false;
	public delegate void TimerFinished();
	public TimerFinished timerFinished;

	public void StartTimer()
	{
		running = true;
	}

	public void StopTimer()
	{
		running = false;
	}

	public void ResetTimer()
	{
		timer = 0;
		finished = false;
	}

	private void Update()
	{
		if (!running)
			return;

		timer += Time.deltaTime;

		if (timer >= maxTime && !finished)
		{
			timerFinished();
			finished = true;
		}
	}

}
