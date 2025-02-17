using System.Collections;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class HorizontalCardHolder : MonoBehaviour
{
    [SerializeField] private Card selectedCard;
    [SerializeReference] private Card hoveredCard;

    [SerializeField] private SlotCard slotPrefab;

    [Header("Spawn Settings")] [SerializeField]
    private int cardsToSpawn = 7;
    public int maxCards = 15;

    public List<SlotCard> slots;

    bool isCrossing = false;
    [SerializeField] private bool tweenCardReturn = true;

    void Start()
    {
        for (var i = 0; i < cardsToSpawn; i++)
            AddCard();
               
        StartCoroutine(Frame());
    }

    public void AddCard()
    {
        if (slots.Count > maxCards) return;
        var s = Instantiate(slotPrefab.gameObject, transform);
        s.name = $"[Slot] {slots.Count}";
        var slot = s.GetComponent<SlotCard>();
        slots.Add(slot);
        var card = slot.card;
        card.PointerEnterEvent.AddListener(CardPointerEnter);
        card.PointerExitEvent.AddListener(CardPointerExit);
        card.BeginDragEvent.AddListener(BeginDrag);
        card.EndDragEvent.AddListener(EndDrag);
    }

    public void ClearCards()
    {
        Debug.Log($"Clear: {slots.Count}");

        if (slots.Count < 1) 
            return;
        
        foreach(var slot in slots.ToArray())
            RemoveCard(slot); 
    }

    public void RemoveCard(Card card)
    {
        var slot = GetSlotFromCard(card);
        if (slot != null) RemoveCard(slot);
    }
    public void RemoveCard(SlotCard slot) {
        Debug.Log($"Remove: {slot}");
        slots.Remove(slot);
        Destroy(slot.gameObject);
    }

    private IEnumerator Frame()
    {
        yield return new WaitForSecondsRealtime(.1f);
        foreach (var t in slots.Where(t => t.CardVisual))
            t.CardVisual.UpdateIndex();
    }

    private SlotCard GetSlotFromCard(Card card)
        => slots.FirstOrDefault(slot => slot.card == card);

    private void BeginDrag(Card card)
        => selectedCard = card;

    void EndDrag(Card card)
    {
        if (!selectedCard) return;

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
            Destroy(GetSlotFromCard(hoveredCard));

        if (Input.GetMouseButtonDown(1))
            foreach (var card in slots.Where(slot => slot.card))
                card.card.Deselect();

        if (!selectedCard)
            return;

        if (isCrossing)
            return;

        for (var i = 0; i < slots.Count; i++)
        {
            var card = slots[i].card;
            if (selectedCard.transform.position.x > card.transform.position.x)
                if (selectedCard.ParentIndex() < card.ParentIndex())
                {
                    Swap(selectedCard, slots[i].card);
                    break;
                }

            if (selectedCard.transform.position.x < card.transform.position.x)
                if (selectedCard.ParentIndex() > card.ParentIndex())
                {
                    Swap(selectedCard, slots[i].card);
                    break;
                }
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
}