using System;
using UnityEngine;

[Serializable]
public struct PartBodyDamage
{
	[HideInInspector]
	public float coefficient;

	public Collider2D partBody;

	public float health;
}
