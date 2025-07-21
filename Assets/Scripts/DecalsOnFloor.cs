using System;
using UnityEngine;

public class DecalsOnFloor : MonoBehaviour
{
	[Serializable]
	public struct Decal
	{
		public string type;

		public GameObject original;
	}

	[Tooltip("Максимальное кол-во")]
	public int max;

	public Decal[] list;

	private GameObject[] decals;

	private int current;

	private void Start()
	{
		decals = new GameObject[max];
		for (int i = 0; i < list.Length; i++)
		{
			list[i].original.SetActive(value: false);
		}
	}

	public GameObject AddDecal(Vector3 position, string type)
	{
		GameObject gameObject = null;
		if (current >= max)
		{
			current = 0;
		}
		for (int i = 0; i < list.Length; i++)
		{
			if (list[i].type == type)
			{
				if (decals[current] == null)
				{
					gameObject = UnityEngine.Object.Instantiate(list[i].original);
					gameObject.SetActive(value: true);
				}
				else
				{
					gameObject = decals[current];
				}
				gameObject.transform.position = position;
				gameObject.transform.SetParent(base.gameObject.transform);
				decals[current] = gameObject;
				current++;
				break;
			}
		}
		return gameObject;
	}
}
