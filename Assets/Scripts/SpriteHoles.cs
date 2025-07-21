using UnityEngine;

public class SpriteHoles
{
	public static bool isSupported = false;

	public static string testedNameShader = string.Empty;

	public static bool TestMaterial(Material material)
	{
		if (testedNameShader != material.shader.name && (!material.HasProperty("PositionHole1") || !material.HasProperty("CurrentHole")))
		{
			UnityEngine.Debug.LogWarning("[SpriteHoles] " + material.shader.name + ".isSupported: false");
			isSupported = false;
		}
		else
		{
			isSupported = true;
		}
		testedNameShader = material.shader.name;
		return isSupported;
	}

	public static Texture GetTextureHole(Material material)
	{
		return material.GetTexture("_HoleTex");
	}

	public static Texture GetTextureBlood(Material material)
	{
		return material.GetTexture("_BloodTex");
	}

	public static bool UpdateCurrentHolePixel(SpriteRenderer render, Vector2 positionPixels, Material materialSource)
	{
		Vector4 value = default(Vector4);
		value.x = Mathf.Floor(positionPixels.x / (float)render.sprite.texture.width * 1000f) / 1000f;
		value.y = Mathf.Floor(positionPixels.y / (float)render.sprite.texture.height * 1000f) / 1000f;
		value.z = Random.Range(0, 36) * 10;
		value.w = 1f;
		if (render.material.shader.name != materialSource.shader.name && TestMaterial(materialSource))
		{
			render.material = materialSource;
			render.material.SetInt("CurrentHole", 1);
			render.material.SetInt("_WidthMainTex", render.sprite.texture.width);
			render.material.SetInt("_HeightMainTex", render.sprite.texture.height);
		}
		int num = render.material.GetInt("CurrentHole");
		int num2 = num - 1;
		if (render.material.HasProperty("PositionHole" + num))
		{
			if (num2 >= 1)
			{
				Vector4 vector = render.material.GetVector("PositionHole" + num2);
				if (vector.x == value.x && vector.y == value.y)
				{
					render.material.SetInt("CurrentHole", num2);
					num = num2;
				}
			}
			render.material.SetVector("PositionHole" + num, value);
			return true;
		}
		return false;
	}

	public static bool UpdateCurrentHole(SpriteRenderer render, Vector2 globalPosition, Material material)
	{
		if (render.sprite == null)
		{
			return false;
		}
		Vector2 vector = render.gameObject.transform.InverseTransformPoint(globalPosition);
		if (render.flipX)
		{
			vector.x *= -1f;
		}
		if (render.flipY)
		{
			vector.y *= -1f;
		}
		float num = vector.x * render.sprite.pixelsPerUnit;
		Vector2 pivot = render.sprite.pivot;
		Vector2 positionPixels = default(Vector2);
		positionPixels.x = num + pivot.x;
		float num2 = vector.y * render.sprite.pixelsPerUnit;
		Vector2 pivot2 = render.sprite.pivot;
		positionPixels.y = num2 + pivot2.y;
		return UpdateCurrentHolePixel(render, positionPixels, material);
	}

	public static bool AddHole(SpriteRenderer render, Vector2 globalPosition, Material material)
	{
		if (render.material.HasProperty("CurrentHole"))
		{
			render.material.SetInt("CurrentHole", render.material.GetInt("CurrentHole") + 1);
		}
		return UpdateCurrentHole(render, globalPosition, material);
	}

	public static int GetCurrentHole(SpriteRenderer render)
	{
		if (render.material.HasProperty("CurrentHole"))
		{
			return render.material.GetInt("CurrentHole");
		}
		return 0;
	}

	public static int GetCurrentHole(Material material)
	{
		if (material.HasProperty("CurrentHole"))
		{
			return material.GetInt("CurrentHole");
		}
		return 0;
	}
}
