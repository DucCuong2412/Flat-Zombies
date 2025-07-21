using UnityEngine;

public class SorterSpriteRenderer : MonoBehaviour
{
	public static int currentOrder = 0;

	public static int numOnStage = 0;

	public static SorterSpriteRenderer[] onStage = new SorterSpriteRenderer[100];

	[Tooltip("Добавить сдвиг при сортировке")]
	public float slideY;

	public SpriteRenderer[] sprites = new SpriteRenderer[0];

	[Tooltip("Обновлять список спрайтов при выполнении сортировки")]
	public bool updateList;

	[Space(6f)]
	public int maxOrder;

	public int sortingOrderSprites;

	public static SorterSpriteRenderer main => onStage[0];

	public static void Add(SorterSpriteRenderer sorter)
	{
		numOnStage++;
		for (int i = 0; i < onStage.Length; i++)
		{
			if (onStage[i] == null)
			{
				onStage[i] = sorter;
				break;
			}
		}
		UpdateSortingOrder();
	}

	public static void Remove(SorterSpriteRenderer sorter)
	{
		numOnStage--;
		for (int i = 0; i < onStage.Length; i++)
		{
			if (onStage[i] == sorter)
			{
				onStage[i] = null;
			}
			if (onStage[i] == null && i + 1 < onStage.Length && onStage[i + 1] != null)
			{
				onStage[i] = onStage[i + 1];
				onStage[i + 1] = null;
			}
		}
		UpdateSortingOrder();
	}

	public static void UpdateSortingOrder()
	{
		int num = 0;
		int num2 = 0;
		for (num = 0; num < numOnStage; num++)
		{
			onStage[num].SortingOrderReset();
			for (num2 = 0; num2 < numOnStage; num2++)
			{
				if (num != num2)
				{
					Vector3 position = onStage[num].transform.position;
					float num3 = position.y + onStage[num].slideY;
					Vector3 position2 = onStage[num2].transform.position;
					if (num3 < position2.y + onStage[num2].slideY)
					{
						onStage[num].AddSortingOrder(onStage[num2].maxOrder);
						continue;
					}
				}
				if (num != num2)
				{
					Vector3 position3 = onStage[num].transform.position;
					float num4 = position3.y + onStage[num].slideY;
					Vector3 position4 = onStage[num2].transform.position;
					if (num4 == position4.y + onStage[num2].slideY)
					{
						onStage[num2].AddSortingOrder(onStage[num].maxOrder);
					}
				}
			}
		}
	}

	private void Awake()
	{
		if (sprites.Length == 0)
		{
			sprites = base.gameObject.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		}
		for (int i = 0; i < sprites.Length; i++)
		{
			maxOrder = Mathf.Max(maxOrder, sprites[i].sortingOrder);
		}
	}

	private void Start()
	{
		Add(this);
	}

	private void OnDestroy()
	{
		Remove(this);
		sprites = null;
	}

	public void AddSortingOrder(int order)
	{
		sortingOrderSprites += order;
		for (int i = 0; i < sprites.Length; i++)
		{
			if (sprites[i] != null)
			{
				sprites[i].sortingOrder += order;
			}
		}
	}

	public void SortingOrderReset()
	{
		if (updateList)
		{
			sprites = base.gameObject.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		}
		for (int i = 0; i < sprites.Length; i++)
		{
			if (sprites[i] != null)
			{
				sprites[i].sortingOrder -= sortingOrderSprites;
			}
		}
		sortingOrderSprites = 0;
	}
}
