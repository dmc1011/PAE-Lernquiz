using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SideMenu : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform sideMenuRectTransform;
    [SerializeField] private Button settingsButton;
    private float width;
    private float startPositionX;
    private float startingAnchoredPositionX;

    public enum Side { left, right }
    [SerializeField] private Side side;

    private bool isOpen = false; 


    void Start()
    {
        width = Screen.width;

        float initialPosition = (side == Side.right) ? GetMaxPosition() : GetMinPosition();
        sideMenuRectTransform.anchoredPosition = new Vector2(initialPosition, 0);

        settingsButton.onClick.AddListener(ToggleMenu);
    }

    public void OnDrag(PointerEventData eventData)
    {
        sideMenuRectTransform.anchoredPosition = new Vector2(
            Mathf.Clamp(startingAnchoredPositionX - (startPositionX - eventData.position.x), GetMinPosition(), GetMaxPosition()), 
            0);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        startPositionX = eventData.position.x;
        startingAnchoredPositionX = sideMenuRectTransform.anchoredPosition.x;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        bool shouldOpen = isAfterHalfPoint();
        float targetPosition = shouldOpen ? GetMinPosition() : GetMaxPosition();
        StartCoroutine(HandleMenuSlide(.25f, sideMenuRectTransform.anchoredPosition.x, targetPosition));
        isOpen = shouldOpen; 
    }

    private bool isAfterHalfPoint()
    {
        if (side == Side.right)
            return sideMenuRectTransform.anchoredPosition.x < width;
        else
            return sideMenuRectTransform.anchoredPosition.x < 0;
    }

    private float GetMinPosition()
    {
        if (side == Side.right)
            return width * 0.6f;
        return -width * 0.4f;
    }

    private float GetMaxPosition()
    {
        if (side == Side.right)
            return width * 1.4f;
        return width / 2;
    }

    private IEnumerator HandleMenuSlide(float slideTime, float startingX, float targetX)
    {
        for (float i = 0; i <= slideTime; i += .025f)
        {
            sideMenuRectTransform.anchoredPosition = new Vector2(Mathf.Lerp(startingX, targetX, i / slideTime), 0);
            yield return new WaitForSecondsRealtime(.025f);
        }
        sideMenuRectTransform.anchoredPosition = new Vector2(targetX, 0);  // Ensure exact final position
    }

    private void ToggleMenu()
    {
        float targetPosition = isOpen ? GetMaxPosition() : GetMinPosition();
        StartCoroutine(HandleMenuSlide(.25f, sideMenuRectTransform.anchoredPosition.x, targetPosition));
        isOpen = !isOpen; 
    }
}