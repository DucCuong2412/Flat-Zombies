using UnityEngine;

[AddComponentMenu("Scripts/Area Damage/RemoveBody")]
public class RemoveBody : ComponentArea
{
	public enum ObjectDestroy
	{
		Area,
		Current
	}

	[Tooltip("Какой объект уничтожать - объект с компонентом или объект с областью")]
	public ObjectDestroy objectDestroy;

	public override void Activation(AreaDamage area)
	{
		if (objectDestroy == ObjectDestroy.Area)
		{
			UnityEngine.Object.Destroy(area.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void Destroy(GameObject gameObj)
	{
		UnityEngine.Object.Destroy(gameObj);
	}
}
