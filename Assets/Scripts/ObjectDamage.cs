using UnityEngine;

public class ObjectDamage
{
	public Object source;

	public Object created;

	public string[] listID = new string[1];

	public string group = string.Empty;

	public Texture2D sourceTexture => source as Texture2D;

	public Texture2D texture => created as Texture2D;

	public Sprite sourceSprite => source as Sprite;

	public Sprite sprite => created as Sprite;

	public ObjectDamage(Object source, Object created, string group, string idDamage, string[] damagesSource)
	{
		this.source = source;
		this.created = created;
		this.group = group.Trim();
		idDamage = idDamage.Trim();
		listID = new string[damagesSource.Length + 1];
		listID[damagesSource.Length] = idDamage;
		int num = 0;
		while (true)
		{
			if (num < damagesSource.Length)
			{
				listID[num] = damagesSource[num];
				if (damagesSource[num] == idDamage)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		UnityEngine.Debug.LogError("#512484 ObjectDamage:\nИдентификатор уже существует - " + idDamage);
	}

	public ObjectDamage(Object source, Object created, string group, string idDamage)
	{
		this.source = source;
		this.created = created;
		this.group = group.Trim();
		listID[0] = idDamage.Trim();
	}

	public bool CheckID(string id)
	{
		id = id.Trim();
		for (int i = 0; i < listID.Length; i++)
		{
			if (id == listID[i])
			{
				return true;
			}
		}
		return false;
	}

	public string GetListID()
	{
		string text = string.Empty;
		for (int i = 0; i < listID.Length; i++)
		{
			text = text + "[" + listID[i] + "]";
		}
		return text;
	}

	public bool IsCopy(Object sourceTex, string[] idDamage, string idNextDamage, string groupDamage)
	{
		if (groupDamage != group || !CheckID(idNextDamage) || sourceTex != source)
		{
			return false;
		}
		for (int i = 0; i < idDamage.Length; i++)
		{
			if (!string.IsNullOrEmpty(idDamage[i]) && !CheckID(idDamage[i]))
			{
				return false;
			}
		}
		return true;
	}

	public bool IsCopy(Object sourceTex, string idDamage, string groupDamage)
	{
		return groupDamage == group && sourceTex == source && listID.Length == 1 && listID[0] == idDamage;
	}

	public bool IsCreated(Object texSourceCurrent, string idDamage, string groupDamage)
	{
		return groupDamage == group && (texSourceCurrent == source || texSourceCurrent == created) && CheckID(idDamage);
	}
}
