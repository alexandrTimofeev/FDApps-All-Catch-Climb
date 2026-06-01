using UnityEngine;
using UnityEngine.EventSystems;

public class UICatchButton : MonoBehaviour, IPointerDownHandler
{
    private CatchPressed_event catchPressed_Event = new();

    public void OnPointerDown(PointerEventData eventData)
    {
        EventBus.Publish(catchPressed_Event);
    }
}
