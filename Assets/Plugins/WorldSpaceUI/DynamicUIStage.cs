using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
        bool bMouseDownThisFrame = Mouse.current.leftButton.wasPressedThisFrame;
        bool bMouseUpThisFrame = Mouse.current.leftButton.wasReleasedThisFrame;   
        bool bIsMouseDown = Mouse.current.leftButton.isPressed;

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

        // Process any active drag operations first, regardless of what's under the cursor
        if (bInIsMouseDown && DragTargets.Count > 0)
        {
            foreach (var DragTarget in DragTargets)
            {
                PointerEventData DragPointerEvent = new PointerEventData(EventSystem.current);
                DragPointerEvent.position = InputPosition;
                DragPointerEvent.dragging = true;
                DragPointerEvent.button = PointerEventData.InputButton.Left;
                
                ExecuteEvents.Execute(DragTarget, DragPointerEvent, ExecuteEvents.dragHandler);
                
                // Also update sliders if they're being dragged
                var TargetSlider = DragTarget.GetComponentInParent<Slider>();
                if (TargetSlider != null)
                    TargetSlider.OnDrag(DragPointerEvent);
            }
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