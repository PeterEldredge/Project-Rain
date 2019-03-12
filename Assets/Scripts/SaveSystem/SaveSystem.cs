using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem 
{
	public static readonly string SAVE_FOLDER = Application.dataPath + "/SaveData/";

	public static void Save(string path, string json)
	{
		if(!Directory.Exists(SAVE_FOLDER))
		{
			Directory.CreateDirectory(SAVE_FOLDER);
		}

		File.WriteAllText(path, json);
	}

	public static string Load(string path)
	{
		if(File.Exists(path))
		{
			return File.ReadAllText(path);
		}
		else return null;
	}
	
}
