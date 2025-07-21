using UnityEngine;
using UnityEngine.UI;

public class ButtonsControll : MonoBehaviour
{
	public int version;

	public Button[] buttons;

	private void Awake()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			EditorControlls.PositionButton(buttons[i], version);
		}
		buttons = new Button[0];
	}
}
