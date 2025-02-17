using System;
using UnityEngine;
using UnityEngine.UI;

public class DeckLogic : MonoBehaviour
{
    public HorizontalCardHolder HorizontalCardHolder;

    public Button addButton;
    public Button removeButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        addButton.onClick.AddListener(OnClick);
        removeButton.onClick.AddListener(RemoveCard);
    }


    public void OnClick()
    {
        HorizontalCardHolder.AddCard();
    }

    public void RemoveCard()
    {
        HorizontalCardHolder.ClearCards();
    }
 }
