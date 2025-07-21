using UnityEngine;

[AddComponentMenu("Scripts/Area Damage/ImpulsePhysicBody")]
public class ImpulsePhysicBody : ComponentArea
{
	public Rigidbody2D body;

	[Tooltip("Сила импульса от области в направлении центра тяжести тела")]
	public int impulse;

	[Tooltip("Добавить крутящий момент")]
	public float torque;

	public override void Activation(AreaDamage area)
	{
		if (body != null)
		{
			Vector2 worldPosition = area.GetWorldPosition();
			Vector2 worldCenterOfMass = body.worldCenterOfMass;
			float y = worldCenterOfMass.y - worldPosition.y;
			Vector2 worldCenterOfMass2 = body.worldCenterOfMass;
			float f = Mathf.Atan2(y, worldCenterOfMass2.x - worldPosition.x);
			Vector2 force = default(Vector2);
			force.x = Mathf.Cos(f) * (float)impulse;
			force.y = Mathf.Sin(f) * (float)impulse;
			body.AddForceAtPosition(force, worldPosition);
			body.AddTorque(torque);
		}
	}
}
