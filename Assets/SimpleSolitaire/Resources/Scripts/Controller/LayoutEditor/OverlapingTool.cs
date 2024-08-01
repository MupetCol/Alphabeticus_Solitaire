using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public class OverlapingTool : MonoBehaviour
    {
        public Sprite OverlappedSprite;
        public Sprite NotOverlappedSprite;

        public Color OverlappedInfoColor;
        public Color NotOverlappedInfoColor;
        
        public RectTransform CorrectlyDeckRect;
        
        private float _deckWidth;
        private float _deckHeight;
        private Vector3[] _corners;

        public void Initialize()
        {
            _corners = new Vector3[4];
            CorrectlyDeckRect.GetWorldCorners(_corners);

            _deckHeight = _corners[2].y - _corners[0].y;
            _deckWidth = _corners[2].x - _corners[0].x;
        }

        public void UpdateOverlapsForCard(List<LayoutCard> cards = null)
        {
            List<LayoutCard> orderedCards = cards.OrderByDescending(x => x.CardInfo.Layer).ToList();

            for (int i = 0; i < orderedCards.Count; i++)
            {
                LayoutCard card = orderedCards[i];
                List<LayoutCard> cardsForCheck = new List<LayoutCard>(orderedCards);
                cardsForCheck.Remove(card);

                for (int j = 0; j < cardsForCheck.Count; j++)
                {
                    var otherCard = cardsForCheck[j];

                    float cardInPercentageX = 0.9f;
                    float cardInPercentageY = 0.95f;
                    float w = _deckWidth;
                    float h = _deckHeight;
                    Vector2 pivot = card.Rect != null ? card.Rect.pivot : Vector2.one / 2;
                    float cardOffsetX = ((1 - cardInPercentageX) * w);
                    float cardOffsetY = ((1 - cardInPercentageY) * h);
                    Vector2 deckVector = otherCard.transform.position;
                    Vector2 cardVector = card.transform.position;

                    Vector2 l1 = new Vector2(
                        deckVector.x - (pivot.x * w) + cardOffsetX,
                        deckVector.y + h * (1 - pivot.y) - cardOffsetY
                    );

                    Vector2 r1 = new Vector2(
                        deckVector.x + w * (1 - pivot.x) - cardOffsetX,
                        deckVector.y - (pivot.y * h) + cardOffsetY
                    );

                    Vector2 l2 = new Vector2(
                        cardVector.x - (pivot.x * w) + cardOffsetX,
                        cardVector.y + h * (1 - pivot.y) - cardOffsetY
                    );

                    Vector2 r2 = new Vector2(
                        cardVector.x + w * (1 - pivot.x) - cardOffsetX,
                        cardVector.y - (pivot.y * h) + cardOffsetY
                    );

                    float epsilon = 0.0001f;

                    bool isOverlapping = true;

                    if (Mathf.Abs(l1.x - r1.x) < epsilon || Mathf.Abs(l1.y - r1.y) < epsilon ||
                        Mathf.Abs(r2.x - l2.x) < epsilon || Mathf.Abs(l2.y - r2.y) < epsilon)
                    {
                        isOverlapping = false;
                    }

                    if (l1.x > r2.x || l2.x > r1.x)
                    {
                        isOverlapping = false;
                    }

                    if (r1.y > l2.y || r2.y > l1.y)
                    {
                        isOverlapping = false;
                    }

                    if (isOverlapping)
                    {
                        if (otherCard.CardInfo.OverlapsId == null)
                        {
                            otherCard.CardInfo.OverlapsId = new List<int>();
                        }

                        if (otherCard.CardInfo.Layer - 1 == card.CardInfo.Layer && !otherCard.CardInfo.OverlapsId.Contains(card.CardInfo.Id))
                        {
                            otherCard.CardInfo.OverlapsId.Add(card.CardInfo.Id);
                        }
                    }
                }
            }
        }

        public void VisualizeOverlappedCards(List<LayoutCard> cards)
        {
            if (cards == null || !cards.Any())
            {
                return;
            }

            cards.ForEach(Visualize);

            void Visualize(LayoutCard card)
            {
                bool isOverlappedByAny = card.CardInfo.OverlapsId != null && card.CardInfo.OverlapsId.Any();

                card.Img.sprite = isOverlappedByAny ? OverlappedSprite : NotOverlappedSprite;
                card.Info.color = isOverlappedByAny ? OverlappedInfoColor : NotOverlappedInfoColor;
                card.UpdateInfo();
            }
        }

        public void RemoveCardOverlapping(List<LayoutCard> cards, int cardId)
        {
            if (cards == null || !cards.Any())
            {
                return;
            }

            cards.ForEach(x =>
            {
                if (x.CardInfo.OverlapsId != null && x.CardInfo.OverlapsId.Contains(cardId))
                {
                    x.CardInfo.OverlapsId.Remove(cardId);
                }
            });
        }

        public void ClearCardOverlaps(LayoutCard card)
        {
            if (card == null)
            {
                return;
            }

            card.CardInfo.OverlapsId = new List<int>();
            card.UpdateInfo();
        }
    }
}