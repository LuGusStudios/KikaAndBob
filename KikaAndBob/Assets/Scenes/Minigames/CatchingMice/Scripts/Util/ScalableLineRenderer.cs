using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class ScalableLineRenderer : MonoBehaviour 
{
	public float length = 1;
	public bool autoUpdate = false;
	public bool only2DDistance = false;

	protected LineRenderer lineRenderer = null;
	public List<Vector3> points = new List<Vector3>();
	protected Material material = null;

	public void SetupLocal()
	{
		if (lineRenderer == null)
		{
			lineRenderer = GetComponent<LineRenderer>();
		}

		if (lineRenderer == null)
		{
			Debug.LogError("ScalableLineRenderer: Missing line renderer. Disabling.");
			this.enabled = false;
		}
		else
		{
			if (material == null)
			{
				material = lineRenderer.material;
			}

			if (material == null)
			{
				Debug.LogError("ScalableLineRenderer: No material found!");
			}
		}

		SetVertexCount(2);
	}
	
	public void SetupGlobal()
	{
		// lookup references to objects / scripts outside of this script
	}
	
	protected void Awake()
	{
		SetupLocal();
	}

	protected void Start () 
	{
		SetupGlobal();
	}
	
	protected void Update () 
	{
//		if (Input.GetKeyDown(KeyCode.A))
//		{
//			SetVertexCount(2);
//			SetPosition(1, GetPosition(1).x(Random.Range(0f, 5f)));
//			ScaleMaterial();
//		}

		if (autoUpdate)
			ScaleMaterial();
	}

	public void SetPosition(int index, Vector3 position)
	{
		lineRenderer.SetPosition(index, position);

		if (index >= points.Count)
		{
			Debug.LogError("ScalableLineRenderer: Point index is out of bounds. Increase vertex count first.");
			return;
		}
		else if (index < 0)
		{
			Debug.LogError("ScalableLineRenderer: Point index cannot be less than 0.");
			return;
		}

		points[index] = position;
	}

	public void SetVertexCount(int newCount)
	{
		lineRenderer.SetVertexCount(newCount);

		if (newCount < 0)
		{
			Debug.LogError("ScalableLineRenderer: Vertex count cannot be less than 0.");
			return;
		}

		if (newCount > points.Count)
		{
			points.AddRange(new Vector3[newCount - points.Count]);
		}
		else if (newCount < points.Count)
		{
			points.RemoveRange(newCount, points.Count - newCount);
		}
	}

	public Vector3 GetPosition(int index)
	{
		if (index >= points.Count)
		{
			Debug.LogError("ScalableLineRenderer: Point index is out of bounds. Increase vertex count first.");
			return Vector3.zero;
		}
		else if (index < 0)
		{
			Debug.LogError("ScalableLineRenderer: Point index cannot be less than 0.");
			return Vector3.zero;
		}

		return points[index];
	}

	public void ScaleMaterial()
	{
		float distance = GetLength();

		material.mainTextureScale = material.mainTextureScale.x(distance / length);
		material.mainTextureOffset = material.mainTextureOffset.x(-distance / length);
	}

	public float GetLength()
	{
		float distance = 0;

		for (int i = 0; i < points.Count; i++) 
		{
			if ( i == 0 )
				continue;
			
			if (only2DDistance)
			{
				distance += Vector2.Distance(points[i-1].v2(), points[i].v2());
			}
			else
			{
				distance += Vector3.Distance(points[i-1], points[i]);
			}
		}

		return distance;
	}
}
