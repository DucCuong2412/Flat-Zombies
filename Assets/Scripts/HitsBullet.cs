using System;
using UnityEngine;

[Serializable]
public struct HitsBullet
{
	public enum ModeHit
	{
		FIRST,
		RANDOM_ONCE,
		RANDOM_FIRST
	}

	[Tooltip("FIRST - Фиксировать несолько попаданий одной пули, начиная с самого первого попадания;\nRANDOM_ONCE - Зафиксировать случайно только одно попадание при попадании в нескольких точках. При этом попадание может быть просто утеряно;\nRANDOM_FIRST - Зафиксировать случайно одно попадание. Выбрать первое попадание, если неудалось подобрать попадание случайным образом;")]
	public ModeHit modeHit;

	[Tooltip("Сколько минимум попаданий фиксировать для одной пули")]
	public int numsMin;

	[Tooltip("Сколько максимум попаданий фиксировать для одной пули")]
	public int numsMax;

	[Tooltip("Радиус, в пределах которого фиксировать попадания")]
	public float distHitsMin;

	public float distHitsMax;

	public HitsBullet(int numsMin, int numsMax, float distHitsMin, float distHitsMax, ModeHit modeHit)
	{
		this.distHitsMin = Mathf.Abs(distHitsMin);
		this.distHitsMax = Mathf.Abs(distHitsMax);
		this.numsMin = Mathf.Max(1, numsMin);
		this.numsMax = Mathf.Max(1, numsMax);
		this.modeHit = modeHit;
	}

	public int GetRandomNumHits()
	{
		numsMin = Mathf.Max(1, numsMin);
		numsMax = Mathf.Max(1, numsMax);
		return UnityEngine.Random.Range(numsMin, numsMax + 1);
	}

	public bool IsHitRangeDist(float distFirstHit, float distCurrentHit)
	{
		distCurrentHit -= distFirstHit;
		return distHitsMin <= distCurrentHit && distCurrentHit <= distHitsMax;
	}
}
