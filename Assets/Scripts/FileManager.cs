using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class FileManager
{
	private static string FilePath = Application.persistentDataPath + "/playerInfo.dat";

	public static void WriteToBinaryFile<T> (T objectToWrite, bool append = true) where T : new()
	{
		if (File.Exists (FilePath)) {
			File.Delete (FilePath);
		}

		BinaryFormatter serializer = new BinaryFormatter ();
		FileStream file = File.Create (FilePath);
		serializer.Serialize (file, objectToWrite);
		Debug.Log ("edit on the player Data");
	}

	public static void ReadFromBinaryFile<T> (ref T objectToRead) where T : new()
	{
		if (!File.Exists (FilePath)) {
			//Create new File Data with Defult Parameters
			GameEngine._instance.IntialzeTheNewData ();
			WriteToBinaryFile (objectToRead, false);
			Debug.Log ("Create new Player Data");
		} else {
			Debug.Log ("Read the existing data file" + "  " + Application.persistentDataPath);
			BinaryFormatter serializer = new BinaryFormatter ();
			FileStream file = File.Open (FilePath, FileMode.Open);
			objectToRead = (T)serializer.Deserialize (file);
			Debug.Log (file.ToString ());
			file.Close ();

		}
	}
}
