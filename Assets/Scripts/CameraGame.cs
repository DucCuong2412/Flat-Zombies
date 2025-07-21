using UnityEngine;

public class CameraGame : MonoBehaviour
{
	public float width = 10f;

	public float slide;

	public float leftWall;

	public float rightWall;

	private Camera cameraScene;

	private Player player;

	private Vector3 positionCamera;

	private void OnDrawGizmosSelected()
	{
		if (base.enabled)
		{
			positionCamera = base.transform.position;
			Gizmos.color = new Color(0f, 1f, 0.7f, 0.5f);
			Gizmos.DrawLine(positionCamera, new Vector3(leftWall, positionCamera.y - 10f, positionCamera.z));
			Gizmos.DrawLine(positionCamera, new Vector3(rightWall, positionCamera.y - 10f, positionCamera.z));
			Gizmos.DrawLine(positionCamera, new Vector3(positionCamera.x + slide, positionCamera.y - 6f, positionCamera.z));
			Gizmos.color = new Color(0f, 1f, 0.7f);
			Gizmos.DrawLine(new Vector3(positionCamera.x + slide, positionCamera.y - 6f, positionCamera.z), new Vector3(positionCamera.x + slide, positionCamera.y + 6f, positionCamera.z));
			Gizmos.DrawLine(new Vector3(leftWall, positionCamera.y - 10f, positionCamera.z), new Vector3(leftWall, positionCamera.y + 10f, positionCamera.z));
			Gizmos.DrawLine(new Vector3(rightWall, positionCamera.y - 10f, positionCamera.z), new Vector3(rightWall, positionCamera.y + 10f, positionCamera.z));
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
		cameraScene = GetComponent<Camera>();
		cameraScene.orthographicSize = Mathf.Round(width / cameraScene.aspect * 100f) / 100f;
		player = UnityEngine.Object.FindObjectOfType<Player>();
	}

	private void LateUpdate()
	{
		positionCamera = cameraScene.transform.position;
		ref Vector3 reference = ref positionCamera;
		Vector3 position = player.gameObject.transform.position;
		reference.x = position.x - slide;
		positionCamera.x = Mathf.Clamp(positionCamera.x, leftWall + width, rightWall - width);
		cameraScene.transform.position = positionCamera;
	}
}
