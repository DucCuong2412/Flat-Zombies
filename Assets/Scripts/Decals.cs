using UnityEngine;

[AddComponentMenu("PVold/Decals")]
public class Decals : MonoBehaviour
{
	[Tooltip("Максимальное кол-во")]
	public int max = 5;

	public GameObject original;

	private GameObject[] decals;

	private int current;

	private void Start()
	{
		decals = new GameObject[max];
		original.SetActive(value: false);
	}

	public GameObject AddDecal(Vector3 position)
	{
		GameObject gameObject = null;
		if (current >= max)
		{
			current = 0;
		}
		if (decals[current] == null)
		{
			gameObject = UnityEngine.Object.Instantiate(original);
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
		return gameObject;
	}
}
