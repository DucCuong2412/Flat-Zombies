using UnityEngine;

public class BackgroundRandom : MonoBehaviour
{
	public static int selectedSkin = -1;

	public static int corridor = 1;

	public static string lastID = string.Empty;

	public string ID = string.Empty;

	public Sprite[] currentSprite = new Sprite[0];

	public SkinBackgroundRandom[] skins = new SkinBackgroundRandom[0];

	private SpriteRenderer[] renders = new SpriteRenderer[0];

	public static void ResetSkin()
	{
		selectedSkin = -1;
		corridor = 1;
		PlayerPrefsFile.DeleteKey(lastID + ".selectedSkin");
	}

	private void Start()
	{
		lastID = ID;
		selectedSkin = PlayerPrefsFile.GetInt(ID + ".selectedSkin", UnityEngine.Random.Range(0, skins.Length + 1));
		if (selectedSkin != skins.Length)
		{
			renders = base.gameObject.GetComponentsInChildren<SpriteRenderer>();
			for (int i = 0; i < renders.Length; i++)
			{
				for (int j = 0; j < currentSprite.Length; j++)
				{
					if (renders[i].sprite == currentSprite[j])
					{
						renders[i].sprite = skins[selectedSkin].sprites[j];
						break;
					}
				}
			}
		}
		PlayerPrefsFile.SetInt(ID + ".selectedSkin", selectedSkin);
		UnityEngine.Object.Destroy(this);
	}
}
