using UnityEngine;

public class ComponentArea : MonoBehaviour
{
	public static AreaDamage[] areas = new AreaDamage[0];

	[HideInInspector]
	public AreaDamage area;

	[HideInInspector]
	public string eventArea = string.Empty;

	[HideInInspector]
	public string[] listEvents = new string[0];

	public AreaDamage GetConnectedArea()
	{
		areas = area.gameObject.GetComponents<AreaDamage>();
		for (int i = 0; i < areas.Length; i++)
		{
			if (areas[i].IsConnected(this))
			{
				return areas[i];
			}
		}
		areas = new AreaDamage[0];
		return null;
	}

	public virtual void OnDrawGizmosSelected()
	{
	}

	public virtual void OnInspectorGUI()
	{
	}

	private void Start()
	{
	}

	public virtual void InitArea(AreaDamage area)
	{
	}

	public virtual void Activation(AreaDamage area)
	{
	}

	public virtual bool ShowIconGizmos(AreaDamage area)
	{
		return false;
	}

	public virtual void GizmosArea(AreaDamage area)
	{
	}
}
