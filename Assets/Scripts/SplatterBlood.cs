using PVold;
using UnityEngine;

public class SplatterBlood : MonoBehaviour
{
	public static SplatterBlood main;

	[Tooltip("Максимальное кол-во")]
	public int max = 5;

	public Blood drops;

	private GameObject[] listDrops;

	private int current;

	private Blood blood;

	private AnimatorSprite animatorCloud;

	private void Awake()
	{
		main = this;
	}

	private void Start()
	{
		listDrops = new GameObject[max];
		drops.gameObject.SetActive(value: false);
	}

	public Blood Add(Vector2 position, float floor, float speed, int angle)
	{
		GameObject gameObject = null;
		if (current >= max)
		{
			current = 0;
		}
		if (listDrops[current] == null)
		{
			gameObject = UnityEngine.Object.Instantiate(drops.gameObject);
			gameObject.transform.SetParent(drops.gameObject.transform.parent);
		}
		else
		{
			gameObject = listDrops[current];
		}
		gameObject.SetActive(value: true);
		gameObject.transform.position = position;
		blood = gameObject.GetComponent<Blood>();
		blood.floor = floor;
		blood.enabled = true;
		blood.SetMove(speed, angle);
		listDrops[current] = gameObject;
		current++;
		return blood;
	}
}
