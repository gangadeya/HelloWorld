using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;

public static class JSDataIO {

	public static bool IsFileExists (string fileName) {
		return File.Exists (fileName);
	}

	public static bool IsDirectoryExists (string fileName) {
		return Directory.Exists (fileName);
	}

	public static void CreateFile (string fileName, string content) {
		if (IsFileExists (fileName)) {
			File.Delete (fileName);
		}
		JSHelper.DebugLog (fileName + " Create !");

		StreamWriter streamWriter = File.CreateText (fileName);
		streamWriter.Write (content);
		streamWriter.Close ();
	}

	public static void CreateDirectory (string fileName) {
		if (!IsDirectoryExists (fileName)) {
			JSHelper.DebugLog (fileName + " Create !");
			Directory.CreateDirectory (fileName);
		} else {
			JSHelper.DebugLog (fileName + " Exist !");
		}
	}

	public static void SaveData (string fileName, object obj) {
		string saveData = SerializeObject (obj);
		CreateFile (fileName, saveData);
	}

	public static object LoadData (string fileName, Type type) {
		if (IsFileExists (fileName)) {
			StreamReader streamReader = File.OpenText (fileName);
			string loadData = streamReader.ReadToEnd ();
			streamReader.Close ();
			return DeserializeObject (loadData, type);
		}
		return null;
	}

	private static string SerializeObject (object obj) {
		string serializedString = string.Empty;
		serializedString = JsonConvert.SerializeObject (obj);
		return serializedString;
	}

	private static object DeserializeObject (string str, Type type) {
		object deserializedObject = null;
		deserializedObject = JsonConvert.DeserializeObject (str, type);
		return deserializedObject;
	}

}
