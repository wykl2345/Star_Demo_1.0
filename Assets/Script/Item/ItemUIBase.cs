using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUIBase : MonoBehaviour,IPointerClickHandler
{
    public Image myIcon;
    public Text myQua;
    public int gridID = 0;
    
    
    public void Start()
    {
        myIcon = GetComponent<Image>();
        myQua = GetComponentInChildren<Text>();
    }

    public void ReLoad(String info, Sprite icon)
    {
        this.myQua.text = info;
        this.myIcon.sprite = icon;
    }

    public void SetID(int id)
    {
        gridID = id;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            MessageManager.Instance.Dispatch("Lift_Click_Item",new object[]{gridID});
        }else if (eventData.button == PointerEventData.InputButton.Right)
        {
            MessageManager.Instance.Dispatch("Right_Click_Item",new object[]{gridID});
        }
    }
}
