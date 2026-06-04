using UnityEngine;
using UnityEngine.EventSystems;

public class MovementButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private int direction;
    private IEventBus eventBus;
    private void Awake()
    {
        eventBus = ServiceLocator.GetService<IEventBus>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        eventBus.Publish(new Events.OnGameInput(direction));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        eventBus.Publish(new Events.OnGameInput(0));
    }
}