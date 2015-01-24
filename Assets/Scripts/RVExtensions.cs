using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public static class RVExtensions
{
	public static T LastItem<T>(this List<T> list)
	{
		if (list.Count > 0)
			return list[list.Count - 1];
		else
			return default(T);
	}

	public static Vector3 PositionWhereYisZ(this Rect rect)
	{
		return new Vector3(rect.position.x, 0f, rect.position.y); 
	}

	public static Vector3 Vector2ToVector3YixZ(this Vector2 vec2)
	{
		return new Vector3(vec2.x, 0f, vec2.y);
	}

	public static Vector3 AddToX(this Vector3 vector3, float x)
	{
		vector3.x += x;
		return vector3;
	}

	public static Vector3 AddToY(this Vector3 vector3, float y)
	{
		vector3.y += y;
		return vector3;
	}

	public static Vector3 AddToZ(this Vector3 vector3, float z)
	{
		vector3.z += z;
		return vector3;
	}

	public static Vector3 SetX(this Vector3 vector3, float x)
	{
		vector3.x = x;
		return vector3;
	}

	public static Vector3 SetY(this Vector3 vector3, float y)
	{
		vector3.y = y;
		return vector3;
	}

	public static Vector3 SetZ(this Vector3 vector3, float z)
	{
		vector3.z = z;
		return vector3;
	}

    public static float SignedAngle(Vector3 v1, Vector3 v2, Vector3 normal)
    {
        return (Mathf.Atan2(Vector3.Dot(normal, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg);
    }

    public static Vector3 Parse(this Vector3 vector3, string x, string y, string z)
    {
        float xValue, yValue, zValue;

        if (!float.TryParse(x, out xValue) | !float.TryParse(y, out yValue) |
            !float.TryParse(z, out zValue))
        {
            return vector3;
        }
        return new Vector3(xValue, yValue, zValue);
    }

    public static Vector3 SecondaryAxis(this Vector3 axis) 
    { 
        return new Vector3(axis.y, axis.z, axis.x); 
    }

    public static Vector3 InvertedAxis(this Vector3 axis)
    {
        Vector3 returnAxis = axis;

        returnAxis.x = (axis.x == 0) ? 1 : 0;
        returnAxis.y = (axis.y == 0) ? 1 : 0;
        returnAxis.z = (axis.z == 0) ? 1 : 0;

        return returnAxis;
    }

    public static Vector3 CrossAxis(this Vector3 axis)
    { 
        return Vector3.Cross(axis, axis.SecondaryAxis()); 
    }

    public static Vector3 RotatePointAroundPivot(this Vector3 point, Vector3 pivot, Quaternion angles)
    {
        Vector3 dir = point - pivot;
        dir = angles * dir;
        point = dir + pivot;
        return point;
    }

    public static Vector3 RotatePointAroundPivot(this Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(angles) * dir;
        point = dir + pivot;
        return point;
    }

    public static void SetLayerRecursively(this GameObject go, int layer, bool changeClashesLayer = true)
    {
        foreach (Transform t in go.GetComponentsInChildren<Transform>())
        {
            if (t.gameObject.layer != LayerMask.NameToLayer("Clashes")){

                t.gameObject.layer = layer;

            }
            //in the case that is has Clashes as layer only change the layer if changeClashesLayer is true
            else {
                if (changeClashesLayer)
                {
                    t.gameObject.layer = layer;
                }
            }
        }
    }

	public static Transform FindChildRecursively(this Transform trans, string name)
	{
		Transform child = null;
		foreach (Transform t in trans.GetComponentsInChildren<Transform>(true))
		{
            if (t != trans)
            {
                if (t.name == name)
                {
                    child = t;
                    break;
                }
            }
		}

		return child;
	}

    public static GameObject GetRootParent(this GameObject go) 
    {
        while (go.transform.parent != null) 
        {
            go = go.transform.parent.gameObject;
        }

        return go;
    }

    public static bool IsNullOrWhiteSpace(this string value)
    {
        if (value == null)
        {
            return true;
        }

        int index = 0;
        while (index < value.Length)
        {
            if (char.IsWhiteSpace(value[index]))
            {
                index++;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsNullWhiteSpaceOrEmpty(this string value) 
    {
        if (value == null)
        {
            return true;
        }

        if (String.IsNullOrEmpty(value) == true) 
        {
            return true;
        }

        int index = 0;
        while (index < value.Length)
        {
            if (char.IsWhiteSpace(value[index]))
            {
                index++;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public static bool Contains2D(this Bounds b, Vector2 point)
    {
        Vector2 min2D = new Vector2(b.min.x, b.min.z);
        Vector2 max2D = new Vector2(b.max.x, b.max.z);

        if (min2D.x < point.x && min2D.y < point.y && point.x < max2D.x && point.y < max2D.y)
        {
            return true;
        }
        return false;
    }

    // Removes any leftover trailing characters and stubborn whitespaces that are not removed by calling Trim()
    //public static string RemoveWhitespaceTrailingChars(this string inputString)
    //{
    //    return Regex.Replace(inputString, @"^\s*$\n", string.Empty, RegexOptions.Multiline).TrimEnd();
    //}

    /// <summary>
    ///  Removes any leftover trailing characters and stubborn whitespaces that are not removed by calling Trim()
    /// </summary>
    /// <param name="inputString"></param>
    public static string TrimEndSpacesTrailChars(this string inputString)
    {
       return Regex.Replace(inputString, @"^\s*$\n", string.Empty, RegexOptions.Multiline).TrimEnd();
    }

    public static string TrimAllSpaces(this string inputString) 
    {
        return TrimEndSpacesTrailChars(inputString.Trim());
    }

	public static string ColorToRGBAHex(this Color color)
	{
		Color32 c = color;
		string hex = c.r.ToString("X2") + c.g.ToString("X2") + c.b.ToString("X2") + c.a.ToString("X2");
		return hex;
	}

	public static Color RGBAHexToColor(this Color c, string hex)
	{
		c.r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
		c.g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
		c.b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
		c.a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

		return c;
	}

    public static Color SetR(this Color color, float r)
    {
        color.r = r;
        return color;
    }

    public static Color SetG(this Color color, float g)
    {
        color.g = g;
        return color;
    }

    public static Color SetB(this Color color, float b)
    {
        color.b = b;
        return color;
    }

    public static Color SetA(this Color color, float a)
    {
        color.a = a;
        return color;
    }
}




public static class MonoBehaviorExt
{
	public static Coroutine<T> StartCoroutine<T>(this MonoBehaviour obj, IEnumerator coroutine)
	{
		Coroutine<T> coroutineObject = new Coroutine<T>();
		coroutineObject.coroutine = obj.StartCoroutine(coroutineObject.InternalRoutine(coroutine));
		return coroutineObject;
	}
}

public class Coroutine<T>
{
	public T Value
	{
		get
		{
			if (e != null)
			{
				throw e;
			}
			return returnVal;
		}
	}
	private T returnVal;
	private Exception e;
	public Coroutine coroutine;

	public IEnumerator InternalRoutine(IEnumerator coroutine)
	{
		while (true)
		{
			try
			{
				if (!coroutine.MoveNext())
				{
					yield break;
				}
			}
			catch (Exception e)
			{
				this.e = e;
				yield break;
			}
			object yielded = coroutine.Current;
			if (yielded != null && yielded.GetType() == typeof(T))
			{
				returnVal = (T)yielded;
				yield break;
			}
			else
			{
				yield return coroutine.Current;
			}
		}
	}
}