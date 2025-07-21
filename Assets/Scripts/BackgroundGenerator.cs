using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
	public Transform[] furnituries;

	private Transform selected;

	private void Start()
	{
		furnituries = base.gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
		for (int i = 0; i < furnituries.Length; i++)
		{
			furnituries[i].gameObject.SetActive(value: true);
		}
		for (int j = 0; j < base.transform.childCount; j++)
		{
			SelectChild(base.transform.GetChild(j));
		}
		furnituries = new Transform[0];
	}

	private void SelectChild(Transform parent)
	{
		int childCount = parent.childCount;
		if (childCount < 2)
		{
			return;
		}
		selected = parent.GetChild(UnityEngine.Random.Range(0, childCount));
		for (int i = 0; i < parent.childCount; i++)
		{
			if (selected != parent.GetChild(i))
			{
				UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
			}
		}
		SelectChild(selected);
	}
}
