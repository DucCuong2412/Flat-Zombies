using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractionArea : MonoBehaviour
{
	public delegate void Method(InteractionArea area);

	public static InteractionArea activeArea;

	public static Player player;

	public bool onceActivate = true;

	private bool activation = true;

	public float radius = 1.4f;

	public Rect rectButton = default(Rect);

	public Sprite icon;

	public bool background = true;

	private Color startColor;

	private Rect worldRectButton = default(Rect);

	[HideInInspector]
	public Vector2 positionHitLocal = default(Vector2);

	private Vector3[] points = new Vector3[21];

	private Vector2 distPlayer = new Vector2(1000f, 1000f);

	private float timer;

	private Method[] listeners = new Method[0];

	private Vector3 positionRectButton = default(Vector3);

	private Vector2 offsetMinButton = default(Vector2);

	private Vector2 offsetMaxButton = default(Vector2);

	private Vector3 lastPositionCamera = default(Vector3);

	private RectTransform canvasRect;

	private void OnDrawGizmos()
	{
		if (base.enabled)
		{
			Vector3 position = base.gameObject.transform.position;
			float num = radius;
			Vector3 localScale = base.gameObject.transform.localScale;
			float num2 = num * localScale.x;
			Gizmos.color = new Color(0f, 0.5f, 1f);
			for (int i = 0; i < points.Length; i++)
			{
				points[i].x = num2 * Mathf.Cos((float)(18 * i) * ((float)Math.PI / 180f));
				points[i].y = num2 * Mathf.Sin((float)(18 * i) * ((float)Math.PI / 180f));
				points[i] += position;
			}
			for (int j = 1; j < points.Length; j++)
			{
				Gizmos.DrawLine(points[j - 1], points[j]);
			}
			Gizmos.DrawLine(position, points[20]);
			Gizmos.color = new Color(0f, 0.05f, 1f);
			Gizmos.DrawLine(position, points[4]);
			if (distPlayer.magnitude <= radius)
			{
				Gizmos.color = new Color(1f, 0.1f, 0f);
				Gizmos.DrawLine(position, base.transform.TransformPoint(distPlayer));
			}
			worldRectButton = rectButton;
			worldRectButton.position = base.transform.TransformPoint(rectButton.position);
			points[0].x = worldRectButton.xMin;
			points[0].y = worldRectButton.yMin;
			points[1].x = worldRectButton.xMax;
			points[1].y = worldRectButton.yMin;
			points[2].x = worldRectButton.xMax;
			points[2].y = worldRectButton.yMax;
			points[3].x = worldRectButton.xMin;
			points[3].y = worldRectButton.yMax;
			Gizmos.DrawLine(points[0], points[1]);
			Gizmos.DrawLine(points[1], points[2]);
			Gizmos.DrawLine(points[2], points[3]);
			Gizmos.DrawLine(points[3], points[0]);
		}
	}

	private void Awake()
	{
		if (player == null)
		{
			player = UnityEngine.Object.FindObjectOfType<Player>();
			player.buttonUse.gameObject.SetActive(value: false);
		}
		startColor = player.buttonUse.GetComponent<Image>().color;
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer > 0.25f)
		{
			timer = 0f;
			distPlayer = player.transform.position - base.transform.position;
			distPlayer.x = Mathf.Floor(distPlayer.x * 100f) / 100f;
			distPlayer.y = Mathf.Floor(distPlayer.y * 100f) / 100f;
		}
		if (activation && distPlayer.magnitude <= radius && !player.isDead && radius != 0f && (activeArea == null || activeArea == this))
		{
			ResizeButton(player.buttonUse);
			if (activeArea == null)
			{
				ExecuteEvents.Execute(base.gameObject, null, delegate(IInteractionArea handler, BaseEventData data)
				{
					handler.OnActiveArea();
				});
				player.OnActiveArea(this);
			}
			activeArea = this;
		}
		else if (activeArea == this)
		{
			player.buttonUse.gameObject.SetActive(value: false);
			ExecuteEvents.Execute(base.gameObject, null, delegate(IInteractionArea handler, BaseEventData data)
			{
				handler.OnInactiveArea();
			});
			player.OnInactiveArea(this);
			activeArea = null;
		}
		else if (player.isDead)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		if (player != null && player.buttonUse != null)
		{
			player.buttonUse.gameObject.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		if (player != null && player.buttonUse != null)
		{
			player.buttonUse.gameObject.SetActive(value: false);
		}
	}

	public void AddListener(Method method)
	{
		if (listeners.Length == 0)
		{
			listeners = new Method[5];
		}
		if (listeners[listeners.Length - 1] != null)
		{
			UnityEngine.Debug.LogWarning("InteractionArea.AddListener(): Список методов переполнен");
		}
		int num = 0;
		while (true)
		{
			if (num < listeners.Length)
			{
				if (listeners[num] == null)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		listeners[num] = method;
	}

	private void ResizeButton(Button buttonUse)
	{
		if (!buttonUse.gameObject.activeSelf)
		{
			lastPositionCamera = Vector3.zero;
			buttonUse.targetGraphic.GetComponent<Image>().sprite = icon;
			buttonUse.gameObject.SetActive(value: true);
			buttonUse.onClick.RemoveAllListeners();
			buttonUse.onClick.AddListener(Activation);
			if (!background)
			{
				buttonUse.GetComponent<Image>().color = Color.clear;
			}
			else
			{
				buttonUse.GetComponent<Image>().color = startColor;
			}
		}
		if (lastPositionCamera != Camera.main.transform.position)
		{
			canvasRect = buttonUse.targetGraphic.canvas.GetComponent<RectTransform>();
			offsetMinButton.x = rectButton.xMin;
			offsetMinButton.y = rectButton.yMin;
			offsetMaxButton.x = rectButton.xMax;
			offsetMaxButton.y = rectButton.yMax;
			offsetMinButton = Camera.main.WorldToScreenPoint(offsetMinButton);
			offsetMaxButton = Camera.main.WorldToScreenPoint(offsetMaxButton);
			positionRectButton = base.transform.TransformPoint(rectButton.position);
			positionRectButton = Camera.main.WorldToScreenPoint(positionRectButton);
			ref Vector2 reference = ref offsetMinButton;
			float x = offsetMinButton.x;
			Vector3 lossyScale = canvasRect.lossyScale;
			reference.x = x / lossyScale.x;
			ref Vector2 reference2 = ref offsetMinButton;
			float y = offsetMinButton.y;
			Vector3 lossyScale2 = canvasRect.lossyScale;
			reference2.y = y / lossyScale2.y;
			ref Vector2 reference3 = ref offsetMaxButton;
			float x2 = offsetMaxButton.x;
			Vector3 lossyScale3 = canvasRect.lossyScale;
			reference3.x = x2 / lossyScale3.x;
			ref Vector2 reference4 = ref offsetMaxButton;
			float y2 = offsetMaxButton.y;
			Vector3 lossyScale4 = canvasRect.lossyScale;
			reference4.y = y2 / lossyScale4.y;
			ref Vector3 reference5 = ref positionRectButton;
			float x3 = positionRectButton.x;
			Vector3 lossyScale5 = canvasRect.lossyScale;
			reference5.x = x3 / lossyScale5.x;
			ref Vector3 reference6 = ref positionRectButton;
			float y3 = positionRectButton.y;
			Vector3 lossyScale6 = canvasRect.lossyScale;
			reference6.y = y3 / lossyScale6.y;
			buttonUse.GetComponent<RectTransform>().pivot = Vector2.zero;
			buttonUse.GetComponent<RectTransform>().offsetMin = offsetMinButton;
			buttonUse.GetComponent<RectTransform>().offsetMax = offsetMaxButton;
			buttonUse.GetComponent<RectTransform>().anchoredPosition = positionRectButton;
			lastPositionCamera = Camera.main.transform.position;
			canvasRect = null;
		}
	}

	public void Activation()
	{
		if (!activation)
		{
			return;
		}
		activation = !onceActivate;
		UnityEngine.Debug.Log(base.gameObject.name + ".InteractionArea.Activation ()");
		ExecuteEvents.Execute(base.gameObject, null, delegate(IInteractionArea handler, BaseEventData data)
		{
			handler.OnActivationArea();
		});
		for (int i = 0; i < listeners.Length; i++)
		{
			if (listeners[i] != null)
			{
				listeners[i](this);
			}
		}
	}
}
