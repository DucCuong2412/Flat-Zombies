using UnityEngine;
using UnityEngine.SceneManagement;

public class GDPR : MonoBehaviour
{
	private GUIStyle logoBackgroundStyle = new GUIStyle();

	private GUIStyle mainLabelStyle = new GUIStyle();

	private GUIStyle mainTextStyle = new GUIStyle();

	private GUIStyle button1Style = new GUIStyle();

	private GUIStyle button2Style = new GUIStyle();

	private GUIContent logoContent;

	public Texture2D logoImage;

	public Texture2D closeImage;

	private Texture2D closeTex;

	private bool answerYes;

	private bool answerNo;

	private string mainText = " personalizes your \nadvertising experience using Appodeal.\nAppodeal and its partners may collect\nand process personal data such as\ndevice identifiers, location data, and\ndemographic and interest data to provide\nother advertising experience tailored to\nyou. By consenting to this improved ad\nexperience, you'll see ads that Appodeal\nand its partners believe are more\nrelevant to you.";

	private string mainText1 = "Great. We hope you enjoy\nyour personalized ad experience.";

	private string mainText2 = "Appodeal won't collect your data\nthrough this app for personalized\nadvertising. If you consent yo Appodeal\npersonalizing your advertising experience\nin a different app, we will still\ncollect your data through that app.";

	private string additionalText = "By agreeing, you confirm that you are\nover the age of 16 and would like a\npersonalized ad experience.";

	private string additionalText1 = "I understand that i will still see ads, \nbut they may not be as relevant\nto my interests.";

	private void Start()
	{
		Texture2D texture2D = UnityEngine.Object.Instantiate(logoImage);
		logoContent = new GUIContent(texture2D);
		int num = logoImage.width / logoImage.height;
		int num2 = Screen.width - Screen.width / 3;
		int newHeight = num2 / num;
		TextureScale.Bilinear(texture2D, num2, newHeight);
		closeTex = UnityEngine.Object.Instantiate(closeImage);
		TextureScale.Bilinear(closeTex, Screen.width / 8, Screen.width / 8);
		Texture2D background = MakeTexure(Screen.width, Screen.height / 6, Color.red);
		logoBackgroundStyle.normal.background = background;
		logoBackgroundStyle.contentOffset = new Vector2((Screen.width - texture2D.width) / 2, 40f);
		mainLabelStyle.normal.textColor = Color.black;
		mainLabelStyle.fontStyle = FontStyle.Bold;
		mainLabelStyle.alignment = TextAnchor.UpperCenter;
		mainLabelStyle.fontSize = Screen.height / 30;
		mainTextStyle.normal.textColor = Color.black;
		mainTextStyle.alignment = TextAnchor.UpperLeft;
		mainTextStyle.fontSize = Screen.height / 40;
		mainTextStyle.richText = true;
		button1Style.fontSize = Screen.height / 40;
		button1Style.active.background = MakeTexure(Screen.width - 40, Screen.height / 15, Color.red);
		button1Style.focused.background = MakeTexure(Screen.width - 40, Screen.height / 15, Color.red);
		button1Style.normal.background = MakeTexure(Screen.width - 40, Screen.height / 15, Color.red);
		button1Style.hover.background = MakeTexure(Screen.width - 40, Screen.height / 15, Color.red);
		button1Style.alignment = TextAnchor.MiddleCenter;
		button1Style.normal.textColor = Color.white;
		button1Style.hover.textColor = Color.white;
		button1Style.active.textColor = Color.white;
		button1Style.focused.textColor = Color.white;
		button2Style.fontSize = Screen.height / 40;
		button2Style.active.background = MakeTexure(Screen.width - 40, Screen.height / 15, Color.white);
		button2Style.focused.background = MakeTexure(Screen.width - 40, Screen.height / 15, Color.white);
		button2Style.normal.background = MakeTexure(Screen.width - 40, Screen.height / 15, Color.white);
		button2Style.hover.background = MakeTexure(Screen.width - 40, Screen.height / 15, Color.white);
		button2Style.alignment = TextAnchor.MiddleCenter;
		button2Style.normal.textColor = Color.black;
		button2Style.hover.textColor = Color.black;
		button2Style.active.textColor = Color.black;
		button2Style.focused.textColor = Color.black;
	}

	private Texture2D MakeTexure(int width, int height, Color color)
	{
		Color[] array = new Color[width * height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = color;
		}
		Texture2D texture2D = new Texture2D(width, height);
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	private void OnGUI()
	{
		GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height / 6), logoContent, logoBackgroundStyle);
		GUI.Label(new Rect(20f, Screen.height / 5 - 30, Screen.width - 40, Screen.height / 6), "Personalize Your \nAd Experience", mainLabelStyle);
		if (!answerYes && !answerNo)
		{
			GUI.Label(new Rect(20f, Screen.height / 4 + 20, Screen.width - 40, Screen.height / 6), Application.productName + mainText, mainTextStyle);
			GUI.Label(new Rect(20f, Screen.height - Screen.height / 3 - 40, Screen.width - 20, Screen.height / 6), additionalText, mainTextStyle);
			if (GUI.Button(new Rect(20f, Screen.height - Screen.height / 4 - 10, Screen.width - 40, Screen.height / 15), "YES, I AGREE", button1Style))
			{
				PlayerPrefs.SetInt("result_gdpr", 1);
				answerYes = true;
			}
			if (GUI.Button(new Rect(20f, Screen.height - Screen.height / 5 + 10, Screen.width - 40, Screen.height / 15), "NO, THANKS", button2Style))
			{
				PlayerPrefs.SetInt("result_gdpr", 0);
				answerNo = true;
			}
			GUI.Label(new Rect(20f, Screen.height - Screen.height / 8, Screen.width - 40, Screen.height / 6), additionalText1, mainTextStyle);
		}
		else if (answerYes)
		{
			GUI.Label(new Rect(20f, Screen.height / 4 + 20, Screen.width - 40, Screen.height / 6), mainText1, mainTextStyle);
			if (GUI.Button(new Rect(20f, Screen.height - Screen.height / 5, Screen.width - 40, Screen.height / 15), closeTex, button2Style))
			{
				SceneManager.LoadScene("AppodealDemo");
			}
			if (GUI.Button(new Rect(20f, Screen.height - Screen.height / 8, Screen.width - 40, Screen.height / 15), "CLOSE", button2Style))
			{
				SceneManager.LoadScene("AppodealDemo");
			}
		}
		else if (answerNo)
		{
			GUI.Label(new Rect(20f, Screen.height / 4 + 20, Screen.width - 40, Screen.height / 6), mainText2, mainTextStyle);
			if (GUI.Button(new Rect(20f, Screen.height - Screen.height / 5, Screen.width - 40, Screen.height / 15), closeTex, button2Style))
			{
				SceneManager.LoadScene("AppodealDemo");
			}
			if (GUI.Button(new Rect(20f, Screen.height - Screen.height / 8, Screen.width - 40, Screen.height / 15), "CLOSE", button2Style))
			{
				SceneManager.LoadScene("AppodealDemo");
			}
		}
	}
}
