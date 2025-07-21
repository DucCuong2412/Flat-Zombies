using UnityEngine;

[AddComponentMenu("Scripts/Area Damage/EventActivationArea")]
public class EventActivationArea : ComponentArea
{
	[Tooltip("Выполнить только один раз для одной облсти")]
	public bool oneArea;

	[Tooltip("Список функций, вызываемых при активации")]
	public AreaDamageEvent activation = new AreaDamageEvent();

	public override bool ShowIconGizmos(AreaDamage area)
	{
		return true;
	}

	public override void Activation(AreaDamage area)
	{
		activation.Invoke(area);
		if (oneArea)
		{
			UnityEngine.Object.Destroy(this);
		}
	}
}
