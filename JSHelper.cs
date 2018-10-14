/// <summary>
/// Helpful static func here
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public static class JSHelper {
	/// <summary>
	/// Debug message func
	/// </summary>
	/// <param name="message">Message.</param>
	#region DEBUG
	public static void DebugLog (object message = null)
	{
		#if UNITY_EDITOR || DEBUG
		Debug.Log (message);
		#endif
	}

	public static void DebugLogError (object message = null)
	{
		#if UNITY_EDITOR || DEBUG
		Debug.LogError (message);
		#endif
	}

	public static void DebugDrawLine (Vector3 start, Vector3 end, Color color, float duration)
	{
		#if UNITY_EDITOR || DEBUG
		Debug.DrawLine (start, end, color, duration);
		#endif
	}

	#endregion

	/// <summary>
	/// Find Object Func
	/// </summary>
	/// <returns>The in children.</returns>
	/// <param name="go">Go.</param>
	/// <param name="name">Name.</param>
	/// <param name="includeInactive">If set to <c>true</c> include inactive.</param>
	#region Find
	public static GameObject FindInChildren(this GameObject go, string name, bool includeInactive = false)
	{
		try
		{
			return (from x in go.GetComponentsInChildren<Transform>(includeInactive)
				where x.gameObject.name == name
				select x.gameObject).First();
		}
		catch(Exception e)
		{
			DebugLog (e);
			return null;
		}
	}
	#endregion

	/// <summary>
	/// Scene control
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	/// <param name="mode">Mode.</param>
	#region Scene
	public static void LoadScene (string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
	{
		DebugLog ("Load Scene : " + sceneName);
		SceneManager.LoadScene (sceneName, mode);
	}
	#endregion

	/// <summary>
	/// PLayerPres control
	/// </summary>
	/// <returns>The prefs get string.</returns>
	/// <param name="key">Key.</param>
	/// <param name="value">Value.</param>
	#region PlayerPrefs
	public static string PlayerPrefsGetString(string key, string value = null)
	{
		if (!PlayerPrefs.HasKey (key)) {
			DebugLogError ("key : " + key + " in PlayerPrefs not found !");
		}
		return PlayerPrefs.GetString (key, value);
	}

	public static void PlayerPrefsSetString(string key, string value = null)
	{
		if (PlayerPrefs.HasKey (key)) {
			DebugLog ("save data in exist key : " + key);
		} else {
			DebugLog ("save data in new key : " + key);
		}
		PlayerPrefs.SetString (key, value);
	}

	public static int PlayerPrefsGetInt(string key, int value = 0)
	{
		if (!PlayerPrefs.HasKey (key)) {
			DebugLogError ("key : " + key + " in PlayerPrefs not found !");
		}
		return PlayerPrefs.GetInt (key, value);
	}

	public static void PlayerPrefsSetInt(string key, int value)
	{
		if (PlayerPrefs.HasKey (key)) {
			DebugLog ("save data in exist key : " + key);
		} else {
			DebugLog ("save data in new key : " + key);
		}
		PlayerPrefs.SetInt (key, value);
	}

	public static float PlayerPrefsGetFloat(string key, float value = 0)
	{
		if (!PlayerPrefs.HasKey (key)) {
			DebugLogError ("key : " + key + " in PlayerPrefs not found !");
		}
		return PlayerPrefs.GetFloat (key, value);
	}

	public static void PlayerPrefsSetFloat(string key, float value)
	{
		if (PlayerPrefs.HasKey (key)) {
			DebugLog ("save data in exist key : " + key);
		} else {
			DebugLog ("save data in new key : " + key);
		}
		PlayerPrefs.SetFloat (key, value);
	}
	#endregion

	#region Load Resource

	public static UnityEngine.Object LoadRes (string path) {
		return Resources.Load (path);
	}

	#endregion

	#region Value Change

	public static string VectorToString (Vector3 vec) {
		return vec.x.ToString () + ',' + vec.y.ToString () + ',' + vec.z.ToString ();
	}

	#endregion
}
