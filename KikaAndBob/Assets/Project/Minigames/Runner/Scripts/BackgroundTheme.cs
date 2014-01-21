using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundTheme : ScriptableObject 
{
	public Sprite[] sky = null;
	public Sprite[] ground = null;

	public Sprite[] skyDetails = null;
	public float skyDetailsIntensity = 1.0f;

	public Sprite[] groundDetails;
	public float groundDetailsIntensity = 1.0f;

	public Sprite[] frontDetails;
	public float frontDetailsIntensity = 1.0f;
}
