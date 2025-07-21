using System;
using UnityEngine;

[Serializable]
public struct SoundHitBullet
{
	[Tooltip("Материал физ.фигуры")]
	public PhysicsMaterial2D material;

	public AudioClip sound;
}
