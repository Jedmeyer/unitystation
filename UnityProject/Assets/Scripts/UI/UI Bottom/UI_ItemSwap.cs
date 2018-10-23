﻿using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSwap : MonoBehaviour, IPointerClickHandler
{
	private UI_ItemSlot itemSlot;

	public void OnPointerClick(BaseEventData eventData)
	{
		OnPointerClick((PointerEventData)eventData);
	}
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			SoundManager.Play("Click01");
			UIManager.Hands.SwapItem(itemSlot);
		}
	}

	private void Start()
	{
		itemSlot = GetComponentInChildren<UI_ItemSlot>();
	}
}