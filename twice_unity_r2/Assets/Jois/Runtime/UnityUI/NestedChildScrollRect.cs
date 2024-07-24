using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NestedChildScrollRect : ScrollRect
{
    ScrollRect _parentScroll;
    bool _draggingParent = false;

    protected override void Awake()
    {
        base.Awake();
        _parentScroll = GetScrollParent(transform);
    }

    ScrollRect GetScrollParent(Transform t)
    {
        if (t.parent != null)
        {
            ScrollRect scroll = t.parent.GetComponent<ScrollRect>();
            if (scroll != null) return scroll;
            else return GetScrollParent(t.parent);
        }

        return null;
    }

    bool IsPotentialParentDrag(Vector2 inputDelta)
    {
        if (_parentScroll != null)
        {
            if (_parentScroll.horizontal && !_parentScroll.vertical)
            {
                return Mathf.Abs(inputDelta.x) > Mathf.Abs(inputDelta.y);
            }

            if (!_parentScroll.horizontal && _parentScroll.vertical)
            {
                return Mathf.Abs(inputDelta.x) < Mathf.Abs(inputDelta.y);
            }
            else return true;
        }

        return false;
    }

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        base.OnInitializePotentialDrag(eventData);
        _parentScroll?.OnInitializePotentialDrag(eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (IsPotentialParentDrag(eventData.delta))
        {
            _parentScroll.OnBeginDrag(eventData);
            _draggingParent = true;
        }
        else
        {
            base.OnBeginDrag(eventData);
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (_draggingParent)
        {
            _parentScroll.OnDrag(eventData);
        }
        else
        {
            base.OnDrag(eventData);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (_parentScroll != null && _draggingParent)
        {
            _draggingParent = false;
            _parentScroll.OnEndDrag(eventData);
        }
    }
}