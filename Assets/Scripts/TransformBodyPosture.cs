using System;
using UnityEngine;

[Serializable]
public struct TransformBodyPosture
{
	public string name;

	public Vector3 transform;

	public void Record(Transform transformBody)
	{
		name = transformBody.name;
		transform = transformBody.localPosition;
		ref Vector3 reference = ref transform;
		Vector3 eulerAngles = transformBody.localRotation.eulerAngles;
		reference.z = Mathf.Floor(100f * eulerAngles.z) / 100f;
	}
}
