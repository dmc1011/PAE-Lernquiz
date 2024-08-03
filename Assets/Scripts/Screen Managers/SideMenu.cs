using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

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
    private bool isInAnimation = false;
    private float targetPositionForCurrentAnimation = 0.0f;
    private float startPositionForCurrentAnimation = 0.0f;
    private float currentAnimationTime = 0.0f;

    // This determines how long the animation takes to complete in Seconds
    private const float animationTime = 0.25f;

    void Start()
    {
        width = Screen.width;

        float initialPosition = (side == Side.right) ? GetMaxPosition() : GetMinPosition();
        sideMenuRectTransform.anchoredPosition = new Vector2(initialPosition, 0);

        settingsButton.onClick.AddListener(ToggleMenu);
    }

    void Update()
    {
        if (isInAnimation)
        {
            currentAnimationTime += Time.deltaTime;
            if (currentAnimationTime < animationTime)
            {
                sideMenuRectTransform.anchoredPosition = new Vector2(
                    Mathf.Lerp(startPositionForCurrentAnimation, targetPositionForCurrentAnimation,
                    Mathf.Pow(currentAnimationTime / animationTime, 2.0f)), 0);
            } else
            {
                sideMenuRectTransform.anchoredPosition = new Vector2(targetPositionForCurrentAnimation, 0);  // Ensure exact final position
                isInAnimation = false;
            }
        }
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
        targetPositionForCurrentAnimation = shouldOpen ? GetMinPosition() : GetMaxPosition();
        startPositionForCurrentAnimation = sideMenuRectTransform.anchoredPosition.x;
        currentAnimationTime = 0.0f;
        isInAnimation = true;
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

    private void ToggleMenu()
    {
        targetPositionForCurrentAnimation = isOpen ? GetMaxPosition() : GetMinPosition();
        startPositionForCurrentAnimation = sideMenuRectTransform.anchoredPosition.x;
        currentAnimationTime = 0.0f;
        isInAnimation = true;
        isOpen = !isOpen; 
    }
}