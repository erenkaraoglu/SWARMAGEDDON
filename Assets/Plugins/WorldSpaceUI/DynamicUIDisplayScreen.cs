using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DynamicUIDisplayScreen : MonoBehaviour
{
    [SerializeField] LayerMask RaycastMask = ~0;
    [SerializeField] float RaycastDistance = 5.0f;
    [SerializeField] Vector2 MouseScrollSensitivity = Vector2.one;
    [SerializeField] bool InvertHorizontalScoll = false;
    [SerializeField] bool InvertVerticalScroll = false;

    [SerializeField] UnityEvent<Vector2, Vector2> OnCursorInput = new();

    private bool isDragging = false;
    private Vector2 lastHitTextureCoord;

    // Update is called once per frame
    void Update()
    {
        Vector3 MousePosition = Mouse.current.position.ReadValue();
        Vector2 MouseScroll = Mouse.current.scroll.ReadValue();

        // apply sensitivity and inversion
        MouseScroll.x *= MouseScrollSensitivity.x * (InvertHorizontalScoll ? -1f : 1f);
        MouseScroll.y *= MouseScrollSensitivity.y * (InvertVerticalScroll ? -1f : 1f);

        bool isMouseDown = Mouse.current.leftButton.isPressed;
        bool mousePressed = Mouse.current.leftButton.wasPressedThisFrame;
        bool mouseReleased = Mouse.current.leftButton.wasReleasedThisFrame;

        // If mouse was released, end dragging state
        if (mouseReleased)
        {
            isDragging = false;
        }

        // construct our ray from the mouse position
        Ray MouseRay = Camera.main.ScreenPointToRay(MousePosition);

        // perform our raycast
        RaycastHit HitResult;
        if (Physics.Raycast(MouseRay, out HitResult, RaycastDistance, RaycastMask, QueryTriggerInteraction.Ignore))
        {
            // ignore if not us
            if (HitResult.collider.gameObject != gameObject)
            {
                // Continue sending events during a drag operation even when outside the collider
                if (isDragging)
                {
                    OnCursorInput.Invoke(lastHitTextureCoord, MouseScroll);
                }
                return;
            }

            // If mouse is pressed, start drag state
            if (mousePressed)
            {
                isDragging = true;
            }

            lastHitTextureCoord = HitResult.textureCoord;
            OnCursorInput.Invoke(HitResult.textureCoord, MouseScroll);
        }
        else if (isDragging)
        {
            // Keep sending events during a drag operation even when the raycast doesn't hit
            OnCursorInput.Invoke(lastHitTextureCoord, MouseScroll);
        }
    }
}