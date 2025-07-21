using UnityEngine.EventSystems;

public interface IInteractionArea : IEventSystemHandler
{
	void OnActiveArea();

	void OnInactiveArea();

	void OnActivationArea();
}
