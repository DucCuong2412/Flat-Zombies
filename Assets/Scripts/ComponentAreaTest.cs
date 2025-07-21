using UnityEngine;

[ExecuteInEditMode]
public class ComponentAreaTest : MonoBehaviour
{
	public AreaDamage objectAreas;

	public Vector2 position;

	[Tooltip("Имя области или группа, к которой относится область. Если не указан - компонент относится ко всем областям")]
	public string areaNameOrGroup = string.Empty;

	private AreaDamage[] listAreas;

	private void Awake()
	{
		if (base.enabled)
		{
			AddListenerInArea();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (objectAreas != null)
		{
			position = objectAreas.GetWorldPosition();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!base.enabled || !(objectAreas != null))
		{
			return;
		}
		listAreas = objectAreas.GetComponents<AreaDamage>();
		for (int i = 0; i < listAreas.Length; i++)
		{
			if (areaNameOrGroup == string.Empty || listAreas[i].name == areaNameOrGroup || listAreas[i].EventExists(areaNameOrGroup))
			{
				Transform transform = listAreas[i].gameObject.transform;
				float x = listAreas[i].position.x;
				float y = listAreas[i].position.y;
				Vector3 vector = listAreas[i].gameObject.transform.position;
				Vector3 center = transform.TransformPoint(new Vector3(x, y, vector.z));
				Gizmos.DrawIcon(center, GetIconGizmos(listAreas[i]), allowScaling: false);
			}
		}
		listAreas = objectAreas.GetComponentsInChildren<AreaDamage>();
		for (int j = 0; j < listAreas.Length; j++)
		{
			if (areaNameOrGroup == string.Empty || listAreas[j].name == areaNameOrGroup || listAreas[j].EventExists(areaNameOrGroup))
			{
				Transform transform2 = listAreas[j].gameObject.transform;
				float x2 = listAreas[j].position.x;
				float y2 = listAreas[j].position.y;
				Vector3 vector2 = listAreas[j].gameObject.transform.position;
				Vector3 center2 = transform2.TransformPoint(new Vector3(x2, y2, vector2.z));
				Gizmos.DrawIcon(center2, GetIconGizmos(listAreas[j]), allowScaling: false);
			}
		}
	}

	public void AddListenerInArea()
	{
		if (!(objectAreas != null))
		{
			return;
		}
		listAreas = objectAreas.GetComponents<AreaDamage>();
		for (int i = 0; i < listAreas.Length; i++)
		{
			if (string.IsNullOrEmpty(areaNameOrGroup) || listAreas[i].name == areaNameOrGroup || listAreas[i].EventExists(areaNameOrGroup))
			{
				listAreas[i].activation.AddListener(Activation);
			}
		}
		listAreas = null;
	}

	public virtual string GetIconGizmos(AreaDamage area)
	{
		return string.Empty;
	}

	public virtual void Activation(AreaDamage area)
	{
		UnityEngine.Debug.Log("ComponentAreaTest:" + area.ToString());
	}
}
