using UnityEngine;

public class SpriteCorridor : MonoBehaviour
{
	public static int selectedSkin = -1;

	public Sprite[] skins = new Sprite[0];

	private void Start()
	{
		if (selectedSkin == -1)
		{
			selectedSkin = UnityEngine.Random.Range(0, 100);
		}
		GetComponent<SpriteRenderer>().sprite = skins[(selectedSkin + BackgroundRandom.selectedSkin) % skins.Length];
		UnityEngine.Object.Destroy(this);
	}
}
