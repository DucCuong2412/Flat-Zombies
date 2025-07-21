using System.Collections;
using UnityEngine;

[AddComponentMenu("Scripts/Area Damage/DestroyJoint")]
public class DestroyJoint : ComponentArea
{
	[Tooltip("Тело в соединении")]
	public Rigidbody2D body;

	[Tooltip("Тело для автоматического предварительного поиска соединений")]
	public Rigidbody2D connectedBody;

	[Tooltip("Болтовое соединение для удаления и для создания импульса, для разброса тел")]
	public HingeJoint2D joint;

	[Tooltip("Импульс для разброса тел из точки соединения в направлении центра массы тела")]
	public int impulse;

	[Tooltip("Добавить крутящий момент для двух указаных тел")]
	public float torque;

	[Tooltip("Список соединений для удаления между указаными телами")]
	public Joint2D[] listDestroy;

	[Header("Другие тела")]
	[Tooltip("Объект для получения списка соединений из его дочерних объектов")]
	public GameObject parentJoints;

	[Tooltip("Список объектов для удаления всех соединений с их участием")]
	public GameObject[] listBodies;

	private ArrayList preList;

	private Vector2 globalPosition = default(Vector2);

	private float angle;

	private Vector2 direction = default(Vector2);

	private GameObject prefab;

	public override bool ShowIconGizmos(AreaDamage area)
	{
		return true;
	}

	public override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		if (body == null && connectedBody == null && joint != null)
		{
			body = joint.gameObject.GetComponent<Rigidbody2D>();
			connectedBody = joint.connectedBody;
		}
		if (body == null)
		{
			body = base.gameObject.GetComponent<Rigidbody2D>();
		}
		if (!(connectedBody != null) || !(body != null) || listDestroy.Length != 0)
		{
			return;
		}
		joint = null;
		preList = new ArrayList();
		listDestroy = connectedBody.gameObject.GetComponents<Joint2D>();
		int num = 0;
		for (num = 0; num < listDestroy.Length; num++)
		{
			if ((bool)listDestroy[num].connectedBody && listDestroy[num].connectedBody.gameObject == body.gameObject)
			{
				preList.Add(listDestroy[num]);
			}
		}
		listDestroy = body.gameObject.GetComponents<Joint2D>();
		for (num = 0; num < listDestroy.Length; num++)
		{
			if ((bool)listDestroy[num].connectedBody && listDestroy[num].connectedBody == connectedBody)
			{
				preList.Add(listDestroy[num]);
			}
		}
		listDestroy = new Joint2D[preList.Count];
		for (num = 0; num < listDestroy.Length; num++)
		{
			listDestroy[num] = (Joint2D)preList[num];
			if (preList[num] is HingeJoint2D)
			{
				joint = (HingeJoint2D)preList[num];
			}
		}
	}

	public override void Activation(AreaDamage area)
	{
		if (!base.enabled)
		{
			return;
		}
		if ((bool)joint && impulse != 0)
		{
			body = joint.gameObject.GetComponent<Rigidbody2D>();
			body.isKinematic = false;
			globalPosition = joint.transform.TransformPoint(joint.anchor);
			ref Vector2 reference = ref direction;
			Vector2 worldCenterOfMass = body.worldCenterOfMass;
			reference.x = worldCenterOfMass.x - globalPosition.x;
			ref Vector2 reference2 = ref direction;
			Vector2 worldCenterOfMass2 = body.worldCenterOfMass;
			reference2.y = worldCenterOfMass2.y - globalPosition.y;
			direction.Normalize();
			direction *= (float)impulse;
			body.AddForceAtPosition(direction, globalPosition);
			body.AddTorque(torque);
			ref Vector2 reference3 = ref direction;
			Vector2 worldCenterOfMass3 = connectedBody.worldCenterOfMass;
			reference3.x = worldCenterOfMass3.x - globalPosition.x;
			ref Vector2 reference4 = ref direction;
			Vector2 worldCenterOfMass4 = connectedBody.worldCenterOfMass;
			reference4.y = worldCenterOfMass4.y - globalPosition.y;
			direction.Normalize();
			direction *= (float)impulse;
			connectedBody = joint.connectedBody;
			connectedBody.isKinematic = false;
			connectedBody.AddForceAtPosition(direction, globalPosition);
			connectedBody.AddTorque(0f - torque);
		}
		for (int i = 0; i < listDestroy.Length; i++)
		{
			UnityEngine.Object.Destroy(listDestroy[i]);
		}
		for (int j = 0; j < listBodies.Length; j++)
		{
			listDestroy = parentJoints.GetComponents<Joint2D>();
			for (int k = 0; k < listDestroy.Length; k++)
			{
				UnityEngine.Object.Destroy(listDestroy[k]);
			}
		}
		if (!(parentJoints != null))
		{
			return;
		}
		listDestroy = parentJoints.GetComponentsInChildren<Joint2D>();
		for (int l = 0; l < listBodies.Length; l++)
		{
			for (int m = 0; m < listDestroy.Length; m++)
			{
				if (listBodies[l] == listDestroy[m].connectedBody)
				{
					UnityEngine.Object.Destroy(listDestroy[m]);
				}
			}
		}
		listDestroy = parentJoints.GetComponents<Joint2D>();
		for (int n = 0; n < listBodies.Length; n++)
		{
			for (int num = 0; num < listDestroy.Length; num++)
			{
				if (listBodies[n] == listDestroy[num].connectedBody)
				{
					UnityEngine.Object.Destroy(listDestroy[num]);
				}
			}
		}
	}

	public void Activation()
	{
		Activation(null);
	}

	public void RemoveJoint(Joint2D joint)
	{
		UnityEngine.Object.Destroy(joint);
	}

	public void RemoveJoint(HingeJoint2D joint)
	{
		UnityEngine.Object.Destroy(joint);
	}

	public void RemoveJoint(SpringJoint2D joint)
	{
		UnityEngine.Object.Destroy(joint);
	}

	public void RemoveJoint(WheelJoint2D joint)
	{
		UnityEngine.Object.Destroy(joint);
	}
}
