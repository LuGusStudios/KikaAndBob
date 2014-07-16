using UnityEngine;
using System.Collections;

public class Pair<T, U> {

	public T First
	{
		get
		{
			return first;
		}
		set
		{
			first = value;
		}
	}
	public U Second
	{
		get
		{
			return second;
		}
		set
		{
			second = value;
		}
	}

	protected T first;
	protected U second;

	public Pair(T f, U s)
	{
		first = f;
		second = s;
	}
}
