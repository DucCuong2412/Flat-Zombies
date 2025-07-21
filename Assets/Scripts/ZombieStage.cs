using UnityEngine;

public class ZombieStage : MonoBehaviour
{
	public enum Operation
	{
		save,
		read
	}

	public static ZombieDetails details = default(ZombieDetails);

	public Operation action;

	public SpriteRenderer[] renders;

	private void Awake()
	{
		if (action == Operation.read && details.sprites != null)
		{
			for (int i = 0; i < renders.Length; i++)
			{
				renders[i].sprite = details.sprites[i];
				renders[i].color = details.colors[i];
				renders[i].enabled = details.enabled[i];
			}
		}
	}

	public void Save()
	{
		details.sprites = new Sprite[renders.Length];
		details.colors = new Color[renders.Length];
		details.enabled = new bool[renders.Length];
		for (int i = 0; i < renders.Length; i++)
		{
			details.sprites[i] = renders[i].sprite;
			details.colors[i] = renders[i].color;
			details.enabled[i] = (renders[i].enabled && renders[i].gameObject.activeInHierarchy);
		}
	}
}
