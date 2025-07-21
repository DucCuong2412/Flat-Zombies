using UnityEngine;
using UnityEngine.EventSystems;

public class ResultNextCorridor : MonoBehaviour, IInteractionArea, IEventSystemHandler
{
	public string nameLevel;

	private void Start()
	{
	}

	public void OnActiveArea()
	{
	}

	public void OnInactiveArea()
	{
	}

	public void OnActivationArea()
	{
		if (base.enabled)
		{
			Door.ResetListBlocked();
			SettingsStartGame.NextCorridor();
			base.enabled = false;
		}
	}
}
