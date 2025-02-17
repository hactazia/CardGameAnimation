using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;
using Unity.VisualScripting;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridCardHolder : MonoBehaviour
{
    [SerializeField] private Card selectedCard;
    [SerializeReference] private Card hoveredCard;

    [SerializeField] private SlotCard slotPrefab;

    [Header("Spawn Settings")]
    [SerializeField]
    private Vector2Int gridDimension = new(4, 4);

    public SlotCard[] slots;

    bool isCrossing = false;
    [SerializeField] private bool tweenCardReturn = true;


    private SlotCard GetSlotFromCard(Card card)
        => slots.FirstOrDefault(slot => slot.card == card);

    private void OnRectTransformDimensionsChange()
    {
        var grid = GetComponent<GridLayoutGroup>();
        var rect = GetComponent<RectTransform>();

        grid.cellSize = new Vector2(
            rect.rect.width / gridDimension.x,
            rect.rect.height / gridDimension.y
        );
    }

    void Start()
    {
        OnRectTransformDimensionsChange();

        for (var i = 0; i < gridDimension.x * gridDimension.y; i++)
        {
            var s = Instantiate(slotPrefab.gameObject, transform);
            s.name = $"[Slot] {i}";
        }

        slots = GetComponentsInChildren<SlotCard>().ToArray();

        var cardCount = 0;

        foreach (var card in slots.Select(slot => slot.card))
        {
            card.PointerEnterEvent.AddListener(CardPointerEnter);
            card.PointerExitEvent.AddListener(CardPointerExit);
            card.BeginDragEvent.AddListener(BeginDrag);
            card.EndDragEvent.AddListener(EndDrag);
            card.name = cardCount.ToString();
            cardCount++;
        }

        StartCoroutine(Frame());
    }

    private IEnumerator Frame()
    {
        yield return new WaitForSecondsRealtime(.1f);
        foreach (var t in slots.Where(t => t.CardVisual))
            t.CardVisual.UpdateIndex();
    }

    private void BeginDrag(Card card)
        => selectedCard = card;

    void EndDrag(Card card)
    {
        if (!selectedCard)
            return;

        SlotCard hoverSlot = null;
        var minDistance = float.MaxValue;

        foreach (var slot in slots)
        {
            var distance = Vector3.Distance(selectedCard.transform.position, slot.transform.position);
            if (distance > minDistance) continue;
            minDistance = distance;
            hoverSlot = slot;
        }

        foreach (var slot in slots)
            if (slot != hoverSlot && slot.card.isHovering)
                slot.card.OnPointerExit();

        if (minDistance < 5 && hoverSlot?.card != selectedCard)
            if (hoverSlot?.card)
            {
                Debug.Log($"Swapping {selectedCard.name} with {hoverSlot.card.name}");
                Swap(selectedCard, hoverSlot.card);
            }
            else
            {
                Debug.Log($"Moving {selectedCard.name} to {hoverSlot.name}");
                GetSlotFromCard(selectedCard).SetCard(null);
                hoverSlot?.SetCard(selectedCard);
            }


        selectedCard.transform.DOLocalMove(
            selectedCard.selected
                ? new Vector3(0, selectedCard.selectionOffset, 0)
                : Vector3.zero,
            tweenCardReturn
                ? .15f
                : 0
        ).SetEase(Ease.OutBack);

        var rect = GetComponent<RectTransform>();
        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        selectedCard = null;
    }

    void CardPointerEnter(Card card)
        => hoveredCard = card;

    void CardPointerExit(Card card)
        => hoveredCard = null;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete) && hoveredCard)
            Destroy(hoveredCard);

        if (Input.GetMouseButtonDown(1))
            foreach (var slot in slots)
                slot.card?.Deselect();

        if (selectedCard && !isCrossing)
        {
            SlotCard hoverSlot = null;
            var minDistance = float.MaxValue;

            foreach (var slot in slots)
            {
                var distance = Vector3.Distance(selectedCard.transform.position, slot.transform.position);
                if (distance > minDistance) continue;
                minDistance = distance;
                hoverSlot = slot;
            }

            foreach (var slot in slots)
                if (slot != hoverSlot && slot.card.isHovering)
                    slot.card.OnPointerExit();

            if (minDistance < 5 && hoverSlot?.card && !hoverSlot.card.isHovering)
                hoverSlot.card.OnPointerEnter();
        }
    }

    void Swap(Card focused, Card crossed)
    {
        isCrossing = true;

        var focusedParent = GetSlotFromCard(focused);
        var crossedParent = GetSlotFromCard(crossed);

        focusedParent.SetCard(crossed);
        crossedParent.SetCard(focused);

        isCrossing = false;

        if (!crossed.cardVisual)
            return;

        crossed.cardVisual.Swap(focused.transform.position.x > crossed.transform.position.x ? 1 : -1);

        foreach (var card in slots.Select(slot => slot.card))
            card?.cardVisual?.UpdateIndex();
    }

    public bool AddCardToGrid(SlotCard slot, Card card)
    {
        if (slot.card) return false;
        slot.SetCard(card);
        return true;
    }
}



    