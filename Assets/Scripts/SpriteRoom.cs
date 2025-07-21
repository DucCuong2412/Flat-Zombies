using UnityEngine;

public class SpriteRoom : MonoBehaviour
{
	public enum ModeSelectSprite
	{
		Loop,
		Corridor,
		Random,
		RandomScene,
		Once,
		LoopScene
	}

	public static int[] showed = new int[100];

	public static int corridor = -1;

	public static int idSpriteScene = -1;

	public static int addStepScene = 0;

	public static int randomScene = 0;

	public int rotateRandom;

	[Tooltip("Сдвиг объекта от стартовой позиции в обе стороны")]
	public float slideX;

	[Tooltip("Сдвиг объекта от стартовой позиции в обе стороны")]
	public float slideY;

	[Tooltip("Отражать случайно по горизонтали/вертикали")]
	public bool flipX;

	[Tooltip("Отражать случайно по горизонтали/вертикали")]
	public bool flipY;

	[Space(8f)]
	[Tooltip("Удалить объект вместе с дочерними объектами если для него не был указан спрайт")]
	public bool destroy;

	[Tooltip("Как будет происходить выбор спрайта из списка - случайно в каждой сцене или последовательно")]
	public ModeSelectSprite modeSelect;

	public int rangeRandom;

	public int preview;

	public Sprite[] sprites = new Sprite[0];

	public bool randomMixList;

	public bool showAllOnce;

	public Sprite[] onceSprites = new Sprite[0];

	private SpriteRenderer render;

	private int currentPreview;

	private Vector2 sizeSpriteSpace = default(Vector2);

	private Vector3 centerSprite = default(Vector2);

	private float spriteHeight;

	private float spriteWidth;

	private Vector3 upLeft = default(Vector3);

	private Vector3 upRight = default(Vector3);

	private Vector3 bottomLeft = default(Vector3);

	private Vector3 bottomRight = default(Vector3);

	public static Sprite[] MixListSprite(Sprite[] sprites)
	{
		for (int i = 0; i < sprites.Length; i++)
		{
			int num = UnityEngine.Random.Range(0, sprites.Length);
			Sprite sprite = sprites[i];
			sprites[i] = sprites[num];
			sprites[num] = sprite;
		}
		return sprites;
	}

	private void OnDrawGizmos()
	{
		if (randomMixList)
		{
			sprites = MixListSprite(sprites);
			randomMixList = false;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (render == null)
		{
			render = base.gameObject.GetComponent<SpriteRenderer>();
		}
		if (preview != currentPreview)
		{
			preview = Mathf.Clamp(preview, 0, sprites.Length - 1);
			currentPreview = preview;
			spriteHeight = 0f;
			spriteWidth = 0f;
			if (sprites.Length != 0 && sprites[preview] != null)
			{
				render.sprite = sprites[preview];
			}
		}
		if (render != null && render.sprite != null)
		{
			sizeSpriteSpace = RectSpriteSpace(render.sprite);
			ref Vector3 reference = ref centerSprite;
			float num = render.sprite.rect.width / 2f;
			Vector2 pivot = render.sprite.pivot;
			reference.x = num - pivot.x;
			ref Vector3 reference2 = ref centerSprite;
			float num2 = render.sprite.rect.height / 2f;
			Vector2 pivot2 = render.sprite.pivot;
			reference2.y = num2 - pivot2.y;
			centerSprite /= render.sprite.pixelsPerUnit;
		}
		Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
		Gizmos.DrawWireCube(base.transform.TransformPoint(centerSprite), new Vector3(sizeSpriteSpace.x, sizeSpriteSpace.y, 0f));
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(base.transform.TransformPoint(centerSprite), new Vector3(2f * slideX + sizeSpriteSpace.x, 2f * slideY + sizeSpriteSpace.y, 0f));
	}

	private Vector2 RectSpriteSpace(Sprite sprite)
	{
		upLeft = base.gameObject.transform.TransformPoint(sprite.bounds.min);
		upRight = base.gameObject.transform.TransformPoint(sprite.bounds.max);
		Transform transform = base.gameObject.transform;
		Vector3 min = sprite.bounds.min;
		float x = min.x;
		Vector3 max = sprite.bounds.max;
		bottomLeft = transform.TransformPoint(x, max.y, 0f);
		Transform transform2 = base.gameObject.transform;
		Vector3 max2 = sprite.bounds.max;
		float x2 = max2.x;
		Vector3 min2 = sprite.bounds.min;
		bottomRight = transform2.TransformPoint(x2, min2.y, 0f);
		spriteWidth = Mathf.Max(upLeft.x, upRight.x, bottomLeft.x, bottomRight.x);
		spriteWidth -= Mathf.Min(upLeft.x, upRight.x, bottomLeft.x, bottomRight.x);
		spriteHeight = Mathf.Max(upLeft.y, upRight.y, bottomLeft.y, bottomRight.y);
		spriteHeight -= Mathf.Min(upLeft.y, upRight.y, bottomLeft.y, bottomRight.y);
		return new Vector2(spriteWidth, spriteHeight);
	}

	private void Awake()
	{
		if (corridor == -1)
		{
			corridor = UnityEngine.Random.Range(0, 100);
		}
		if (idSpriteScene == -1)
		{
			idSpriteScene = UnityEngine.Random.Range(0, 100);
		}
		randomScene = UnityEngine.Random.Range(0, 200);
		idSpriteScene += addStepScene;
		addStepScene = 0;
	}

	private void Start()
	{
		int num = Mathf.Max(1, BackgroundRandom.selectedSkin);
		render = base.gameObject.GetComponent<SpriteRenderer>();
		if (sprites.Length == 0)
		{
			sprites = new Sprite[1];
			sprites[0] = render.sprite;
		}
		if (modeSelect == ModeSelectSprite.Loop)
		{
			render.sprite = sprites[num * BackgroundRandom.corridor % sprites.Length];
		}
		else if (modeSelect == ModeSelectSprite.Corridor)
		{
			render.sprite = sprites[(num * BackgroundRandom.corridor + corridor) % sprites.Length];
		}
		else if (modeSelect == ModeSelectSprite.Random)
		{
			rangeRandom = Mathf.Max(rangeRandom, sprites.Length);
			int num2 = UnityEngine.Random.Range(0, rangeRandom);
			render.sprite = null;
			if (num2 < sprites.Length)
			{
				render.sprite = sprites[num2];
			}
		}
		else if (modeSelect == ModeSelectSprite.RandomScene)
		{
			render.sprite = sprites[randomScene % sprites.Length];
		}
		else if (modeSelect == ModeSelectSprite.Once)
		{
			render.sprite = sprites[Mathf.Min(BackgroundRandom.corridor, sprites.Length - 1)];
		}
		else if (modeSelect == ModeSelectSprite.LoopScene)
		{
			render.sprite = sprites[idSpriteScene % sprites.Length];
		}
		if (flipX && UnityEngine.Random.value >= 0.5f)
		{
			render.flipX = !render.flipX;
		}
		if (flipY && UnityEngine.Random.value >= 0.5f)
		{
			render.flipY = !render.flipY;
		}
		int num3 = 0;
		num3 = 0;
		while (render.sprite != null && num3 < showed.Length && showed[num3] != 0)
		{
			if (render.sprite.GetHashCode() == showed[num3])
			{
				UnityEngine.Debug.LogWarning("Delete:" + render.sprite.name);
				render.sprite = null;
				num3 = -1;
				break;
			}
			num3++;
		}
		num3 = Mathf.Min(num3, showed.Length - 1);
		if (num3 == showed.Length - 1)
		{
			showed = new int[showed.Length];
			num3 = -1;
		}
		if (render.sprite != null && showAllOnce && num3 != -1)
		{
			UnityEngine.Debug.Log("Add:" + render.sprite.name);
			showed[num3] = render.sprite.GetHashCode();
		}
		else if (render.sprite != null && num3 != -1)
		{
			for (int i = 0; i < onceSprites.Length; i++)
			{
				if (render.sprite.GetHashCode() == onceSprites[i].GetHashCode())
				{
					UnityEngine.Debug.Log("Add:" + render.sprite.name);
					showed[num3] = render.sprite.GetHashCode();
				}
			}
		}
		if (render.sprite == null && (destroy || base.transform.childCount == 0))
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (render.sprite == null)
		{
			UnityEngine.Object.Destroy(render);
			UnityEngine.Object.Destroy(this);
			return;
		}
		Vector3 position = base.transform.position;
		slideX = Mathf.Floor(UnityEngine.Random.Range(0f - slideX, slideX) * 100f) / 100f;
		position.x += slideX;
		slideY = Mathf.Floor(UnityEngine.Random.Range(0f - slideY, slideY) * 100f) / 100f;
		position.y += slideY;
		rotateRandom = UnityEngine.Random.Range(-rotateRandom, rotateRandom + 1);
		base.transform.Rotate(0f, 0f, rotateRandom);
		base.transform.position = position;
		UnityEngine.Object.Destroy(this);
	}

	private void OnDestroy()
	{
		addStepScene = 1;
	}
}
