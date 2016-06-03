using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad
{
	public static float[] savedGames = { 0.0f, 0.0f, 0.0f };

	public static void Save(EDifficulty mode, float score)
	{
		savedGames[(int)mode] = score;
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.persistentDataPath + "/savedGames.save", FileMode.OpenOrCreate);
		bf.Serialize(file, savedGames);
		file.Close();
	}

	public static void Load()
	{
		if (File.Exists(Application.persistentDataPath + "/savedGames.save"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/savedGames.save", FileMode.Open);
			savedGames = (float[])bf.Deserialize(file);
			file.Close();
		}
	}
}
