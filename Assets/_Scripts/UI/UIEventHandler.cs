using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventHandler : MonoBehaviour, 
								IPointerEnterHandler, 
								IPointerExitHandler, 
								IPointerClickHandler,
								ISelectHandler,
								IDeselectHandler,
								ISubmitHandler,
								ICancelHandler

{
	public Action<PointerEventData> Clicked;
	public Action<PointerEventData> PointerEntered;
	public Action<PointerEventData> PointerExited;

	public Action<BaseEventData> Selected;
	public Action<BaseEventData> Deselected;
	public Action<BaseEventData> Submitted;

	public Action<BaseEventData> Canceled;

	public void OnPointerClick(PointerEventData eventData)
	{
		Clicked?.Invoke(eventData);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		PointerEntered?.Invoke(eventData);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		PointerExited?.Invoke(eventData);
	}
	public void OnSelect(BaseEventData eventData)
	{
		Selected?.Invoke(eventData);
	}
	public void OnDeselect(BaseEventData eventData)
	{
		Deselected?.Invoke(eventData);
	}
	public void OnSubmit(BaseEventData eventData)
	{
		Submitted?.Invoke(eventData);
	}
	public void OnCancel(BaseEventData eventData)
	{
		Canceled?.Invoke(eventData);
	}
}