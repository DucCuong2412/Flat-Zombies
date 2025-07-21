using UnityEngine;

[AddComponentMenu("Scripts/Area Damage/DamageTextureMaterial")]
public class DamageTextureMaterial : ComponentArea
{
	public enum Action
	{
		Default,
		Multiply
	}

	public static bool enabledComponents = true;

	[Tooltip("Группа, для создания отдельного набора спрайтов с координатами")]
	public string group;

	[Tooltip("Имя, добавляемое в идентификатор")]
	public string nameDamage;

	[Tooltip("Наносимое изображение, например со следами крови")]
	public Texture2D decal;

	[Tooltip("Режим наложения")]
	public Action overlay;

	[Tooltip("Исходная текстура")]
	public Texture2D texture;

	[Tooltip("Материал с шейдером")]
	public Material material;

	[Tooltip("Параметр шейдера с текстурой, который будет изменен")]
	public string propShader = "_MainTex";

	[Tooltip("Спрайты, в которые будут добавлен шейдер")]
	public SpriteRenderer[] renders = new SpriteRenderer[0];

	private bool searchSource;

	public string IdDamage(AreaDamage area)
	{
		return nameDamage + ":" + area.position.x + ";" + area.position.y;
	}

	public string IdDamage()
	{
		return nameDamage + ":" + area.position.x + ";" + area.position.y;
	}

	public override bool ShowIconGizmos(AreaDamage area)
	{
		return renders.Length != 0;
	}

	private void Awake()
	{
		group += propShader;
		base.enabled = (base.enabled && DamageOfSprite.enabledComponents && texture != null && decal != null && propShader != string.Empty && material != null);
	}

	private void Start()
	{
	}

	public override void InitArea(AreaDamage area)
	{
		int num = 0;
		while (base.enabled && DamageOfSprite.enabledComponents && num < renders.Length)
		{
			if (renders[num] == null)
			{
				UnityEngine.Debug.LogError("SpriteRenderer не найден в " + base.gameObject.name + ".DamageTextureMaterial.renders[" + num + "]");
			}
			if (renders[num].material.shader != material.shader)
			{
				renders[num].material = material;
			}
			renders[num].material.SetColor("_Color", renders[num].color);
			renders[num].material.SetTexture(propShader, texture);
			Vector2 vector = new Vector2(area.position.x, area.position.y);
			vector = area.gameObject.transform.TransformPoint(vector);
			vector = renders[num].gameObject.transform.InverseTransformPoint(vector);
			float num2 = vector.x * renders[num].sprite.pixelsPerUnit;
			Vector2 pivot = renders[num].sprite.pivot;
			vector.x = num2 + pivot.x;
			float num3 = vector.y * renders[num].sprite.pixelsPerUnit;
			Vector2 pivot2 = renders[num].sprite.pivot;
			vector.y = num3 + pivot2.y;
			if (searchSource)
			{
				SpriteWithDamage.Create(SpriteWithDamage.GetSourceTexture(texture), group, IdDamage(area), AddDecal, vector);
			}
			else
			{
				SpriteWithDamage.Create(texture, group, IdDamage(area), AddDecal, vector);
			}
			num++;
		}
	}

	private void AddDecal(Texture2D texture, Vector2 position)
	{
		int rotate = Mathf.FloorToInt(Random.Range(0, 10) * 15);
		if (overlay == Action.Default)
		{
			SpriteWithDamage.AddDecal(texture, decal, OverlayDecal, position.x, position.y, rotate);
		}
		else if (overlay == Action.Multiply)
		{
			SpriteWithDamage.AddDecal(texture, decal, MultiplyColor, position.x, position.x, rotate);
		}
		texture.Apply();
	}

	private Color OverlayDecal(Color tex, Color decal)
	{
		tex.r = Mathf.Lerp(tex.r, decal.r, decal.a);
		tex.g = Mathf.Lerp(tex.g, decal.g, decal.a);
		tex.b = Mathf.Lerp(tex.b, decal.b, decal.a);
		return tex;
	}

	private Color MultiplyColor(Color tex, Color decal)
	{
		tex.r *= decal.r;
		tex.g *= decal.g;
		tex.b *= decal.b;
		return tex;
	}

	public override void Activation(AreaDamage area)
	{
		searchSource = true;
		int num = 0;
		while (base.enabled && num < renders.Length)
		{
			texture = (Texture2D)renders[num].material.GetTexture(propShader);
			texture = SpriteWithDamage.GetTexture(texture, IdDamage(area), group);
			renders[num].material.SetTexture(propShader, texture);
			if (texture == null)
			{
				UnityEngine.Debug.LogError("SpriteWithDamage.GetTexture: NULL \n" + base.gameObject.name + ":" + IdDamage(area));
			}
			num++;
		}
	}
}
