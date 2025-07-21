using System;
using UnityEngine;

[Serializable]
public struct SourceWeaponShell
{
	public WeaponShell original;

	public int numCreate;

	public Sprite frame;

	public Vector2 position;

	public int impulse;

	[Range(0f, 360f)]
	public int angle;

	[Range(0f, 180f)]
	public int angleScatter;

	[Range(-180f, 180f)]
	[Tooltip("Угол наклона гильзы")]
	public int angleRotation;

	public Sprite skin;

	[Tooltip("Сколько гильз выбросить одновеременно")]
	public int quantity;

	private WeaponShell[] shells;

	[HideInInspector]
	private int current;

	public SourceWeaponShell(int current)
	{
		original = null;
		numCreate = 0;
		frame = null;
		position = Vector2.zero;
		impulse = 0;
		angle = 0;
		angleScatter = 0;
		angleRotation = 0;
		skin = null;
		quantity = 1;
		shells = new WeaponShell[0];
		this.current = 0;
	}

	public void Awake(string nameWeapon)
	{
		if (!(original == null) && numCreate > 0)
		{
			shells = new WeaponShell[numCreate];
			for (int i = 0; i < shells.Length; i++)
			{
				shells[i] = UnityEngine.Object.Instantiate(original);
				shells[i].name = nameWeapon + "." + shells[i].name;
			}
		}
	}

	public void Reset(Transform weapon)
	{
		if (shells.Length != 0)
		{
			quantity = Mathf.Max(1, quantity);
			for (int i = 1; i <= quantity; i++)
			{
				current++;
				current = Mathf.FloorToInt(Mathf.Repeat(current, shells.Length));
				Vector3 eulerAngles = weapon.rotation.eulerAngles;
				float z = eulerAngles.z;
				float angleMove = z + (float)angle + (float)UnityEngine.Random.Range(0, angleScatter);
				int num = Mathf.FloorToInt(z) + angleRotation;
				shells[current].Reset(weapon.TransformPoint(position.x, position.y, 0f), skin, angleMove, impulse, num);
			}
		}
	}
}
