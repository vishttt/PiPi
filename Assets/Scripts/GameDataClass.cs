using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class GameDataClass
{
	public int TotalCorrectAnswers;
	public List<LevelClass> LevelsData = new List<LevelClass> ();

	//Save and Load Player Data Functions
	public void SaveData (GameDataClass playerData)
	{
		FileManager.WriteToBinaryFile (playerData, false);
	}

	public void LoadData (ref GameDataClass playerData)
	{
		FileManager.ReadFromBinaryFile (ref playerData);
	}
}

[Serializable]
public class LevelClass
{
	public int levelNum;
	public string Active;
	public List<ClassCategory> categories = new List<ClassCategory> ();
}

[Serializable]
public class ClassCategory
{
	public string categotyName;
	public int CorrectAnswers;
}

public enum LevelCategories
{
	History,
	Kings,
	Religion,
	Heritage,
	Places,
	Sports,
	Photos
}