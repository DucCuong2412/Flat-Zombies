using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpriteWithDamage
{
	public delegate void Method(Texture2D texture, Vector2 positionOfPixels);

	public delegate Color MethodOverlayColor(Color colorTex, Color colorDecal);

	public const int maxSprites = 512;

	public static bool generateMipMaps = true;

	public static ObjectDamage[] sprites = new ObjectDamage[0];

	private static ObjectDamage[] tempDamages = new ObjectDamage[0];

	public static int numCopySprite = 1;

	[CompilerGenerated]
	private static MethodOverlayColor _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static MethodOverlayColor _003C_003Ef__mg_0024cache1;

	public static void AddDamage(ObjectDamage damage)
	{
		if (sprites.Length >= 512)
		{
			UnityEngine.Debug.LogWarning("SpriteWithDamage: Превышение кол-ва спрайтов;\n Кол-во спрайтов должно быть меньше " + 512);
			return;
		}
		tempDamages = sprites;
		sprites = new ObjectDamage[tempDamages.Length + 1];
		for (int i = 0; i < tempDamages.Length; i++)
		{
			sprites[i] = tempDamages[i];
		}
		sprites[tempDamages.Length] = damage;
		tempDamages = new ObjectDamage[0];
	}

	public static bool Create(Sprite current, string group, string id, Method createDamages, Vector2 position)
	{
		for (int i = 0; i < sprites.Length; i++)
		{
			if (sprites[i].IsCreated(current, id, group))
			{
				return false;
			}
		}
		Sprite sprite = CopySprite(current);
		Sprite sourceSprite = GetSourceSprite(current);
		createDamages(sprite.texture, position);
		AddDamage(new ObjectDamage(sourceSprite, sprite, group, id));
		int num = sprites.Length - 1;
		for (int j = 0; j < num; j++)
		{
			if (sprites[j].group == group && sprites[j].source == sourceSprite && !sprites[j].CheckID(id))
			{
				sprite = CopySprite(sprites[j].created as Sprite);
				createDamages(sprite.texture, position);
				AddDamage(new ObjectDamage(sourceSprite, sprite, group, id, sprites[j].listID));
			}
		}
		sprite = null;
		return true;
	}

	public static bool Create(Sprite source, string id, Method createDamages, Vector2 position)
	{
		return Create(source, string.Empty, id, createDamages, position);
	}

	public static Sprite CopySprite(Sprite source)
	{
		Vector2 pivot = source.pivot;
		float x = pivot.x / (float)source.texture.width;
		Vector2 pivot2 = source.pivot;
		Vector2 pivot3 = new Vector2(x, pivot2.y / (float)source.texture.height);
		Sprite sprite = Sprite.Create(rect: new Rect(0f, 0f, source.texture.width, source.texture.height), texture: new Texture2D(source.texture.width, source.texture.height, source.texture.format, generateMipMaps && source.texture.mipmapCount != 0), pivot: pivot3, pixelsPerUnit: source.pixelsPerUnit, extrude: 0u, meshType: SpriteMeshType.FullRect, border: source.border);
		sprite.texture.SetPixels32(source.texture.GetPixels32());
		sprite.texture.wrapMode = source.texture.wrapMode;
		sprite.texture.filterMode = source.texture.filterMode;
		sprite.texture.anisoLevel = source.texture.anisoLevel;
		sprite.texture.name = numCopySprite.ToString() + "." + source.name;
		sprite.name = numCopySprite.ToString() + "." + source.name;
		numCopySprite++;
		return sprite;
	}

	public static void SpriteTrim(Sprite source)
	{
	}

	public static bool Create(Texture2D source, string group, string id, Method createDamages, Vector2 position)
	{
		Texture2D texture2D = null;
		Texture2D texture2D2 = null;
		for (int i = 0; i < sprites.Length; i++)
		{
			if (sprites[i].IsCreated(source, id, group))
			{
				return false;
			}
		}
		texture2D = new Texture2D(source.width, source.height, source.format, generateMipMaps && source.mipmapCount != 0);
		texture2D.SetPixels32(source.GetPixels32());
		texture2D.Apply();
		texture2D.wrapMode = source.wrapMode;
		texture2D.filterMode = source.filterMode;
		texture2D.anisoLevel = source.anisoLevel;
		texture2D.name = "d." + source.name;
		createDamages(texture2D, position);
		AddDamage(new ObjectDamage(source, texture2D, group, id));
		int num = sprites.Length - 1;
		for (int j = 0; j < num; j++)
		{
			if (sprites[j].group == group && sprites[j].source == source && !sprites[j].CheckID(id))
			{
				texture2D2 = sprites[j].texture;
				texture2D = new Texture2D(texture2D2.width, texture2D2.height, texture2D2.format, generateMipMaps && texture2D2.mipmapCount != 0);
				texture2D.SetPixels32(texture2D2.GetPixels32());
				texture2D.wrapMode = texture2D2.wrapMode;
				texture2D.filterMode = texture2D2.filterMode;
				texture2D.anisoLevel = texture2D2.anisoLevel;
				texture2D.name = "d." + texture2D2.name;
				createDamages(texture2D, position);
				AddDamage(new ObjectDamage(source, texture2D, group, id, sprites[j].listID));
			}
		}
		texture2D2 = null;
		texture2D = null;
		return true;
	}

	public static Vector2 PositionToPixels(Vector2 position, Sprite sprite)
	{
		float num = position.x * sprite.pixelsPerUnit;
		Vector2 pivot = sprite.pivot;
		position.x = num + pivot.x;
		float height = sprite.rect.height;
		float num2 = position.y * sprite.pixelsPerUnit;
		Vector2 pivot2 = sprite.pivot;
		position.y = height - (num2 + pivot2.y);
		return position;
	}

	public static Sprite GetSprite(Sprite sprite, string idNext, string group)
	{
		return GetCreated(sprite, idNext, group) as Sprite;
	}

	public static Texture2D GetTexture(Texture2D texture, string idNext, string group)
	{
		return GetCreated(texture, idNext, group) as Texture2D;
	}

	public static UnityEngine.Object GetCreated(UnityEngine.Object currentSprite, string idNext, string group)
	{
		int num = -1;
		for (int i = 0; i < sprites.Length; i++)
		{
			if (sprites[i].group == group && sprites[i].created == currentSprite)
			{
				num = i;
				break;
			}
			if (sprites[i].IsCopy(currentSprite, idNext, group))
			{
				return sprites[i].created;
			}
		}
		if (num != -1)
		{
			if (sprites[num].CheckID(idNext))
			{
				return sprites[num].created;
			}
			for (int j = 0; j < sprites.Length; j++)
			{
				if (sprites[j].IsCopy(sprites[num].source, sprites[num].listID, idNext, group))
				{
					return sprites[j].created;
				}
			}
		}
		return currentSprite;
	}

	public static Sprite GetSourceSprite(Sprite sprite)
	{
		for (int i = 0; i < sprites.Length; i++)
		{
			if (sprites[i].created == sprite)
			{
				return sprites[i].source as Sprite;
			}
		}
		return sprite;
	}

	public static Texture2D GetSourceTexture(Texture2D texture)
	{
		for (int i = 0; i < sprites.Length; i++)
		{
			if (sprites[i].created == texture)
			{
				return sprites[i].sourceTexture;
			}
		}
		return null;
	}

	public static ObjectDamage GetDamage(UnityEngine.Object sprite)
	{
		for (int i = 0; i < sprites.Length; i++)
		{
			if (sprites[i].created == sprite)
			{
				return sprites[i];
			}
		}
		return null;
	}

	public static void AddBlood(Texture2D texture, Texture2D decal, float centerX, float centerY, int rotate)
	{
		AddDecal(texture, decal, OverlayBlood, centerX, centerY, rotate);
	}

	public static Color OverlayBlood(Color tex, Color decal)
	{
		tex.r = Mathf.Lerp(tex.r, decal.r, decal.a);
		tex.g = Mathf.Lerp(tex.g, decal.g, decal.a);
		tex.b = Mathf.Lerp(tex.b, decal.b, decal.a);
		return tex;
	}

	public static void AddHoleTexture(Texture2D texture, Texture2D hole, float centerX, float centerY, int rotate)
	{
		AddDecal(texture, hole, OverlayHole, centerX, centerY, rotate);
	}

	public static Color OverlayHole(Color tex, Color decal)
	{
		tex.a *= decal.r;
		return tex;
	}

	public static void AddColorInsteadAlpha(Texture2D texture, Texture2D decal, bool apply = true)
	{
		Color c = default(Color);
		Color32[] pixels = texture.GetPixels32();
		Color32[] pixels2 = decal.GetPixels32();
		for (int i = 0; i < pixels2.Length && i < pixels.Length; i++)
		{
			c = pixels[i];
			Color color = pixels2[i];
			if (c.a == 0f)
			{
				c = color;
			}
			else
			{
				c.r = Mathf.Lerp(c.r, color.r, (1f - c.a) * color.a);
				c.g = Mathf.Lerp(c.g, color.g, (1f - c.a) * color.a);
				c.b = Mathf.Lerp(c.b, color.b, (1f - c.a) * color.a);
				c.a = Mathf.Min(color.a + c.a, 1f);
			}
			pixels[i] = c;
		}
		texture.SetPixels32(pixels);
		if (apply)
		{
			texture.Apply();
		}
	}

	public static void AddDecal(Texture2D texture, Texture2D decal, MethodOverlayColor OverlayColor, float centerX, float centerY, int rotate)
	{
		int num = Mathf.FloorToInt(Mathf.Sqrt(decal.width * decal.width + decal.height * decal.height) / 2f);
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = Mathf.FloorToInt((float)decal.width * 0.5f);
		int num7 = Mathf.FloorToInt((float)decal.height * 0.5f);
		Rect rect = default(Rect);
		rect.x = centerX - (float)num;
		rect.y = centerY - (float)num;
		rect.xMax = Mathf.Floor(centerX + (float)num);
		rect.yMax = Mathf.Floor(centerY + (float)num);
		Rect rect2 = default(Rect);
		rect2.x = Mathf.Max(0f, rect.x);
		rect2.xMax = Mathf.Min(texture.width, rect.xMax);
		rect2.y = Mathf.Max(0f, rect.y);
		rect2.yMax = Mathf.Min(texture.height, rect.yMax);
		if (!(rect2.height > 1f) || !(rect2.width > 1f))
		{
			return;
		}
		for (int i = Mathf.FloorToInt(rect2.y); (float)i <= rect2.yMax; i++)
		{
			for (int j = Mathf.FloorToInt(rect2.x); (float)j <= rect2.xMax; j++)
			{
				num2 = j - Mathf.FloorToInt(centerX - (float)num6);
				num3 = i - Mathf.FloorToInt(centerY - (float)num7);
				num4 = Mathf.FloorToInt((float)(num2 - num6) * Mathf.Cos((float)Math.PI / 180f * (float)rotate) + (float)(num7 - num3) * Mathf.Sin((float)Math.PI / 180f * (float)rotate));
				num5 = Mathf.FloorToInt((float)(num2 - num6) * Mathf.Sin((float)Math.PI / 180f * (float)rotate) + (float)(num3 - num7) * Mathf.Cos((float)Math.PI / 180f * (float)rotate));
				num2 = num6 + num4;
				num3 = num7 + num5;
				if (0 < num2 * num3 && num2 < decal.width && num3 < decal.height)
				{
					Color pixel = texture.GetPixel(j, texture.height - i);
					Color pixel2 = decal.GetPixel(num2, num3);
					pixel = OverlayColor(pixel, pixel2);
					texture.SetPixel(j, texture.height - i, pixel);
				}
			}
		}
	}

	public static Vector2 SlideOnEdge(Vector2 position, Texture2D meat, int stepSlide)
	{
		float num = Mathf.RoundToInt(position.x);
		float num2 = Mathf.RoundToInt(position.y);
		float num3 = Mathf.Atan2(0.5f * (float)meat.height - position.y, 0.5f * (float)meat.width - position.x) / (float)Math.PI * 180f;
		float num4 = (float)stepSlide * Mathf.Cos(num3 / 180f * (float)Math.PI);
		float num5 = (float)stepSlide * Mathf.Sin(num3 / 180f * (float)Math.PI);
		int num6 = 0;
		while (num < (float)meat.width && num > 0f && num2 < (float)meat.height && num2 > 0f)
		{
			num -= num4;
			num2 -= num5;
			Color pixel = meat.GetPixel(Mathf.RoundToInt(num), Mathf.RoundToInt(num2));
			if (pixel.a != 0f)
			{
				position.x = Mathf.RoundToInt(num);
				position.y = Mathf.RoundToInt(num2);
			}
			num6++;
		}
		return position;
	}
}
