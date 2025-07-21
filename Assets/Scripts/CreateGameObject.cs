using UnityEngine;

[AddComponentMenu("PVold/CreateGameObject")]
public class CreateGameObject : ComponentArea
{
	public GameObject obj;

	public int max = 5;

	private GameObject[] list;

	private int current;

	public override void InitArea(AreaDamage area)
	{
		list = new GameObject[max];
	}

	public override void Activation(AreaDamage area)
	{
		GameObject gameObject = null;
		if (current >= max)
		{
			current = 0;
		}
		if (list[current] == null)
		{
			gameObject = Object.Instantiate(obj);
			gameObject.SetActive(value: true);
		}
		else
		{
			gameObject = list[current];
		}
		gameObject.transform.position = area.positionHit;
		gameObject.SetActive(value: true);
		list[current] = gameObject;
		current++;
	}
}
