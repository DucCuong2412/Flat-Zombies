using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("PVold/UI/ShareImage")]
public class ShareImage : MonoBehaviour
{
	public Camera cameraScene;

	[TextArea(3, 10)]
	public string messageRus = "Текст сообщения";

	[TextArea(3, 10)]
	public string messageEng = "Text message";

	[HideInInspector]
	public string message;

	[Space(8f)]
	public Button button;

	private string destination = string.Empty;

	private void OnDrawGizmosSelected()
	{
		if (button == null)
		{
			button = base.gameObject.GetComponent<Button>();
		}
		if (cameraScene == null)
		{
			cameraScene = Camera.main;
		}
	}

	private void Start()
	{
		if (button != null)
		{
			button.onClick.AddListener(SendImage);
		}
		if (cameraScene == null)
		{
			cameraScene = Camera.main;
		}
		if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian)
		{
			message = messageRus;
		}
		else
		{
			message = messageEng;
		}
	}

	private void OnDestroy()
	{
		if (destination != string.Empty)
		{
			File.Delete(destination);
			destination = string.Empty;
		}
	}

	private void OnPostRender()
	{
		UnityEngine.Debug.Log("OnPostRender");
	}

	public void SendImage()
	{
		StartCoroutine(Asd());
	}

	private IEnumerator Asd()
	{
		yield return new WaitForEndOfFrame();
		UnityEngine.Debug.Log("SHARE");
		Texture2D texture = new Texture2D(cameraScene.pixelWidth, cameraScene.pixelHeight, TextureFormat.RGB24, mipChain: false);
		texture.ReadPixels(new Rect(0f, 0f, cameraScene.pixelWidth, cameraScene.pixelHeight), 0, 0);
		texture.Apply();
		destination = Path.Combine(Application.persistentDataPath, "share-rfpsv.png");
		File.WriteAllBytes(destination, texture.EncodeToPNG());
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
		intentObject.Call<AndroidJavaObject>("setAction", new object[1]
		{
			intentClass.GetStatic<string>("ACTION_SEND")
		});
		AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", new object[1]
		{
			"file://" + destination
		});
		intentObject.Call<AndroidJavaObject>("putExtra", new object[2]
		{
			intentClass.GetStatic<string>("EXTRA_TEXT"),
			message
		});
		intentObject.Call<AndroidJavaObject>("putExtra", new object[2]
		{
			intentClass.GetStatic<string>("EXTRA_STREAM"),
			uriObject
		});
		intentObject.Call<AndroidJavaObject>("setType", new object[1]
		{
			"image/jpeg"
		});
		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
		currentActivity.Call("startActivity", intentObject);
	}
}
