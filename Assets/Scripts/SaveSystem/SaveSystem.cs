using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem 
{
	public static readonly string SAVE_FOLDER = Application.dataPath + "/SaveData/";

	public static void Save(string file, string json)
	{
		if(!Directory.Exists(SAVE_FOLDER))
		{
			Directory.CreateDirectory(SAVE_FOLDER);
		}

		File.WriteAllText(SAVE_FOLDER + file, json);
	}

	public static string Load(string file)
	{
		if(File.Exists(SAVE_FOLDER + file))
		{
			return File.ReadAllText(SAVE_FOLDER + file);
		}
		else return null;
	}
}
