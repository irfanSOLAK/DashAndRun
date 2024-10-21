using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    //************* floating de canvas render mode kameradaysa o zaman ilk dokunuşta sorun yaşıyorsun
    //************* sebebi joystick de can null geliyor OnDrag daki cam atamayı start a al

    protected override void Start()
    {
        base.Start();

    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
    }
}