using System;
using UnityEngine;

[Serializable]
public struct WeaponSourceBullets
{
	[Tooltip("Кол-в во пуль при одном выстреле")]
	public int bullets;

	[Tooltip("Максмлн отклонение пули в градусах, для создания разброса")]
	[Range(0f, 180f)]
	public float angleScatter;

	[Tooltip("Равномерное распределение пуль")]
	[Range(0f, 1f)]
	public float evenlySpread;

	[Tooltip("Наносимый урон от всех пуль (будет распределен между всеми пулями)")]
	public float damage;

	[Tooltip("Импульс (будет распределен между всеми пулями)")]
	public float impulse;

	[Tooltip("Дистанция луча")]
	public float distance;

	[Tooltip("Параметры попадания для одной пули. Запись попадания пули, если оно проиcходит в разные игровые объекты, которые находятся в разных родительских объектах")]
	public HitsBullet hitsBullet;

	[Tooltip("Снижение наносимого урона после прохождения пули сквозь тела")]
	public PhysicsMaterialMultiply[] damageMultiply;

	[Space(10f)]
	[Tooltip("Тег игровых объектов существ, с которыми искать попадание пуль (необязательно)")]
	public string tagEntity;

	[Tooltip("Слои с физ.телами для столкновения луча RaycastHit2D")]
	public LayerMask layerBodies;

	public WeaponSourceBullets(int bullets, float damage, float impulse)
	{
		this.bullets = Mathf.Max(1, bullets);
		this.damage = damage;
		this.impulse = impulse;
		angleScatter = 0f;
		evenlySpread = 0f;
		hitsBullet = new HitsBullet(1, 1, 0f, 0f, HitsBullet.ModeHit.FIRST);
		damageMultiply = new PhysicsMaterialMultiply[0];
		distance = 0f;
		layerBodies = default(LayerMask);
		tagEntity = string.Empty;
	}
}
