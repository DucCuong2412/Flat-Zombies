using UnityEngine;

[CreateAssetMenu(menuName = "List Damages Sprites")]
public class ListDamagesSprites : ScriptableObject, IScreenLoadingGame
{
	public Sprite spriteSource;

	public string group = string.Empty;

	[Tooltip("Создать каждое повреждение отдельно, если все повреждения не будут отображаться вместе на одной текстуре")]
	public bool damageAsGroup;

	public ParametersDamageSprites[] damages = new ParametersDamageSprites[0];

	public Sprite[] skins = new Sprite[0];

	private int currentAngle;

	private int current = -1;

	private ParametersDamageSprites damage;

	public void OnStartLoad(LoadingGameObject loader)
	{
		currentAngle = Mathf.FloorToInt(UnityEngine.Random.Range(0, 10) * 15);
		current = 0;
	}

	public bool OnStepLoad(LoadingGameObject loader)
	{
		damage = damages[current];
		SpriteWithDamage.Create(spriteSource, GetGroup(damage.idDamage), damage.idDamage, CreateBloodHoleTex, damage.positionPixels);
		for (int i = 0; i < skins.Length; i++)
		{
			currentAngle += 15;
			currentAngle %= 360;
			//SpriteWithDamage.Create(skins[i], GetGroup(damage.idDamage), damage.idDamage, CreateBloodHoleTex, damage.GetPosition(spriteSource.texture, skins[i].texture));
		}
		//loader.textCurrentObject.text = damage.positionPixels.x + " : " + damage.positionPixels.y;
		current++;
		return current == damages.Length;
	}

	public void Create(Sprite original)
	{
		for (int i = 0; i < damages.Length; i++)
		{
			damage = damages[i];
			currentAngle += 15;
			currentAngle %= 360;
			SpriteWithDamage.Create(original, GetGroup(damage.idDamage), damage.idDamage, CreateBloodHoleTex, damage.GetPosition(spriteSource.texture, original.texture));
		}
	}

	public void CreateDamage(Sprite current, string idDamage)
	{
		int num = 0;
		while (true)
		{
			if (num < damages.Length)
			{
				damage = damages[num];
				if (damage.idDamage == idDamage)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		currentAngle += 15;
		currentAngle %= 360;
		SpriteWithDamage.Create(current, GetGroup(damage.idDamage), damage.idDamage, CreateBloodHoleTex, damage.positionPixels);
	}

	public void CreateBloodHoleTex(Texture2D texture, Vector2 positionOfPixels)
	{
		if (damage.hole != null && damage.hole.texture.width != damage.hole.texture.height)
		{
			currentAngle = 0;
			positionOfPixels.x = (float)texture.width / 2f;
			positionOfPixels.y = (float)texture.height / 2f;
		}
		if (damage.hole != null)
		{
			SpriteWithDamage.AddHoleTexture(texture, damage.hole.texture, positionOfPixels.x, positionOfPixels.y, currentAngle);
		}
		if (damage.blood != null)
		{
			SpriteWithDamage.AddBlood(texture, damage.blood.texture, positionOfPixels.x, positionOfPixels.y, currentAngle);
		}
		texture.Apply();
	}

	public bool IsSprite(Sprite sprite)
	{
		if (sprite == spriteSource)
		{
			return true;
		}
		for (int i = 0; i < skins.Length; i++)
		{
			if (sprite == skins[i])
			{
				return true;
			}
		}
		return false;
	}

	private string GetGroup(string idDamage)
	{
		if (damageAsGroup)
		{
			return idDamage + group;
		}
		return group;
	}

	public Sprite GetSprite(Sprite current, string idDamage)
	{
		for (int i = 0; i < damages.Length; i++)
		{
			if (damages[i].idDamage == idDamage)
			{
				if (IsSprite(current) || IsSprite(SpriteWithDamage.GetSourceSprite(current)))
				{
					return SpriteWithDamage.GetSprite(current, idDamage, GetGroup(idDamage));
				}
				return current;
			}
		}
		return current;
	}
}
