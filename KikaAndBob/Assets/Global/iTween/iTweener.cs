using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class iTweener : MonoBehaviour 
{
    public enum TweenType
    {
        NONE,
        MoveTo,
        ScaleTo,
        RotateTo,
		RotateBy
    }

    protected static Vector3 NOTHING = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

    public TweenType type = TweenType.NONE;

    public GameObject _subject = null;

    public Vector3[] _targetPositions = null;

    public Vector3 _targetPosition = NOTHING;
    public Vector3 _targetScale = NOTHING;
    public Vector3 _targetRotation = NOTHING;

    public Vector3 _originalPosition = NOTHING;
    public Vector3 _originalScale = NOTHING;
    public Vector3 _originalRotation = NOTHING;

    public float _time = float.MaxValue;
    public float _speed = float.MaxValue;
    public float _delay = float.MaxValue;
    public iTween.EaseType _easeType = iTween.EaseType.linear;
    public iTween.LoopType _loopType = iTween.LoopType.none;

    public bool _local = false;
    public bool _moveToPath = true;

    public float _lookAhead = 0.0f;
    public float _lookTime = 0.0f;
    public bool _orientToPath = false;
    public string _axis = string.Empty;

    public bool _stopOthers = false;
    public bool _ignoreTimeScale = false;

    public iTweener Subject(GameObject go)
    {
        _subject = go;

        return this;
    }

    public iTweener TargetPosition(Vector3 target)
    {
        _targetPosition = target;
        return this;
    }

    public iTweener TargetPositions(Vector3[] path)
    {
        _targetPositions = path;
        return this;
    }

    public iTweener TargetScale(Vector3 target)
    {
        _targetScale = target;
        return this;
    }

    public iTweener TargetRotation(Vector3 target)
    {
        _targetRotation = target;
        return this;
    }

    public iTweener Time(float time)
    {
        _time = time;
        return this;
    }

    public iTweener Speed(float speed)
    {
        _speed = speed;
        return this;
    }

    public iTweener Delay(float delay)
    {
        _delay = delay;
        return this;
    }

    public iTweener EaseType(iTween.EaseType easeType)
    {
        _easeType = easeType;
        return this;
    }

    public iTweener Looptype(iTween.LoopType loopType)
    {
        _loopType = loopType;
        return this;
    }

    public iTweener IsLocal(bool local)
    {
        _local = local;
        return this;
    }

    public iTweener IgnoreTimeScale(bool ignore)
    {
        _ignoreTimeScale = ignore;
        return this;
    }

    public iTweener StopOthers(bool stop)
    {
        _stopOthers = stop;
        return this;
    }

    public iTweener MoveToPath(bool moveToPath)
    {
        _moveToPath = moveToPath;
        return this;
    }

    public iTweener LookAhead(float lookAhead)
    {
        _lookAhead = lookAhead;
        return this;
    }

    public iTweener LookTime(float lookTime)
    {
        _lookTime = lookTime;
        return this;
    }

    public iTweener OrientToPath(bool orientToPath)
    {
        _orientToPath = orientToPath;
        return this;
    }

    public iTweener Axis(string axis)
    {
        _axis = axis;
        return this;
    }

    public iTweener Store()
    {
        _originalScale = _subject.transform.localScale;
        _originalPosition = _subject.transform.position;
        return this;
    }

    // needed to make the code easily usable in ping-pong while loop:
    //  iTweener tweener = gameObject.MoveTo( new Vector3(10, 10, 10) ).Time(2.0f).Store();
    //	while( true )
    //	{
    //		tweener.Revert(); // otherwhise, this would not work the first time
    //		yield return new WaitForSeconds(2.0f);
    //	}

    protected bool hasRevertedBefore = false;

    public iTweener Revert()
    {
        if (hasRevertedBefore)
        {
            Vector3 temp = Vector3.zero;

            temp = _targetPosition;
            _targetPosition = _originalPosition;
            _originalPosition = temp;

            temp = _targetScale;
            _targetScale = _originalScale;
            _originalScale = temp;

            temp = _targetRotation;
            _targetRotation = _originalRotation;
            _originalRotation = temp;
        }
        else
        {
            hasRevertedBefore = true;
        }


        Execute();

        return this;
    }

    protected Hashtable BuildHashtable()
    {
        Hashtable output = new Hashtable();

        if (_speed != float.MaxValue)
        {
            output.Add("speed", _speed);
        }
        else
        {
            if (_time != float.MaxValue)
                output.Add("time", _time);
        }

        if (_delay != float.MaxValue)
            output.Add("delay", _delay);

        //if( _target != NOTHING )
        //{

        // NOTE: adding scale and rotation in a MoveTo has no effect (and vice versa for Scale and Rotate functions
        if (_targetPositions == null)
            output.Add("position", _targetPosition);
        else
        {
            output.Add("path", _targetPositions);
            output.Add("movetopath", _moveToPath);
        }

        output.Add("scale", _targetScale);

		if( type != TweenType.RotateBy )
       		output.Add("rotation", _targetRotation);
		else
			output.Add("amount", _targetRotation); 


        if (_local)
        {
            output.Add("islocal", _local);
        }

        if (_ignoreTimeScale)
        {
            output.Add("ignoretimescale", _ignoreTimeScale);
        }

        //}

        output.Add("easetype", _easeType);

        output.Add("looptype", _loopType);

        output.Add("orienttopath", _orientToPath);

        if (_lookAhead != 0.0f)
            output.Add("lookahead", _lookAhead);

        if (_lookTime != 0.0f)
            output.Add("looktime", _lookTime);

        if (_axis != string.Empty)
            output.Add("axis", _axis);

        return output;
    }

    public void Execute()
    {
        if (_stopOthers)
        {
            this.gameObject.StopTweens();
        }

        Hashtable arguments = BuildHashtable();

        if (type == TweenType.MoveTo)
            iTween.MoveTo(_subject, arguments);
        else if (type == TweenType.ScaleTo)
            iTween.ScaleTo(_subject, arguments);
        else if (type == TweenType.RotateTo)
			iTween.RotateTo(_subject, arguments);
		else if (type == TweenType.RotateBy)
			iTween.RotateBy(_subject, arguments);

        GameObject.Destroy(this);

        //return this;
    }
}

// List class, especially to make "mappend" functions possible
// i.e. Execute() on all iTweeners in here
// or: change duration (Time(xx)) for all tweens for easy syncing
public class iTweenerList
{
    public List<iTweener> tweeners = new List<iTweener>();

    public iTweenerList(iTweener[] tweenerArray)
    {
        tweeners.AddRange(tweenerArray);
    }

    public void Execute()
    {
        foreach (iTweener tweener in tweeners)
            tweener.Execute();
    }

    public void Revert()
    {
        foreach (iTweener tweener in tweeners)
            tweener.Revert();
    }

    public void Time(float time)
    {
        foreach (iTweener tweener in tweeners)
            tweener.Time(time);
    }
}

public static class iTweenExtensions
{
	// Note: difference from iTween : rotationAmount here is NOT normalized, so values between [0,360] are expected
	public static iTweener RotateBy(this GameObject go, Vector3 rotationAmount)
	{
		iTweener output = go.AddComponent<iTweener>();

		// iTween itself expects normalized values for RotateBy
		if( rotationAmount.x > 1.0f || rotationAmount.y > 1.0f || rotationAmount.z > 1.0f ||
		    rotationAmount.x < -1.0f || rotationAmount.y < -1.0f || rotationAmount.z < -1.0f)
		{
			rotationAmount /= 360.0f;
		}
		
		output.Subject(go);
		output.type = iTweener.TweenType.RotateBy;
		output.TargetRotation(rotationAmount);
		
		return output;
	}

    public static iTweener RotateTo(this GameObject go, Vector3 targetRotation)
    {
        iTweener output = go.AddComponent<iTweener>();

        output.Subject(go);
        output.type = iTweener.TweenType.RotateTo;
        output.TargetRotation(targetRotation);

        return output;
    }

    public static iTweener MoveTo(this GameObject go, Transform target)
    {
        return go.MoveTo(target.position);
    }

    public static iTweener MoveTo(this GameObject go, Vector3 targetPosition)
    {
        iTweener output = go.AddComponent<iTweener>();

        output.Subject(go);
        output.type = iTweener.TweenType.MoveTo;
        output.TargetPosition(targetPosition);

        return output;
    }

    public static iTweener LobTo(this GameObject go, Vector3 begin, Vector3 end, float yOffset = 2.0f )
    {
        Vector3[] path = new Vector3[3];

        path[0] = begin;
        path[2] = end;
        path[1] = Vector3.Lerp(path[0], path[2], .5f).yAdd(yOffset);

        return go.MoveTo(path);
    }

    public static iTweener MoveTo(this GameObject go, Vector3[] path)
    {
        iTweener output = go.AddComponent<iTweener>();

        output.Subject(go);
        output.type = iTweener.TweenType.MoveTo;
        output.TargetPositions(path);

        return output;
    }

    public static iTweener ScaleTo(this GameObject go, Vector3 targetScale)
    {
        iTweener output = go.AddComponent<iTweener>();

        output.Subject(go);
        output.type = iTweener.TweenType.ScaleTo;
        output.TargetScale(targetScale);

        return output;
    }

    public static iTweener MoveBy(this GameObject go, float yValue)
    {
        iTweener output = go.AddComponent<iTweener>();






        return output;
    } 

    public static void StopTweens(this GameObject go)
    {
        iTween.Stop(go);
    }

    // always returns the first iTweener on the object
    // only really usefull if you only have 1 iTweener on the object
    // otherwhise: use tweeners() to get the list
    public static iTweener tweener(this GameObject go)
    {
        return go.GetComponent<iTweener>();
    }

    public static iTweenerList tweeners(this GameObject go)
    {
        iTweenerList tweeners = new iTweenerList(go.GetComponents<iTweener>());
        return tweeners;
    }
}