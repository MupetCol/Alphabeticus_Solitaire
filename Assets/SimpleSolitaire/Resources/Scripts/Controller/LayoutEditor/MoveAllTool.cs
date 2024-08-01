using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public enum MoveAllDirection
    {
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3
    }

    public class MoveAllTool : MonoBehaviour
    {
        public void MoveAllCardsInDirection(List<LayoutCard> cards, Vector2Int vector)
        {
            if (cards == null || !cards.Any())
            {
                Debug.LogError("Has no cards to move");
                return;
            }

            cards.ForEach(Move);

            void Move(LayoutCard card)
            {
                Vector2Int newPosition = new Vector2Int(card.CardInfo.AnchoredPos.VectorPosInt.x + vector.x, card.CardInfo.AnchoredPos.VectorPosInt.y + vector.y);
                card.Rect.anchoredPosition = newPosition;
                card.CardInfo.AnchoredPos = new CardPosition(newPosition);
                card.UpdateInfo();
            }
        }
    }
}