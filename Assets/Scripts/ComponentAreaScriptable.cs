using UnityEngine;

public class ComponentAreaScriptable : ScriptableObject
{
	[HideInInspector]
	public string eventArea = string.Empty;

	public virtual void OnStartArea(AreaDamage area)
	{
	}

	public virtual void OnActivation(AreaDamage area)
	{
	}

	public virtual void OnGizmosArea(AreaDamage area)
	{
	}
}
