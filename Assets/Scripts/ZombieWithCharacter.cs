using System;
using UnityEngine;

[Serializable]
public struct ZombieWithCharacter
{
	public float priority;

	[HideInInspector]
	public float rangePriorityMin;

	[HideInInspector]
	public float rangePriorityMax;

	public ZombieInHome zombie;

	public ZombieCharacteristic characters;

	[HideInInspector]
	public float sizeLowerPriority;

	public void SetLowerPriority(ZombieWithCharacter[] zombies, int totalOnStage)
	{
		float num = 0f;
		for (int i = 0; i < zombies.Length; i++)
		{
			num += zombies[i].priority;
		}
		sizeLowerPriority = Mathf.Floor(num / (float)totalOnStage * 100f) / 100f;
	}

	public void LowerPriority()
	{
		priority = Mathf.Max(0f, priority - sizeLowerPriority);
	}
}
