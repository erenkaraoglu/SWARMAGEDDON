using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DynamicUIStage : MonoBehaviour
{
    [SerializeField] RectTransform CanvasTransform;
    [SerializeField] GraphicRaycaster Raycaster;

    List<GameObject> DragTargets = new();

    public void OnCursorInput(Vector2 InNormalisedPosition, Vector2 InScrollDelta,
                              bool bInMouseDownThisFrame, bool bInMouseUpThisFrame, bool bInIsMouseDown)
    {
        ProcessInput(InNormalisedPosition, InScrollDelta, bInMouseDownThisFrame, bInMouseUpThisFrame, bInIsMouseDown);
    }

    public void OnCursorInput(Vector2 InNormalisedPosition, Vector2 InScrollDelta)
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        bool bMouseDownThisFrame = Input.GetMouseButtonDown(0);
        bool bMouseUpThisFrame = Input.GetMouseButtonUp(0);
        bool bIsMouseDown = Input.GetMouseButton(0);
#else
        bool bMouseDownThisFrame = UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame;
        bool bMouseUpThisFrame = UnityEngine.InputSystem.Mouse.current.leftButton.wasReleasedThisFrame;   
        bool bIsMouseDown = UnityEngine.InputSystem.Mouse.current.leftButton.isPressed;
#endif // ENABLE_LEGACY_INPUT_MANAGER

        ProcessInput(InNormalisedPosition, InScrollDelta, bMouseDownThisFrame, bMouseUpThisFrame, bIsMouseDown);
    }

    protected virtual void ProcessInput(Vector2 InNormalisedPosition, Vector2 InScrollDelta,
                                        bool bInMouseDownThisFrame, bool bInMouseUpThisFrame, bool bInIsMouseDown)
    { 
        // Get the input position in canvas space
        Vector3 InputPosition = new Vector3(CanvasTransform.sizeDelta.x * InNormalisedPosition.x,
                                            CanvasTransform.sizeDelta.y * InNormalisedPosition.y,
                                            0);

        // Builder a pointer event
        PointerEventData PointerEvent = new PointerEventData(EventSystem.current);
        PointerEvent.position = InputPosition;

        // Determine what we've hit in the UI
        List<RaycastResult> Results = new();
        Raycaster.Raycast(PointerEvent, Results);

        // has the mouse button gone up this frame
        if (bInMouseUpThisFrame)
        {
            foreach(var Target in DragTargets)
            {
                if (ExecuteEvents.Execute(Target, PointerEvent, ExecuteEvents.endDragHandler))
                    break;
            }
            DragTargets.Clear();
        }

        // process any hit results
        foreach(var Result in Results)
        {
            // Create a new event data
            PointerEventData PointerEventForResult = new PointerEventData(EventSystem.current);
            PointerEventForResult.position = InputPosition;
            PointerEventForResult.pointerCurrentRaycast = Result;
            PointerEventForResult.pointerPressRaycast = Result;

            // is the button down?
            if (bInIsMouseDown)
                PointerEventForResult.button = PointerEventData.InputButton.Left;

            // is there scroll input?
            if ((Mathf.Abs(InScrollDelta.x) > float.Epsilon) || (Mathf.Abs(InScrollDelta.y) > float.Epsilon))
            {
                PointerEventForResult.scrollDelta = InScrollDelta;
                ExecuteEvents.Execute(Result.gameObject, PointerEventForResult, ExecuteEvents.scrollHandler);
            }

            // retrieve a slider if hit
            var HitSlider = Result.gameObject.GetComponentInParent<Slider>();

            // new drag targets?
            if (bInMouseDownThisFrame)
            {
                if (ExecuteEvents.Execute(Result.gameObject, PointerEventForResult, ExecuteEvents.beginDragHandler))
                    DragTargets.Add(Result.gameObject);

                if (HitSlider != null)
                {
                    HitSlider.OnInitializePotentialDrag(PointerEventForResult);

                    if (!DragTargets.Contains(Result.gameObject))
                        DragTargets.Add(Result.gameObject);
                }
            } // need to update current drag targets?
            else if (DragTargets.Contains(Result.gameObject))
            {
                PointerEventForResult.dragging = true;
                ExecuteEvents.Execute(Result.gameObject, PointerEventForResult, ExecuteEvents.dragHandler);

                if (HitSlider != null)
                    HitSlider.OnDrag(PointerEventForResult);
            }

            if (bInMouseDownThisFrame)
            {
                if (ExecuteEvents.Execute(Result.gameObject, PointerEventForResult, ExecuteEvents.pointerDownHandler))
                    break;
            }
            else if (bInMouseUpThisFrame)
            {
                bool bDidRun = false;
                bDidRun |= ExecuteEvents.Execute(Result.gameObject, PointerEventForResult, ExecuteEvents.pointerUpHandler);
                bDidRun |= ExecuteEvents.Execute(Result.gameObject, PointerEventForResult, ExecuteEvents.pointerClickHandler);

                if (bDidRun)
                    break;
            }
        }
    }
}
