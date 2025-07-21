using System;
using UnityEngine;

[Serializable]
public struct PositionBloodSprite
{
	public string name;

	public Vector2 position;

	public bool preview;

	[HideInInspector]
	public Vector2 lastPosition;
}
