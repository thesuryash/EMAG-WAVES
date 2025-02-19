using UnityEngine;
using UnityEngine.EventSystems;

public class PanelInteraction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static bool isMouseinPlayArea = true;

    public void OnPointerDown(PointerEventData eventData)
    {
        isMouseinPlayArea = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isMouseinPlayArea = true;
    }

    /*
    public void OnDrag(PointerEventData eventData)
    {
        
    }
    */
}