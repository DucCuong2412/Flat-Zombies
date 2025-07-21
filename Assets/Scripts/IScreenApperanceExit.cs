using UnityEngine.EventSystems;

public interface IScreenApperanceExit : IEventSystemHandler
{
	void OnScreenExit(string nameObject);
}
