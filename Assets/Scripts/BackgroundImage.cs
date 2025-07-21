using UnityEngine;

public class BackgroundImage : MonoBehaviour
{
	public float angleMax;

	public float scaleMax;

	public float timeStep = 0.30769f;

	private RectTransform rectTrans;

	private float randValue;

	private float scale;

	private void Start()
	{
		rectTrans = base.gameObject.GetComponent<RectTransform>();
		InvokeRepeating("UpdateTransform", timeStep, timeStep);
	}

	public void UpdateTransform()
	{
		randValue = UnityEngine.Random.Range(0f, 1000f) / 1000f;
		rectTrans.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f - angleMax, angleMax, randValue));
		scale = Mathf.Lerp(1f, scaleMax, Mathf.Abs(0.5f - randValue) * 2f);
		rectTrans.localScale = new Vector3(scale, scale, 1f);
	}
}
