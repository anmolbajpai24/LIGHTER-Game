using UnityEngine;
using UnityEngine.EventSystems;

public static class LighterInput
{
    public static Vector2 Direction;
    public static bool Jump;
    public static void Reset() { Direction = Vector2.zero; Jump = false; }
}

public class LighterJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform Knob;
    private int pointer = int.MinValue;
    public void OnPointerDown(PointerEventData e) { if (pointer != int.MinValue) return; pointer = e.pointerId; OnDrag(e); }
    public void OnDrag(PointerEventData e)
    {
        if (e.pointerId != pointer) return;
        var rect = (RectTransform)transform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, e.position, e.pressEventCamera, out var point);
        LighterInput.Direction = Vector2.ClampMagnitude(point / (rect.rect.width * .36f), 1);
        Knob.anchoredPosition = LighterInput.Direction * rect.rect.width * .28f;
    }
    public void OnPointerUp(PointerEventData e) { if (e.pointerId != pointer) return; pointer = int.MinValue; LighterInput.Direction = Vector2.zero; Knob.anchoredPosition = Vector2.zero; }
    private void OnDisable() { pointer = int.MinValue; LighterInput.Reset(); if (Knob != null) Knob.anchoredPosition = Vector2.zero; }
}
