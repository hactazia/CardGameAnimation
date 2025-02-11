using UnityEngine;

public class SlotCard : MonoBehaviour
{
    public Card card;

    public CardVisual CardVisual => card?.cardVisual;

    public void SetCard(Card c)
    {
        card = c;
        if (!c) return;
        c.transform.SetParent(transform);
        c.transform.localPosition = c.selected
            ? new Vector3(0, c.selectionOffset, 0)
            : Vector3.zero;
    }
}