using UnityEngine;

public class SpliceBody : ComponentArea
{
	public const string UP = "spliceUp";

	public const string DOWN = "spliceDown";

	public Rigidbody2D body;

	public Transform parentRenders;

	public float lineSplice;

	public float impulse;

	public float angleImpulse;

	public float massBody1 = 1f;

	public Rigidbody2D[] jointsBody1 = new Rigidbody2D[0];

	public float massBody2 = 1f;

	public Rigidbody2D[] jointsBody2 = new Rigidbody2D[0];

	public AreaDamage bodyArea;

	public string[] areaDisabled = new string[0];

	public Sprite blood;

	public Sprite hole;

	public string groupDamage = string.Empty;

	public SpriteRenderer[] renders = new SpriteRenderer[0];

	public Transform parent;

	[HideInInspector]
	public Rigidbody2D newBody;

	[HideInInspector]
	public SpriteRenderer newRenderBody;

	private Collider2D[] colliders2D = new Collider2D[0];

	private Joint2D[] joints2D;

	private MonoBehaviour[] compDestroy = new MonoBehaviour[0];

	private SpriteRenderer[] rendersCreated = new SpriteRenderer[0];

	private ObjectDamage damageSprite;

	private Sprite copy;

	private Texture2D tex2D;

	private BoxCollider2D newBoxCollider;

	private float sizeY;

	private float offsetY;

	public override void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(base.transform.TransformPoint(-0.8f, lineSplice, 0f), base.transform.TransformPoint(0.6f, lineSplice, 0f));
	}

	public override void OnInspectorGUI()
	{
		if (bodyArea != null && areaDisabled.Length == 0)
		{
			ComponentArea.areas = bodyArea.GetComponents<AreaDamage>();
			areaDisabled = new string[ComponentArea.areas.Length];
			for (int i = 0; i < ComponentArea.areas.Length; i++)
			{
				areaDisabled[i] = ComponentArea.areas[i].areaName;
			}
		}
	}

	public override void Activation(AreaDamage area)
	{
		int num = 0;
		int num2 = 0;
		newBody = Object.Instantiate(body.gameObject, body.transform.parent).GetComponent<Rigidbody2D>();
		newBody.name = body.name + "Splice";
		newBody.transform.position = body.transform.position;
		newBody.transform.rotation = body.transform.rotation;
		newBody.transform.localScale = body.transform.localScale;
		newRenderBody = newBody.GetComponent<SpriteRenderer>();
		colliders2D = newBody.GetComponents<Collider2D>();
		for (num = 0; num < colliders2D.Length; num++)
		{
			if (colliders2D[num] is BoxCollider2D)
			{
				SpliceDownBoxCollider(colliders2D[num] as BoxCollider2D);
				continue;
			}
			Vector2 offset = colliders2D[num].offset;
			if (offset.y > lineSplice && (colliders2D[num] is CircleCollider2D || colliders2D[num] is PolygonCollider2D))
			{
				UnityEngine.Object.Destroy(colliders2D[num]);
			}
		}
		colliders2D = body.GetComponents<Collider2D>();
		for (num = 0; num < colliders2D.Length; num++)
		{
			if (colliders2D[num] is BoxCollider2D)
			{
				SpliceUpBoxCollider(colliders2D[num] as BoxCollider2D);
				continue;
			}
			Vector2 offset2 = colliders2D[num].offset;
			if (offset2.y < lineSplice && (colliders2D[num] is CircleCollider2D || colliders2D[num] is PolygonCollider2D))
			{
				UnityEngine.Object.Destroy(colliders2D[num]);
			}
		}
		joints2D = newBody.GetComponents<Joint2D>();
		for (num2 = 0; num2 < jointsBody1.Length; num2++)
		{
			for (num = 0; num < joints2D.Length; num++)
			{
				if (joints2D[num].connectedBody == jointsBody1[num2])
				{
					UnityEngine.Object.Destroy(joints2D[num]);
				}
			}
		}
		joints2D = body.GetComponents<Joint2D>();
		for (num2 = 0; num2 < jointsBody2.Length; num2++)
		{
			for (num = 0; num < joints2D.Length; num++)
			{
				if (joints2D[num].connectedBody == jointsBody2[num2])
				{
					UnityEngine.Object.Destroy(joints2D[num]);
				}
			}
		}
		newBody.mass = massBody2;
		newBody.simulated = true;
		newBody.AddForce(new Vector2(0f, -0.2f * impulse), ForceMode2D.Impulse);
		newBody.AddTorque(0f - angleImpulse);
		body.mass = massBody1;
		body.simulated = true;
		body.AddForce(new Vector2(0f, impulse), ForceMode2D.Impulse);
		body.AddTorque(angleImpulse);
		compDestroy = newBody.GetComponents<MonoBehaviour>();
		for (num2 = 0; num2 < compDestroy.Length; num2++)
		{
			UnityEngine.Object.Destroy(compDestroy[num2]);
		}
		if (bodyArea != null)
		{
			for (num = 0; num < areaDisabled.Length; num++)
			{
				bodyArea.Disable(areaDisabled[num]);
			}
		}
		rendersCreated = new SpriteRenderer[renders.Length];
		for (num2 = 0; num2 < renders.Length; num2++)
		{
			rendersCreated[num2] = newRenderBody;
			if (newRenderBody != renders[num2])
			{
				rendersCreated[num2] = CreateCopy(renders[num2], parent);
			}
			rendersCreated[num2].sprite = CreateDown(rendersCreated[num2].sprite, rendersCreated[num2].transform, lineSplice, parentRenders);
			if (rendersCreated[num2].material.HasProperty("_WidthMainTex"))
			{
				rendersCreated[num2].material.SetInt("_WidthMainTex", rendersCreated[num2].sprite.texture.width);
				rendersCreated[num2].material.SetInt("_HeightMainTex", rendersCreated[num2].sprite.texture.height);
			}
		}
		for (num2 = 0; num2 < renders.Length; num2++)
		{
			renders[num2].sprite = CreateUp(renders[num2].sprite, renders[num2].transform, lineSplice, parentRenders);
			if (renders[num2].material.HasProperty("_WidthMainTex"))
			{
				renders[num2].material.SetInt("_WidthMainTex", renders[num2].sprite.texture.width);
				renders[num2].material.SetInt("_HeightMainTex", renders[num2].sprite.texture.height);
			}
		}
		for (num = 0; num < renders.Length; num++)
		{
			for (num2 = 0; num2 < renders.Length; num2++)
			{
				if (num != num2 && renders[num].transform == renders[num2].transform.parent)
				{
					rendersCreated[num2].transform.SetParent(rendersCreated[num].transform);
				}
			}
		}
		for (num = 0; num < rendersCreated.Length; num++)
		{
			if (rendersCreated[num].transform.parent == parent)
			{
				newRenderBody = rendersCreated[num];
				break;
			}
		}
		for (num2 = 0; num2 < renders.Length; num2++)
		{
			colliders2D = renders[num2].gameObject.GetComponents<Collider2D>();
			for (num = 0; num < colliders2D.Length; num++)
			{
				if (colliders2D[num].isTrigger && colliders2D[num] is BoxCollider2D)
				{
					SpliceBoxCollider(colliders2D[num] as BoxCollider2D, rendersCreated[num2].gameObject);
					colliders2D[num] = null;
				}
			}
		}
		UnityEngine.Object.Destroy(this);
	}

	private Sprite CreateUp(Sprite current, Transform objectRender, float lineSplice, Transform objectLine)
	{
		copy = SpriteWithDamage.GetSprite(current, "spliceUp", groupDamage);
		if (copy != current)
		{
			return copy;
		}
		Vector3 position = new Vector3(0f, lineSplice, 0f);
		position = objectLine.TransformPoint(position);
		position = objectRender.InverseTransformPoint(position);
		position = SpriteWithDamage.PositionToPixels(position, current);
		tex2D = new Texture2D(current.texture.width, Mathf.CeilToInt(position.y), current.texture.format, SpriteWithDamage.generateMipMaps && current.texture.mipmapCount != 0);
		tex2D.wrapMode = current.texture.wrapMode;
		tex2D.filterMode = current.texture.filterMode;
		tex2D.anisoLevel = current.texture.anisoLevel;
		tex2D.name = SpriteWithDamage.numCopySprite.ToString() + "." + current.name;
		tex2D.SetPixels32(CutColorsUp(current.texture.GetPixels32(), tex2D.width, tex2D.height));
		SpriteWithDamage.AddBlood(tex2D, blood.texture, tex2D.width / 2, tex2D.height, Random.Range(0, 15) * 20);
		SpriteWithDamage.AddHoleTexture(tex2D, hole.texture, tex2D.width / 2, tex2D.height, 0);
		tex2D.Apply();
		Rect rect = new Rect(0f, 0f, tex2D.width, tex2D.height);
		Vector2 pivot = default(Vector2);
		Vector2 pivot2 = current.pivot;
		pivot.x = pivot2.x / (float)tex2D.width;
		float num = current.texture.height;
		Vector2 pivot3 = current.pivot;
		pivot.y = 1f - (num - pivot3.y) / (float)tex2D.height;
		copy = Sprite.Create(tex2D, rect, pivot, current.pixelsPerUnit, 0u, SpriteMeshType.FullRect, current.border);
		copy.name = SpriteWithDamage.numCopySprite.ToString() + "." + current.name;
		CacheSprite(current, copy, "spliceUp");
		return copy;
	}

	private Sprite CreateDown(Sprite current, Transform objectRender, float lineSplice, Transform objectLine)
	{
		copy = SpriteWithDamage.GetSprite(current, "spliceDown", groupDamage);
		if (copy != current)
		{
			return copy;
		}
		Vector3 position = new Vector3(0f, lineSplice, 0f);
		position = objectLine.TransformPoint(position);
		position = objectRender.InverseTransformPoint(position);
		position = SpriteWithDamage.PositionToPixels(position, current);
		tex2D = new Texture2D(current.texture.width, current.texture.height - Mathf.CeilToInt(position.y), current.texture.format, SpriteWithDamage.generateMipMaps && current.texture.mipmapCount != 0);
		tex2D.wrapMode = current.texture.wrapMode;
		tex2D.filterMode = current.texture.filterMode;
		tex2D.anisoLevel = current.texture.anisoLevel;
		tex2D.name = SpriteWithDamage.numCopySprite.ToString() + "." + current.name;
		tex2D.SetPixels32(CutColorsDown(current.texture.GetPixels32(), tex2D.width, tex2D.height));
		SpriteWithDamage.AddBlood(tex2D, blood.texture, tex2D.width / 2, 0f, Random.Range(0, 15) * 20);
		SpriteWithDamage.AddHoleTexture(tex2D, hole.texture, tex2D.width / 2, 0f, 0);
		tex2D.Apply();
		Rect rect = new Rect(0f, 0f, tex2D.width, tex2D.height);
		Vector2 pivot = default(Vector2);
		Vector2 pivot2 = current.pivot;
		pivot.x = pivot2.x / (float)current.texture.width;
		Vector2 pivot3 = current.pivot;
		pivot.y = pivot3.y / (float)current.texture.height * (float)(current.texture.height / tex2D.height);
		copy = Sprite.Create(tex2D, rect, pivot, current.pixelsPerUnit, 0u, SpriteMeshType.FullRect, current.border);
		copy.name = SpriteWithDamage.numCopySprite.ToString() + "." + current.name;
		CacheSprite(current, copy, "spliceDown");
		return copy;
	}

	private SpriteRenderer CreateCopy(SpriteRenderer orignal, Transform parent)
	{
		GameObject gameObject = new GameObject(orignal.name + "Splice");
		gameObject.transform.SetParent(parent);
		gameObject.transform.position = orignal.transform.position;
		gameObject.transform.rotation = orignal.transform.rotation;
		gameObject.transform.localScale = orignal.transform.localScale;
		SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = orignal.sprite;
		spriteRenderer.color = orignal.color;
		spriteRenderer.flipX = orignal.flipX;
		spriteRenderer.flipY = orignal.flipY;
		spriteRenderer.material = orignal.material;
		spriteRenderer.drawMode = orignal.drawMode;
		spriteRenderer.sortingLayerName = orignal.sortingLayerName;
		spriteRenderer.sortingOrder = orignal.sortingOrder;
		spriteRenderer.maskInteraction = orignal.maskInteraction;
		return spriteRenderer;
	}

	private void CacheSprite(Sprite current, Sprite sprite, string idDamage)
	{
		damageSprite = SpriteWithDamage.GetDamage(current);
		if (damageSprite != null)
		{
			SpriteWithDamage.AddDamage(new ObjectDamage(damageSprite.source, sprite, damageSprite.group, idDamage, damageSprite.listID));
		}
		else
		{
			SpriteWithDamage.AddDamage(new ObjectDamage(SpriteWithDamage.GetSourceSprite(current), sprite, groupDamage, idDamage));
		}
	}

	private Color32[] CutColorsUp(Color32[] colors, int width, int height)
	{
		Color32[] array = new Color32[width * height];
		int num = (colors.Length / width - height) * width;
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = colors[i + num];
		}
		return array;
	}

	private Color32[] CutColorsDown(Color32[] colors, int width, int height)
	{
		Color32[] array = new Color32[width * height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = colors[i];
		}
		return array;
	}

	private void SpliceBoxCollider(BoxCollider2D box, GameObject newObject)
	{
		if (LineIsSpliceBoxCollider(box, lineSplice))
		{
			newBoxCollider = newObject.AddComponent<BoxCollider2D>();
			newBoxCollider.enabled = box.enabled;
			newBoxCollider.sharedMaterial = box.sharedMaterial;
			newBoxCollider.isTrigger = box.isTrigger;
			newBoxCollider.usedByEffector = box.usedByEffector;
			newBoxCollider.usedByComposite = box.usedByComposite;
			newBoxCollider.edgeRadius = box.edgeRadius;
			SpliceUpBoxCollider(box);
			SpliceDownBoxCollider(newBoxCollider);
		}
		box = null;
		newBoxCollider = null;
	}

	private void SpliceUpBoxCollider(BoxCollider2D box)
	{
		Vector2 size = box.size;
		float num = size.y / 2f;
		Vector2 offset = box.offset;
		sizeY = num + (offset.y - lineSplice);
		if (sizeY > 0f)
		{
			offsetY = lineSplice + sizeY / 2f;
			Vector2 size2 = box.size;
			box.size = new Vector2(size2.x, sizeY);
			Vector2 offset2 = box.offset;
			box.offset = new Vector2(offset2.x, offsetY);
		}
	}

	private void SpliceDownBoxCollider(BoxCollider2D box)
	{
		Vector2 size = box.size;
		float num = size.y / 2f;
		float num2 = lineSplice;
		Vector2 offset = box.offset;
		sizeY = num + (num2 - offset.y);
		if (sizeY > 0f)
		{
			offsetY = lineSplice - sizeY / 2f;
			Vector2 size2 = box.size;
			box.size = new Vector2(size2.x, sizeY);
			Vector2 offset2 = box.offset;
			box.offset = new Vector2(offset2.x, offsetY);
		}
	}

	private bool LineIsSpliceBoxCollider(BoxCollider2D box, float lineSplice)
	{
		Vector2 offset = box.offset;
		float num = Mathf.Abs(lineSplice - offset.y);
		Vector2 size = box.size;
		return num < size.y / 2f;
	}

	private void CopyCollider(CircleCollider2D original, CircleCollider2D newCopy)
	{
	}

	private void CopyCollider(BoxCollider2D original, CircleCollider2D BoxCollider2D)
	{
	}

	private void CopyRigidbody(Rigidbody2D original, Rigidbody2D BoxCollider2D)
	{
	}
}
