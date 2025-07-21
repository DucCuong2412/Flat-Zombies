using UnityEngine;

public class DeadZombie : MonoBehaviour
{
	public Material materialHolesSkin;

	public ZombieCharacteristic character;

	public Color colorBody = new Color(1f, 1f, 1f);

	public Color colorLegs = new Color(1f, 1f, 1f);

	public SpriteRenderer[] spritesOfBody = new SpriteRenderer[0];

	public SpriteRenderer[] spritesOfLegs = new SpriteRenderer[0];

	private SpriteRenderer[] renders = new SpriteRenderer[0];

	private SpriteRenderer[] allRenders = new SpriteRenderer[0];

	private void OnDrawGizmosSelected()
	{
		if (!base.enabled || spritesOfBody.Length == 0 || spritesOfLegs.Length == 0 || (!(spritesOfLegs[0].color != colorLegs) && !(spritesOfBody[0].color != colorBody)))
		{
			return;
		}
		if (character != null)
		{
			if (character.colorsBody.Length != 0)
			{
				colorBody = character.colorsBody[0];
			}
			if (character.colorsLegs.Length != 0)
			{
				colorLegs = character.colorsLegs[0];
			}
		}
		for (int i = 0; i < spritesOfBody.Length; i++)
		{
			spritesOfBody[i].color = colorBody;
		}
		for (int j = 0; j < spritesOfLegs.Length; j++)
		{
			spritesOfLegs[j].color = colorLegs;
		}
	}

	private void Start()
	{
		OnDrawGizmosSelected();
		if (character != null)
		{
			Color newColorBody = colorBody;
			Color newColorLegs = colorLegs;
			if (character.colorsBody.Length != 0)
			{
				newColorBody = character.colorsBody[Random.Range(0, character.colorsBody.Length)];
			}
			if (character.colorsLegs.Length != 0)
			{
				newColorLegs = character.colorsLegs[Random.Range(0, character.colorsLegs.Length)];
			}
			character.ChangeSprite(base.gameObject, colorBody, newColorBody, colorLegs, newColorLegs);
		}
		renders = new SpriteRenderer[spritesOfBody.Length + spritesOfLegs.Length];
		for (int i = 0; i < renders.Length; i++)
		{
			if (i < spritesOfBody.Length)
			{
				renders[i] = spritesOfBody[i];
			}
			else
			{
				renders[i] = spritesOfLegs[i - spritesOfBody.Length];
			}
		}
		if (!DamageOfSprite.enabledComponents || renders.Length == 0)
		{
			return;
		}
		Vector2[] array = new Vector2[5];
		int[] array2 = new int[array.Length];
		Bounds bounds = default(Bounds);
		for (int j = 0; j < array.Length; j++)
		{
			array2[j] = UnityEngine.Random.Range(0, renders.Length);
			bounds = renders[array2[j]].bounds;
			ref Vector2 reference = ref array[j];
			Vector3 min = bounds.min;
			float min2 = min.x * 100f;
			Vector3 max = bounds.max;
			reference.x = UnityEngine.Random.Range(min2, max.x * 100f) / 100f;
			ref Vector2 reference2 = ref array[j];
			Vector3 min3 = bounds.min;
			float min4 = min3.y * 100f;
			Vector3 max2 = bounds.max;
			reference2.y = UnityEngine.Random.Range(min4, max2.y * 100f) / 100f;
			for (int k = 0; k < array2.Length; k++)
			{
				if (k != j && array2[j] == array2[k])
				{
					array[j] = Vector2.zero;
				}
			}
		}
		for (int l = 0; l < array.Length; l++)
		{
			if (array[l].x == 0f && array[l].y == 0f)
			{
				continue;
			}
			SpriteHoles.AddHole(renders[array2[l]], array[l], materialHolesSkin);
			ShowSpriteMeat(renders[array2[l]].transform);
			for (int m = 0; m < renders.Length; m++)
			{
				if (renders[m].bounds.Contains(array[l]) && (renders[m].transform.parent == renders[array2[l]].transform || renders[array2[l]].transform.parent == renders[m].transform))
				{
					SpriteHoles.AddHole(renders[m], array[l], materialHolesSkin);
					ShowSpriteMeat(renders[m].transform);
					break;
				}
			}
		}
	}

	public void ShowSpriteMeat(Transform body)
	{
		if (allRenders.Length == 0)
		{
			allRenders = base.gameObject.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		}
		int num = 0;
		while (true)
		{
			if (num < allRenders.Length)
			{
				if (!allRenders[num].gameObject.activeSelf && allRenders[num].transform.parent == body)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		allRenders[num].gameObject.SetActive(value: true);
	}

	private void OnDestroy()
	{
		allRenders = new SpriteRenderer[0];
		spritesOfBody = new SpriteRenderer[0];
		spritesOfLegs = new SpriteRenderer[0];
	}
}
