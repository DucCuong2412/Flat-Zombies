using System;
using UnityEngine;

[Serializable]
public struct PhysicsMaterialMultiply
{
	[Tooltip("На сколько процентов будет снижена сила повреждения")]
	public float multiply;

	[Tooltip("Материал физ.фигуры")]
	public PhysicsMaterial2D material;

	[Tooltip("Остановить пулю")]
	public bool stopBullet;

	public static float GetMultiplyPower(PhysicsMaterial2D materialHit, PhysicsMaterialMultiply[] list, int numHitBullet, float defaultValue)
	{
		for (int i = 0; i < list.Length; i++)
		{
			if (list[i].material.name == materialHit.name && (numHitBullet != 0 || list[i].stopBullet))
			{
				numHitBullet = Mathf.Max(1, numHitBullet);
				return list[i].multiply / (float)numHitBullet;
			}
		}
		return defaultValue;
	}

	public static float GetMultiplyPower(PhysicsMaterial2D materialHit, PhysicsMaterialMultiply[] list, float defaultValue)
	{
		return GetMultiplyPower(materialHit, list, 1, defaultValue);
	}

	public static bool StopBullet(PhysicsMaterial2D materialHit, PhysicsMaterialMultiply[] list)
	{
		for (int i = 0; i < list.Length; i++)
		{
			if (list[i].material.name == materialHit.name)
			{
				return list[i].stopBullet;
			}
		}
		return false;
	}
}
