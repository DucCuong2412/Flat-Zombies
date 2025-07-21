using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractionArea, IScreenApperanceExit, IEventSystemHandler
{
	public static string lastScene = string.Empty;

	public static string lastDoor = string.Empty;

	public static string listBlocked = string.Empty;

	public static string targetPosition = string.Empty;

	public string sceneName;

	public string positionName;

	public bool positionPlayerIsDoor;

	public AudioClip soundCurrentDoor;

	public bool blockDoor;

	public GameObject markClearRoom;

	private InteractionArea area;

	public static void ResetListBlocked()
	{
		PlayerPrefsFile.DeleteKey("blockedDoors");
		PlayerPrefsFile.DeleteKey("lastOpenDoor");
		PlayerPrefsFile.DeleteKey("lastSceenDoor");
		listBlocked = string.Empty;
		lastDoor = string.Empty;
		lastScene = string.Empty;
		targetPosition = string.Empty;
	}

	public static void Lock(Door door)
	{
		string text = door.gameObject.name + ";";
		listBlocked = listBlocked.Replace(text, string.Empty);
		listBlocked += text;
		PlayerPrefsFile.SetString("blockedDoors", listBlocked);
	}

	public static bool IsLocked(Door door)
	{
		if (listBlocked == string.Empty)
		{
			listBlocked = PlayerPrefsFile.GetString("blockedDoors", string.Empty);
			lastDoor = PlayerPrefsFile.GetString("lastOpenDoor", string.Empty);
			lastScene = PlayerPrefsFile.GetString("lastSceenDoor", string.Empty);
		}
		return listBlocked.IndexOf(door.gameObject.name + ";") != -1;
	}

	private void Start()
	{
		UnityEngine.Debug.Log("ManagerZombies.aliveSceneTotal:" + ManagerZombies.aliveSceneTotal.ToString());
		area = base.gameObject.GetComponent<InteractionArea>();
		if (string.IsNullOrEmpty(positionName))
		{
			positionName = lastDoor;
		}
		if (positionPlayerIsDoor && base.name == lastDoor)
		{
			targetPosition = base.name;
		}
		if (base.name == targetPosition)
		{
			InteractionArea.player.transform.position = base.transform.position;
			InteractionArea.player.PlaySound(soundCurrentDoor);
			targetPosition = string.Empty;
		}
		Block();
	}

	public void Block()
	{
		if (blockDoor && IsLocked(this) && base.enabled)
		{
			if (markClearRoom != null)
			{
				markClearRoom.SetActive(value: true);
				if (markClearRoom.transform.parent == base.transform)
				{
					markClearRoom.transform.SetParent(base.transform.parent);
					markClearRoom.name = base.name + "." + markClearRoom.name;
				}
			}
			markClearRoom = null;
			if (area != null)
			{
				area.radius = 0f;
				area.enabled = false;
				area = null;
			}
			base.enabled = false;
			base.gameObject.SetActive(value: false);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (markClearRoom != null)
		{
			markClearRoom.SetActive(value: false);
			UnityEngine.Object.Destroy(markClearRoom);
			markClearRoom = null;
		}
	}

	public void OnActiveArea()
	{
		Block();
	}

	public void OnInactiveArea()
	{
	}

	public void OnActivationArea()
	{
		if (!IsLocked(this))
		{
			ScreenApperance.main.PlayExit(base.name);
		}
	}

	public void OnScreenExit(string nameObject)
	{
		if (nameObject == base.name)
		{
			targetPosition = positionName;
			if (blockDoor)
			{
				Lock(this);
			}
			if (positionPlayerIsDoor)
			{
				lastDoor = base.name;
				lastScene = SceneManager.GetActiveScene().name;
				PlayerPrefsFile.SetString("lastOpenDoor", lastDoor);
				PlayerPrefsFile.SetString("lastSceenDoor", lastScene);
			}
			SceneManager.LoadScene(sceneName);
		}
	}
}
