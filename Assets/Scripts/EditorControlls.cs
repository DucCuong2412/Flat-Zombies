using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorControlls : MonoBehaviour
{
	public struct AnchoredPositionButton
	{
		public string name;

		public Vector2 anchoredPosition;
	}

	public static AnchoredPositionButton[] startDefaultPosition = new AnchoredPositionButton[10];

	public int version;

	public Canvas canvas;

	private RectTransform canvasRect;

	public int sizeGrid = 10;

	public TouchButtonPointer areaButton;

	public Button[] buttons;

	public Button reset;

	public Button resize;

	public Button save;

	private TouchButtonPointer[] areasButtons;

	private TouchButtonDrag buttonDrag;

	private RectTransform rectTransform;

	private RectTransform rectButton;

	private Graphic[] images;

	private float currentScale = 1f;

	private float audioVolume;

	private Vector3[] newPositions = new Vector3[0];

	private int idSelected = -1;

	private Vector2 slidePositionDrag = default(Vector2);

	private Vector2 positionCanvas = default(Vector2);

	public static void PositionButton(Button button, int version)
	{
		Vector3 vector = button.GetComponent<RectTransform>().anchoredPosition;
		for (int i = 0; i < startDefaultPosition.Length; i++)
		{
			if (startDefaultPosition[i].name == button.name || string.IsNullOrEmpty(startDefaultPosition[i].name))
			{
				startDefaultPosition[i].name = button.name;
				startDefaultPosition[i].anchoredPosition = vector;
				break;
			}
		}
		float num = 0.2f;
		if (PlayerPrefsFile.GetInt("versionEditorControlls", 0) == version)
		{
			button.GetComponent<RectTransform>().anchoredPosition = PlayerPrefsFile.GetVector3(button.name, vector);
			num = PlayerPrefsFile.GetFloat(button.name + ".scale", 1f);
			num = Mathf.Max(0.5f, num);
			button.GetComponent<RectTransform>().localScale = new Vector3(num, num, 1f);
		}
	}

	private void Start()
	{
		canvasRect = canvas.GetComponent<RectTransform>();
		reset.onClick.AddListener(ResetPositions);
		resize.onClick.AddListener(ResizeButtons);
		save.onClick.AddListener(SavePositions);
		areasButtons = new TouchButtonPointer[buttons.Length];
		buttonDrag = base.gameObject.AddComponent<TouchButtonDrag>();
		buttonDrag.enabled = false;
		buttonDrag.drag.AddListener(DragButton);
		buttonDrag.endDrag.AddListener(delegate
		{
			StopMoveButton();
		});
		for (int i = 0; i < buttons.Length; i++)
		{
			if (!(areaButton != null))
			{
				break;
			}
			buttons[i].enabled = false;
			images = buttons[i].GetComponentsInChildren<Graphic>();
			for (int j = 0; j < images.Length; j++)
			{
				images[j].raycastTarget = false;
			}
			areasButtons[i] = UnityEngine.Object.Instantiate(areaButton);
			areasButtons[i].name = buttons[i].name;
			areasButtons[i].pointerDown.AddListener(StartMoveButton);
			areasButtons[i].transform.SetParent(base.transform);
			rectTransform = areasButtons[i].GetComponent<RectTransform>();
			rectButton = buttons[i].GetComponent<RectTransform>();
			rectTransform.pivot = rectButton.pivot;
			rectTransform.sizeDelta = rectButton.sizeDelta;
			rectTransform.anchorMax = rectButton.anchorMax;
			rectTransform.anchorMin = rectButton.anchorMin;
			rectTransform.anchoredPosition = rectButton.anchoredPosition;
			rectTransform.position = rectButton.position;
			rectTransform.localScale = rectButton.localScale;
			rectButton.transform.SetParent(rectTransform.transform);
			rectButton.localPosition = default(Vector3);
			Vector3 localScale = rectTransform.localScale;
			currentScale = localScale.x;
			UnityEngine.Object.Destroy(buttons[i]);
		}
		buttons = new Button[0];
	}

	private void OnEnable()
	{
		if (AudioListener.volume != 0f)
		{
			audioVolume = AudioListener.volume;
			AudioListener.volume = 0f;
		}
	}

	private void OnDisable()
	{
		if (AudioListener.volume != audioVolume && audioVolume != 0f)
		{
			AudioListener.volume = audioVolume;
		}
	}

	public void ResizeButtons()
	{
		currentScale += 0.1f;
		if (currentScale > 1.4f)
		{
			currentScale = 0.8f;
		}
		for (int i = 0; i < areasButtons.Length; i++)
		{
			rectTransform = areasButtons[i].GetComponent<RectTransform>();
			rectTransform.localScale = new Vector3(currentScale, currentScale, 1f);
		}
	}

	public void ResetPositions()
	{
		currentScale = 1f;
		for (int i = 0; i < areasButtons.Length; i++)
		{
			PlayerPrefsFile.DeleteKey("versionEditorControlls");
			PlayerPrefsFile.DeleteKey(areasButtons[i].name);
			PlayerPrefsFile.DeleteKey(areasButtons[i].name + ".scale");
			for (int j = 0; j < startDefaultPosition.Length; j++)
			{
				if (startDefaultPosition[j].name == areasButtons[i].name)
				{
					rectTransform = areasButtons[i].GetComponent<RectTransform>();
					rectTransform.anchoredPosition = startDefaultPosition[j].anchoredPosition;
					rectTransform.localScale = Vector3.one;
					break;
				}
			}
		}
	}

	public void SavePositions()
	{
		PlayerPrefsFile.SetInt("versionEditorControlls", version);
		newPositions = new Vector3[areasButtons.Length];
		for (int i = 0; i < areasButtons.Length; i++)
		{
			rectTransform = areasButtons[i].GetComponent<RectTransform>();
			newPositions[i] = rectTransform.anchoredPosition;
			newPositions[i].x = Mathf.Round(newPositions[i].x / (float)sizeGrid) * (float)sizeGrid;
			newPositions[i].y = Mathf.Round(newPositions[i].y / (float)sizeGrid) * (float)sizeGrid;
			PlayerPrefsFile.SetVector3(areasButtons[i].name, newPositions[i]);
			string key = areasButtons[i].name + ".scale";
			Vector3 localScale = rectTransform.localScale;
			PlayerPrefsFile.SetFloat(key, localScale.x);
		}
		PlayerPrefsFile.Save();
	}

	public void StartMoveButton(PointerEventData eventData)
	{
		for (int i = 0; i < areasButtons.Length; i++)
		{
			if (areasButtons[i].gameObject == eventData.selectedObject)
			{
				ref Vector2 reference = ref positionCanvas;
				Vector2 position = eventData.position;
				float x = position.x;
				Vector3 lossyScale = canvasRect.lossyScale;
				reference.x = x / lossyScale.x;
				ref Vector2 reference2 = ref positionCanvas;
				Vector2 position2 = eventData.position;
				float y = position2.y;
				Vector3 lossyScale2 = canvasRect.lossyScale;
				reference2.y = y / lossyScale2.y;
				slidePositionDrag = areasButtons[i].GetComponent<RectTransform>().anchoredPosition - positionCanvas;
				areasButtons[i].transform.SetAsLastSibling();
				idSelected = i;
				break;
			}
		}
		buttonDrag.enabled = true;
	}

	public void DragButton(PointerEventData eventData)
	{
		if (idSelected != -1)
		{
			ref Vector2 reference = ref positionCanvas;
			Vector2 position = eventData.position;
			float x = position.x;
			Vector3 lossyScale = canvasRect.lossyScale;
			reference.x = x / lossyScale.x;
			ref Vector2 reference2 = ref positionCanvas;
			Vector2 position2 = eventData.position;
			float y = position2.y;
			Vector3 lossyScale2 = canvasRect.lossyScale;
			reference2.y = y / lossyScale2.y;
			positionCanvas += slidePositionDrag;
			positionCanvas.x = Mathf.Round(positionCanvas.x / (float)sizeGrid) * (float)sizeGrid;
			positionCanvas.y = Mathf.Round(positionCanvas.y / (float)sizeGrid) * (float)sizeGrid;
			areasButtons[idSelected].GetComponent<RectTransform>().anchoredPosition = positionCanvas;
		}
	}

	public void StopMoveButton()
	{
		idSelected = -1;
		buttonDrag.enabled = false;
	}
}
