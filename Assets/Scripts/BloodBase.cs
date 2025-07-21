using UnityEngine;

public class BloodBase : MonoBehaviour
{
	public virtual void OnGetDrops()
	{
	}

	public virtual void SetMove(float speed, float angle)
	{
	}

	public virtual void SetFloor(float value)
	{
	}

	public virtual void SetSpeedBody(Vector2 speed)
	{
	}

	public void SetScale(float scaleX, float scaleY)
	{
		base.transform.localScale = new Vector3(scaleX, scaleY);
	}

	public virtual BloodBase[] GetListObjects()
	{
		return new BloodBase[0];
	}
}
