using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;



public class VirtualSlider : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private Image bgImg;
    private Image jsImg;

    private bool isDragged;
    private Vector2 dragInitPos;
    
    [Serializable]//Indicates that a class or a struct can be serialized. (attributes in c# are complicated)
    public class DirectionChangeEvent : UnityEvent<float> { }

    public DirectionChangeEvent directionChageEvents;
    public float sensitivitiy = 0.005f;


    private void Awake()
    {
        bgImg = GetComponent<Image>();
        jsImg = transform.GetChild(0).GetComponent<Image>();
        isDragged = false;
    }


    public void Update()
    {
        if (isDragged)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, Input.mousePosition, GetComponentInParent<Canvas>().worldCamera, out pos);


            float pos_limit_x = bgImg.rectTransform.sizeDelta.x / 2 * 0.8f;

            pos.x = Mathf.Clamp(pos.x, -pos_limit_x, pos_limit_x);
            pos.y = jsImg.rectTransform.anchoredPosition.y;

            jsImg.rectTransform.anchoredPosition = pos;

            pos.x = pos.x / pos_limit_x * sensitivitiy;

            directionChageEvents.Invoke(pos.x);
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Called");
        Vector2 pos;
        isDragged = RectTransformUtility.ScreenPointToLocalPointInRectangle(jsImg.rectTransform, eventData.position, eventData.pressEventCamera, out pos);
        if (isDragged)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform, eventData.position, eventData.pressEventCamera, out pos);
            jsImg.rectTransform.anchoredPosition = new Vector2(pos.x, jsImg.rectTransform.anchoredPosition.y);
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        jsImg.rectTransform.anchoredPosition = Vector3.zero;
        isDragged = false;
    }

}
