using UnityEngine;

public class DeadZombieBodyPosture : MonoBehaviour
{
	public enum Action
	{
		None,
		Record,
		Update,
		Preview
	}

	public Action action;

	public TransformBodyPosture[] positions = new TransformBodyPosture[0];

	private Transform[] childrens;

	[HideInInspector]
	public bool selectRandom = true;

	private DeadZombieBodyPosture[] neighbors = new DeadZombieBodyPosture[0];

	private void OnDrawGizmosSelected()
	{
		if (action == Action.Record)
		{
			action = Action.None;
			childrens = base.gameObject.GetComponentsInChildren<Transform>();
			positions = new TransformBodyPosture[childrens.Length];
			for (int i = 0; i < childrens.Length; i++)
			{
				positions[i].Record(childrens[i]);
			}
		}
		else if (action == Action.Preview)
		{
			action = Action.None;
			TransformBody();
		}
		else
		{
			if (action != Action.Update)
			{
				return;
			}
			action = Action.None;
			childrens = base.gameObject.GetComponentsInChildren<Transform>();
			for (int j = 0; j < positions.Length; j++)
			{
				for (int k = 0; k < childrens.Length; k++)
				{
					if (positions[j].name == childrens[k].name)
					{
						positions[j].Record(childrens[k]);
						break;
					}
				}
			}
		}
	}

	private void Start()
	{
		if (selectRandom)
		{
			neighbors = base.gameObject.GetComponents<DeadZombieBodyPosture>();
			int num = UnityEngine.Random.Range(0, neighbors.Length);
			neighbors[num].TransformBody();
			for (int i = 0; i < neighbors.Length; i++)
			{
				UnityEngine.Object.Destroy(neighbors[i]);
			}
			neighbors = null;
		}
	}

	private void TransformBody()
	{
		childrens = base.gameObject.GetComponentsInChildren<Transform>();
		for (int i = 0; i < childrens.Length; i++)
		{
			for (int j = 0; j < positions.Length; j++)
			{
				if (childrens[i].name == positions[j].name)
				{
					Transform obj = childrens[i];
					float x = positions[j].transform.x;
					float y = positions[j].transform.y;
					Vector3 localPosition = childrens[i].localPosition;
					obj.localPosition = new Vector3(x, y, localPosition.z);
					childrens[i].localRotation = Quaternion.Euler(0f, 0f, positions[j].transform.z);
					break;
				}
			}
		}
	}
}
