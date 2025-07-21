using System;
using UnityEngine;

[Serializable]
public struct ParametersDamageSprites
{
	public string idDamage;

	public Sprite blood;

	public Sprite hole;

	[Tooltip("Сдвиг координат к прозрачному фону из центра спрайта")]
	public bool slideToEdge;

	[Tooltip("Координаты в пикселях от верхнего левого угла")]
	public Vector2 positionPixels;

	public ParametersDamageSprites(AreaDamage area, SpriteRenderer renderer, string idDamage)
	{
		this.idDamage = idDamage;
		blood = null;
		hole = null;
		slideToEdge = false;
		positionPixels = area.transform.TransformPoint(area.position);
		positionPixels = renderer.transform.InverseTransformPoint(positionPixels);
		ref Vector2 reference = ref positionPixels;
		float num = positionPixels.x * renderer.sprite.pixelsPerUnit;
		Vector2 pivot = renderer.sprite.pivot;
		reference.x = num + pivot.x;
		ref Vector2 reference2 = ref positionPixels;
		float num2 = positionPixels.y * renderer.sprite.pixelsPerUnit;
		Vector2 pivot2 = renderer.sprite.pivot;
		reference2.y = num2 + pivot2.y;
		positionPixels = new Vector2(Mathf.Floor(positionPixels.x * 100f) / 100f, Mathf.Floor(positionPixels.y * 100f) / 100f);
	}

	public bool IsEmpty()
	{
		return string.IsNullOrEmpty(idDamage);
	}

	public Vector2 GetPosition(Texture2D fromTexture, Texture2D targetTexture)
	{
		Vector2 vector = new Vector2(Mathf.Floor(positionPixels.x / (float)fromTexture.width * (float)targetTexture.width), Mathf.Floor(positionPixels.y / (float)fromTexture.height * (float)targetTexture.height));
		if (slideToEdge)
		{
			return SpriteWithDamage.SlideOnEdge(vector, targetTexture, 3);
		}
		return vector;
	}
}
