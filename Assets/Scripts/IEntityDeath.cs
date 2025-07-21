using UnityEngine.EventSystems;

public interface IEntityDeath : IEventSystemHandler
{
	void OnDeathEntity(Entity entity);
}
