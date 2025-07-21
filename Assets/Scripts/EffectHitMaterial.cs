using System;
using UnityEngine;

[Serializable]
public class EffectHitMaterial
{
	public static int minNumDrops = 1;

	[Tooltip("Метка игрового объекта, в которое произошло попадание")]
	public PhysicsMaterial2D material;

	[Tooltip("Объект с брызгом крови или с другим эффектом попадания пули")]
	public GameObject drops;

	[Tooltip("Максимальное кол-во")]
	public int max = 1;

	[HideInInspector]
	public GameObject[] sceneObjects = new GameObject[0];

	private int current;

	private MonoBehaviour[] behaviours = new MonoBehaviour[0];

	private IEffectHitBullet objectEffectHit;

	public void SetList(GameObject[] objectsOnStage)
	{
		if (objectsOnStage.Length != 0 && !(objectsOnStage[0] == null))
		{
			max = Mathf.Max(minNumDrops, max);
			sceneObjects = new GameObject[Mathf.Max(max, objectsOnStage.Length)];
			for (int i = 0; i < sceneObjects.Length && i < objectsOnStage.Length; i++)
			{
				sceneObjects[i] = objectsOnStage[i];
			}
		}
	}

	public void Awake(Transform parent)
	{
		current = 0;
		max = Mathf.Max(minNumDrops, max);
		if (sceneObjects.Length == 0 || sceneObjects[0] == null)
		{
			sceneObjects = new GameObject[max];
		}
		for (int i = 0; i < sceneObjects.Length; i++)
		{
			if (sceneObjects[i] == null)
			{
				sceneObjects[i] = UnityEngine.Object.Instantiate(drops);
				sceneObjects[i].name = material.name + "." + drops.name + "." + i.ToString();
				sceneObjects[i].SetActive(value: false);
				sceneObjects[i].transform.SetParent(parent);
			}
		}
	}

	public void OnHitBullet(HitBullet hitBullet)
	{
		current %= sceneObjects.Length;
		sceneObjects[current].SetActive(value: true);
		behaviours = sceneObjects[current].GetComponents<MonoBehaviour>();
		objectEffectHit = null;
		for (int i = 0; i < behaviours.Length; i++)
		{
			if (behaviours[i] is IEffectHitBullet)
			{
				objectEffectHit = (behaviours[i] as IEffectHitBullet);
				objectEffectHit.OnEffectHitBullet(hitBullet);
			}
		}
		if (objectEffectHit == null)
		{
			sceneObjects[current].transform.position = hitBullet.raycastHit.point;
		}
		behaviours = new MonoBehaviour[0];
		current++;
	}
}
