using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(TextMesh))]
public class EditableTextMesh : MonoBehaviour {
	
	public string defaultText = "";
	BoxCollider boxCollider;
	TextMesh textMesh;
	bool editing = false;
	string editedString = "";
	bool useScreenKeyboard = false;
	TouchScreenKeyboard keyBoard;
	TextMeshWrapper wrapper = null;
	
	protected void Awake () 
	{
		SetupLocal();
	}

	public void SetupLocal()
	{
#if UNITY_EDITOR
		useScreenKeyboard = false;
#elif UNITY_ANDROID || UNITY_IOS
		useScreenKeyboard = true;
#else
		useScreenKeyboard = false;
#endif

		if (useScreenKeyboard)
			TouchScreenKeyboard.hideInput = false;

		if (boxCollider == null)
		{
			boxCollider = GetComponent<BoxCollider>();
		}

		if (boxCollider == null)
		{
			Debug.LogError("EditableTextMesh: Missing box collider.");
		}

		if (textMesh == null)
		{
			textMesh = GetComponent<TextMesh>();
		}

		if (textMesh == null)
		{
			Debug.LogError("EditableTextMesh: Missing text mesh.");
		}

		if (wrapper == null)
		{
			wrapper = GetComponent<TextMeshWrapper>();
		}

		Reset();
		AlterTransparency();
	}
	
	public void Reset()
	{
		editedString = "";
		textMesh.text = defaultText;
	}
	
	public string GetEnteredString()
	{
		return editedString;
	}

	void Update () 
	{
		if (editing == true)
		{
			if (useScreenKeyboard)
			{
				editedString = keyBoard.text;
				textMesh.text = editedString;

				if (wrapper != null)
				{
					wrapper.UpdateWrapping();
				}
				
				if (keyBoard.wasCanceled || keyBoard.done)
				{
					editing = false;
					AlterTransparency();	

					if (string.IsNullOrEmpty(editedString))
						Reset();
				}
			}
			else
			{
				textMesh.text = editedString;   

				if (wrapper != null)
				{
					wrapper.UpdateWrapping();
				}

				if (Input.GetKeyDown(KeyCode.Return) || 
				    Input.GetKeyDown(KeyCode.KeypadEnter) || 
				    (LugusInput.use.down == true && LugusInput.use.RayCastFromMouseDown(LugusCamera.ui) != transform) )
				{
					editing = false;
					AlterTransparency();

					if (string.IsNullOrEmpty(editedString))
						Reset();
				}
			}
		}
		else if (editing == false && LugusInput.use.RayCastFromMouseDown(LugusCamera.ui) == transform)
		{
			editing = true;
			AlterTransparency();

			if (useScreenKeyboard)
			{
				keyBoard = TouchScreenKeyboard.Open(editedString, TouchScreenKeyboardType.Default, false, false, false);
			}
		}
	
	}
	
	void AlterTransparency()
	{
		if (editing)
			renderer.material.color = renderer.material.color.a(1);
		else
			renderer.material.color = renderer.material.color.a(0.7f);
	}

	void OnGUI()
	{
		if (!useScreenKeyboard && editing)
		{
			GUI.SetNextControlName ("hiddenTextField"); //Prepare a Control Name so we can focus the TextField
			GUI.FocusControl ("hiddenTextField");       //Focus the TextField
			editedString = GUI.TextField (new Rect (90, -100, 200, 25), editedString, 32);    //Display a TextField outside the Screen Rect
		}
	}
}
