using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Shoper : MonoBehaviour, IPointerClickHandler
{
    public const string Name = "shop";
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            MessageManager.Instance.Dispatch("Open_Dialog");
        }
    }

    public void StartDialog()
    {
        Debug.Log("Right_Click");
        MessageManager.Instance.Dispatch("Open_Dialog");
    }
}
