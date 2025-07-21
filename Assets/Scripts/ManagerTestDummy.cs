using UnityEngine;

public class ManagerTestDummy : MonoBehaviour
{
	[Tooltip("Задержка перед активацией")]
	public float timeStart;

	public float speed;

	public float lineStop;

	public Entity[] entities;

	public Entity dummyOnStage;

	private void OnDrawGizmos()
	{
		if (base.enabled)
		{
			Gizmos.color = new Color(1f, 0.5f, 0f);
			Gizmos.DrawLine(new Vector3(lineStop, -10f, 0f), new Vector3(lineStop, 10f, 0f));
		}
	}

	private void Start()
	{
		if (dummyOnStage == null)
		{
			Invoke("Create", timeStart);
		}
	}

	public void Create()
	{
		dummyOnStage = UnityEngine.Object.Instantiate(entities[Random.Range(0, entities.Length)]);
		dummyOnStage.transform.position = base.transform.position;
	}

	private void Update()
	{
		if (!(dummyOnStage != null))
		{
			return;
		}
		if (dummyOnStage.isDead)
		{
			Create();
			return;
		}
		float num = lineStop;
		Vector3 position = dummyOnStage.transform.position;
		if (num < position.x && speed != 0f)
		{
			dummyOnStage.transform.Translate(Mathf.Floor(1000f * speed * Time.deltaTime) * 0.001f, 0f, 0f);
		}
	}
}
