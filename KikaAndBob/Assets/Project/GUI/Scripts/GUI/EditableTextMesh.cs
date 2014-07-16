using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(TextMesh))]
public class EditableTextMesh : MonoBehaviour {

	public string defaultTextKey = "";

	protected BoxCollider boxCollider;
	protected TextMesh textMesh;
	protected bool editing = false;
	protected string editedString = "";
	protected bool useScreenKeyboard = false;

#if UNITY_IPHONE || UNITY_ANDROID
	protected TouchScreenKeyboard keyBoard;
#endif

	protected TextMeshWrapper wrapper = null;
	protected string defaultText = "";
	
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

		#if UNITY_IPHONE || UNITY_ANDROID
		if (useScreenKeyboard)
			TouchScreenKeyboard.hideInput = false;
#endif

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
	}

	protected void Start () 
	{
		SetupGlobal();
	}

	public void SetupGlobal()
	{
		LugusResources.use.Localized.onResourcesReloaded += UpdateDefaultText;
		defaultText = LugusResources.use.GetText(defaultTextKey);
		Reset();
		AlterTransparency();
	}

	protected void UpdateDefaultText()
	{
		defaultText = LugusResources.use.GetText(defaultTextKey);
	}
	
	public void Reset()
	{
		editedString = "";
		textMesh.text = defaultText;
	}

	public bool IsDefaultValue()
	{
		return editedString == defaultText;
	}
	
	public string GetEnteredString()
	{
		return editedString;
	}

	public void SetEnteredString(string newContent)
	{
		if (string.IsNullOrEmpty(newContent))
		{
			Debug.LogError("EditableTextMesh: Cannot set entered string to empty. Resetting instead.");
			Reset();
			return;
		}

		editedString = newContent;

		textMesh.text = editedString;   
		
		if (wrapper != null)
		{
			wrapper.UpdateWrapping();
		}
	}

	void Update () 
	{
		if (editing == true)
		{
			if (useScreenKeyboard)
			{
#if UNITY_IPHONE || UNITY_ANDROID
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
#endif
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
#if UNITY_IPHONE || UNITY_ANDROID
				keyBoard = TouchScreenKeyboard.Open(editedString, TouchScreenKeyboardType.Default, false, false, false);
#endif
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
			GUI.SetNextControlName ("HiddenTextField"); //Prepare a Control Name so we can focus the TextField
			GUI.FocusControl ("HiddenTextField");       //Focus the TextField
			editedString = GUI.TextField (new Rect (90, -100, 200, 25), editedString, 32);    //Display a TextField outside the Screen Rect
		}
	}
}
