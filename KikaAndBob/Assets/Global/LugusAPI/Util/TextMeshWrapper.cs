using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))] 
public class TextMeshWrapperHelper : LugusSingletonRuntime<TextMeshWrapperHelper>
{
	public void WrapText( TextMesh targetMesh, float maxWidth, bool allowSplit = true )
	{
		TextMesh workMesh = GetComponent<TextMesh>();

		workMesh.font = targetMesh.font;
		workMesh.alignment = targetMesh.alignment;
		workMesh.fontSize = targetMesh.fontSize;
		workMesh.characterSize = targetMesh.characterSize;
		
		targetMesh.text = targetMesh.text.Replace("<br>", "\n");
		targetMesh.text = targetMesh.text.Replace("<br/>", "\n");
		targetMesh.text = targetMesh.text.Replace("\\n", "\n");
		
		string[] words = targetMesh.text.Split(' ');
		
		string newString = "";
		string textString = "";
		
		for( int i = 0; i < words.Length; ++i )
		{
			textString = textString + words[i] + " ";
			workMesh.text = textString;
			
			float textSize = workMesh.renderer.bounds.size.x;
			
			
			//Debug.Log("WrapText : textSize is now " + textSize + ", ySize: " + mesh.renderer.bounds.size.y + ", zSize: " + mesh.renderer.bounds.size.z + " -> " + newString);
			
			if( allowSplit && (textSize > maxWidth) )
			{
				textString = words[i] + " ";
				newString += "\n" + words[i] + " ";
			}
			else
				newString += words[i] + " ";
		}
		
		//workMesh.text = newString;
		
		targetMesh.text = newString;
	}

	public void Awake()
	{
		this.transform.position = new Vector3(9999.0f, 9999.0f, 9999.0f);
	}
}


[RequireComponent(typeof(TextMesh))] 
public class TextMeshWrapper : MonoBehaviour 
{
	public float width = -1f;
	public float height = -1f;

	public bool autoUpdate = false;
	public bool allowSmallerCharacterSize = true;
	public bool allowSplit = true;

	public TextMesh textMesh;

	protected string savedText;
	protected float originalCharacterSize = 1.0f;

	void Awake()
	{

		if( width <= 0 )
		{
			// derive width from boxcollider
			BoxCollider collider = GetComponent<BoxCollider>();
			if( collider != null )
			{
				width = collider.bounds.size.x;
			}
			else
			{
				BoxCollider2D collider2 = GetComponent<BoxCollider2D>();
				if( collider2 != null )
				{
					width = collider2.size.x;
				}
				else
				{
					width = LugusUtil.UIWidth * 0.9f;
				}
			}
		}

		if( height <= 0 )
		{
			// derive width from boxcollider
			BoxCollider collider = GetComponent<BoxCollider>();
			if( collider != null )
			{
				height = collider.bounds.size.y;
			}
			else
			{
				BoxCollider2D collider2 = GetComponent<BoxCollider2D>();
				if( collider2 != null )
				{
					height = collider2.size.y;
				}
				else
				{
					height = LugusUtil.UIHeight * 0.9f;
				}
			}
		}

		textMesh = GetComponent<TextMesh>(); 
		if (textMesh == null)
		{
			Debug.LogError("TextMeshWrapper: Missing text mesh!", gameObject);
			return;
		}

		savedText = textMesh.text;

		originalCharacterSize = textMesh.characterSize;
	}

	void Start ()
	{
		//TextMeshWrapperHelper.use.WrapText(textMesh, width, allowSplit);
		UpdateWrapping();
	}

	public void SetTextKey(string key)
	{
		SetText ( LugusResources.use.Localized.GetText(key) );
	}

	public void SetText(string text)
	{
		textMesh.text = text;
		UpdateWrapping();
	}

	public void UpdateWrapping()
	{
		bool proceed = true;
		textMesh.characterSize = this.originalCharacterSize;
		savedText = textMesh.text;

		if (width <= 0 || height <= 0)
		{
			Debug.LogError("TextMeshWrapper: Width or height was smaller than or equal to 0.");
			return;
		}

		while( proceed )
		{
			textMesh.text = savedText;


			TextMeshWrapperHelper.use.WrapText(textMesh, width, allowSplit);

			
			if(textMesh.renderer.bounds.size.y > height || textMesh.renderer.bounds.size.x > width)
			{
				proceed = true;
				textMesh.characterSize -= this.originalCharacterSize / 10.0f;
			}
			else
				proceed = false;
		}
	}

	void Update () 
	{	
		if ( autoUpdate && textMesh.text != savedText)
		{
			UpdateWrapping();
		}
	}
	
}
