using UnityEngine;
using UnityEngine.EventSystems;

public class SliderInteraction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static bool isMouseOverSlider = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isMouseOverSlider = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isMouseOverSlider = false;
    }
}
