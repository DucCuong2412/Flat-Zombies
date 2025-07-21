using System.IO;
using UnityEngine;

public class PlayerPrefsFile
{
	public float saveFloat = 0.25f;

	public int saveInt = 7;

	public bool saveBool = true;

	public static bool isCrypt = true;

	public static bool isLoaded;

	public static void SetString(string key, string value)
	{
		LoadFile();
		int num = 0;
		while (true)
		{
			if (num < ParametrFile.variables.Length)
			{
				if (ParametrFile.variables[num].name == key || ParametrFile.variables[num].IsEmpty())
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		ParametrFile.makeSaveFile = (ParametrFile.makeSaveFile || value != ParametrFile.variables[num].value);
		ParametrFile.variables[num].name = key;
		ParametrFile.variables[num].value = value;
	}

	public static void SetBool(string key, bool value)
	{
		if (value)
		{
			SetString(key, ParametrFile.StringTrue);
		}
		else
		{
			SetString(key, ParametrFile.StringFalse);
		}
	}

	public static void SetInt(string key, int value)
	{
		SetString(key, value.ToString());
	}

	public static void SetFloat(string key, float value)
	{
		SetString(key, value.ToString());
	}

	public static void SetVector2(string key, Vector2 vector)
	{
		SetString(key, vector.x.ToString() + "_" + vector.y.ToString());
	}

	public static void SetVector3(string key, Vector3 vector)
	{
		SetString(key, vector.x.ToString() + "_" + vector.y.ToString() + "_" + vector.z.ToString());
	}

	public static void DeleteKey(string key)
	{
		LoadFile();
		for (int i = 0; i < ParametrFile.variables.Length; i++)
		{
			if (ParametrFile.variables[i].name == key)
			{
				ParametrFile.makeSaveFile = true;
				ParametrFile.variables[i] = default(ParametrFile);
			}
		}
	}

	public static void DeleteAll()
	{
		ParametrFile.makeSaveFile = true;
		ParametrFile.variables = new ParametrFile[ParametrFile.variables.Length];
	}

	public static bool HasKey(string key)
	{
		LoadFile();
		for (int i = 0; i < ParametrFile.variables.Length; i++)
		{
			if (ParametrFile.variables[i].name == key)
			{
				return true;
			}
		}
		return false;
	}

	public static bool GetBool(string key, bool defaultValue)
	{
		LoadFile();
		for (int i = 0; i < ParametrFile.variables.Length; i++)
		{
			if (ParametrFile.variables[i].name == key)
			{
				if (ParametrFile.variables[i].value == ParametrFile.StringTrue || ParametrFile.variables[i].value == ParametrFile.StringFalse)
				{
					defaultValue = (ParametrFile.variables[i].value == ParametrFile.StringTrue);
				}
				break;
			}
		}
		return defaultValue;
	}

	public static int GetInt(string key, int defaultValue)
	{
		LoadFile();
		for (int i = 0; i < ParametrFile.variables.Length && (!(ParametrFile.variables[i].name == key) || !int.TryParse(ParametrFile.variables[i].value, out defaultValue)); i++)
		{
		}
		return defaultValue;
	}

	public static float GetFloat(string key, float defaultValue)
	{
		LoadFile();
		for (int i = 0; i < ParametrFile.variables.Length && (!(ParametrFile.variables[i].name == key) || !float.TryParse(ParametrFile.variables[i].value, out defaultValue)); i++)
		{
		}
		return defaultValue;
	}

	public static string GetString(string key, string defaultValue)
	{
		LoadFile();
		for (int i = 0; i < ParametrFile.variables.Length; i++)
		{
			if (ParametrFile.variables[i].name == key)
			{
				defaultValue = ParametrFile.variables[i].value;
				break;
			}
		}
		return defaultValue;
	}

	public static Vector2 GetVector2(string key, Vector2 defaultValue)
	{
		string @string = GetString(key, string.Empty);
		if (string.IsNullOrEmpty(@string))
		{
			return defaultValue;
		}
		char c = '_';
		string[] array = @string.Split(c);
		if (array.Length == 2)
		{
			float.TryParse(array[0], out defaultValue.x);
			float.TryParse(array[1], out defaultValue.y);
		}
		return defaultValue;
	}

	public static Vector3 GetVector3(string key, Vector3 defaultValue)
	{
		string @string = GetString(key, string.Empty);
		if (string.IsNullOrEmpty(@string))
		{
			return defaultValue;
		}
		char c = '_';
		string[] array = @string.Split(c);
		if (array.Length == 3)
		{
			float.TryParse(array[0], out defaultValue.x);
			float.TryParse(array[1], out defaultValue.y);
			float.TryParse(array[2], out defaultValue.z);
		}
		return defaultValue;
	}

	public static string GetFullTextFile(string lineBreak)
	{
		LoadFile();
		string text = string.Empty;
		for (int i = 0; i < ParametrFile.variables.Length; i++)
		{
			if (!ParametrFile.variables[i].IsEmpty())
			{
				text = text + ParametrFile.variables[i].ToString() + lineBreak;
			}
		}
		return text;
	}

	public static void LoadFile()
	{
		if (isLoaded)
		{
			return;
		}
		string path = Path.Combine(Application.persistentDataPath, "cache.vp");
		if (!File.Exists(path))
		{
			return;
		}
		DeleteAll();
		string[] array = File.ReadAllLines(path);
		for (int i = 0; i < array.Length && i < ParametrFile.variables.Length; i++)
		{
			array[i] = Crypt(array[i], array[i].IndexOf(':') == -1);
			int num = array[i].IndexOf(':');
			if (num == -1)
			{
				continue;
			}
			ParametrFile.variables[i].name = array[i].Remove(num);
			ParametrFile.variables[i].value = array[i].Remove(0, num + 1);
			for (int j = 0; j < ParametrFile.variables.Length; j++)
			{
				if (ParametrFile.variables[j].name == ParametrFile.variables[i].name && j != i)
				{
					ParametrFile.variables[i] = default(ParametrFile);
				}
			}
		}
		if (array.Length >= ParametrFile.variables.Length)
		{
			UnityEngine.Debug.LogWarning("Превышение максимального кол-ва строк, которое может вместиться в массив ParametrFile.variables");
		}
		array = new string[0];
		isLoaded = true;
	}

	public static bool Save()
	{
		if (!ParametrFile.makeSaveFile)
		{
			return false;
		}
		string empty = string.Empty;
		isCrypt = (isCrypt && !Application.isEditor);
		ParametrFile.makeSaveFile = false;
		empty = Path.Combine(Application.persistentDataPath, "cache.vp");
		StreamWriter streamWriter = new StreamWriter(empty);
		string text;
		string text2;
		for (int i = 0; i < ParametrFile.variables.Length; i++)
		{
			if (!ParametrFile.variables[i].IsEmpty())
			{
				text = ParametrFile.variables[i].name + ":" + ParametrFile.variables[i].value;
				text = text.Replace("\n", string.Empty);
				text2 = Crypt(text, isCrypt);
				if (text2.IndexOf("\n") != -1)
				{
					text2 = text;
				}
				streamWriter.WriteLine(text2);
			}
		}
		if (!File.Exists(empty))
		{
			UnityEngine.Debug.LogWarning("PlayerPrefsFile.Save: не удалось создать файл:\n" + empty);
			empty = string.Empty;
		}
		streamWriter.Close();
		text = string.Empty;
		text2 = string.Empty;
		return empty != string.Empty;
	}

	public static string Crypt(string text, bool perform)
	{
		if (!perform)
		{
			return text;
		}
		string text2 = string.Empty;
		foreach (char c in text)
		{
			text2 += (char)(c ^ 0x26);
		}
		return text2;
	}
}
