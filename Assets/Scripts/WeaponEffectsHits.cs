using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Effects Hits")]
public class WeaponEffectsHits : ScriptableObject
{
	public static WeaponEffectsHits[] allLists;

	[Tooltip("Брать со сцены уже созданные объекты с эффектами")]
	public bool takeStage = true;

	[Tooltip("Эффекты при попадании")]
	public EffectHitMaterial[] effectsHitsMaterials = new EffectHitMaterial[0];

	[HideInInspector]
	public GameObject parent;

	public void AwakeWeapon()
	{
		if (parent == null && WeaponCartridge.effectsHitIsEnabled)
		{
			allLists = Resources.FindObjectsOfTypeAll<WeaponEffectsHits>();
			parent = new GameObject(base.name);
			for (int i = 0; i < effectsHitsMaterials.Length; i++)
			{
				for (int j = 0; j < allLists.Length; j++)
				{
					for (int k = 0; k < allLists[j].effectsHitsMaterials.Length; k++)
					{
						if (effectsHitsMaterials[i].drops == allLists[j].effectsHitsMaterials[k].drops)
						{
							effectsHitsMaterials[i].SetList(allLists[j].effectsHitsMaterials[k].sceneObjects);
						}
					}
				}
				effectsHitsMaterials[i].Awake(parent.transform);
			}
		}
		allLists = new WeaponEffectsHits[0];
	}

	public GameObject[] GetObjectsHits(GameObject sample)
	{
		for (int i = 0; i < effectsHitsMaterials.Length; i++)
		{
			for (int j = 0; j < effectsHitsMaterials[i].sceneObjects.Length; j++)
			{
				if (effectsHitsMaterials[i].sceneObjects[j] == sample)
				{
					return effectsHitsMaterials[i].sceneObjects;
				}
			}
		}
		return new GameObject[0];
	}

	public void OnHitBullet(HitBullet hit)
	{
		for (int i = 0; i < effectsHitsMaterials.Length; i++)
		{
			if (!WeaponCartridge.effectsHitIsEnabled)
			{
				break;
			}
			if (hit.collider.sharedMaterial == effectsHitsMaterials[i].material)
			{
				effectsHitsMaterials[i].OnHitBullet(hit);
			}
		}
	}
}
