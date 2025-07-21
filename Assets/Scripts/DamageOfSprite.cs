using UnityEngine;

[AddComponentMenu("Scripts/Area Damage/DamageOfSprite")]
public class DamageOfSprite : ComponentArea
{
	public static bool enabledComponents = true;

	[HideInInspector]
	public string idDamage = string.Empty;

	[Tooltip("SpriteRenderer, для извлечения спрайтов, на которых будут созданы повреждения при активации")]
	public SpriteRenderer[] list = new SpriteRenderer[0];

	[Tooltip("Файл-источник со спрайтами, из которого будет получена информация о повреждении")]
	public ListDamagesSprites[] sources = new ListDamagesSprites[0];

	private bool IsReady()
	{
		return base.enabled && idDamage != string.Empty;
	}

	public override bool ShowIconGizmos(AreaDamage area)
	{
		return IsReady();
	}

	public override void Activation(AreaDamage area)
	{
		if (!enabledComponents)
		{
			return;
		}
		for (int i = 0; i < list.Length && list[i] != null && list[i].sprite != null; i++)
		{
			list[i].gameObject.SetActive(value: true);
			for (int j = 0; j < sources.Length; j++)
			{
				list[i].sprite = sources[j].GetSprite(list[i].sprite, idDamage);
			}
		}
	}

	private void Start()
	{
	}
}
