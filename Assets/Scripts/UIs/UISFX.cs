using UnityEngine;
using UnityEngine.EventSystems;

public class UISFX : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.instance.Play("button_onclick");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.instance.Play("button_onhover");
    }
}
