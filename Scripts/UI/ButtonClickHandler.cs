﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ButtonClickHandler : MonoBehaviour, IPointerClickHandler{

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(eventData.button);
    }
}
