using System.Collections.Generic;
using System.Linq;
using SimpleSolitaire.Model.Config;
using SimpleSolitaire.Model.Enum;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public class PyramidDeck : Deck
    {
        private PyramidCardLogic _pyramidCardLogic => CardLogicComponent as PyramidCardLogic;

        protected override void UpdateCardsActiveStatus()
        {
            if (Type == DeckType.DECK_TYPE_WASTE || Type == DeckType.DECK_TYPE_PACK)
            {
                int compareNum = 2;

                if (HasCards)
                {
                    int j = 0;

                    for (int i = CardsArray.Count - 1; i >= 0; i--)
                    {
                        Card card = CardsArray[i];
                        if (j < compareNum)
                        {
                            card.gameObject.SetActive(true);
                            j++;
                        }
                        else
                        {
                            card.gameObject.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                for (int i = CardsArray.Count - 1; i >= 0; i--)
                {
                    CardsArray[i].gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Update card position in game by solitaire style
        /// </summary>
        /// <param name="firstTime">If it first game update</param>
        public override void UpdateCardsPosition(bool firstTime)
        {
            if (CardsCount == 0)
            {
                return;
            }

            for (int i = 0; i < CardsArray.Count; i++)
            {
                PyramidCard card = CardsArray[i] as PyramidCard;
                card.transform.SetAsLastSibling();
                if (Type == DeckType.DECK_TYPE_WASTE || Type == DeckType.DECK_TYPE_PACK || Type == DeckType.DECK_TYPE_PYRAMID_TRASH)
                {
                    card.gameObject.transform.position = gameObject.transform.position;
                    card.CardStatus = 1;
                    card.UpdateCardImg();
                }
                else if (Type == DeckType.DECK_TYPE_PYRAMID)
                {
                    card.CardRect.anchoredPosition = card.Info.AnchoredPos.VectorPos;
                    card.CardStatus = 1;
                    card.UpdateCardImg();
                }
                else if (Type == DeckType.DECK_TYPE_PYRAMID_TRASH)
                {
                    card.gameObject.transform.position = gameObject.transform.position;
                    card.CardStatus = 1;
                }
            }

            UpdateDraggableStatus();
            UpdateCardsActiveStatus();
        }


        /// <summary>
        /// If we can drop card to other card it will be true.
        /// </summary>
        /// <param name="card">Checking card</param>
        /// <returns>We can drop or no</returns>
        public override bool AcceptCard(Card card)
        {
            Card topCard = GetTopCard();
            switch (Type)
            {
                case DeckType.DECK_TYPE_WASTE:
                case DeckType.DECK_TYPE_PYRAMID:
                case DeckType.DECK_TYPE_PACK:
                    if (topCard != null)
                    {
                        if (topCard.Number + card.Number == Public.PYRAMID_ACCEPT_NUMBERS_SUM)
                        {
                            return true;
                        }
                    }

                    break;
            }

            return false;
        }

        /// <summary>
        /// Check overlapping for 2 cards.
        /// </summary>
        public bool OverlapWithCard(Card first, Card second)
        {
            float cardInPercentageX = 0.9f;
            float cardInPercentageY = 0.95f;
            float w = CardLogicComponent.DeckWidth;
            float h = CardLogicComponent.DeckHeight;
            Vector2 pivot = Rect != null ? Rect.pivot : Vector2.one / 2;
            float cardOffsetX = ((1 - cardInPercentageX) * w);
            float cardOffsetY = ((1 - cardInPercentageY) * h);
            Vector2 deckVector = first.transform.position;
            Vector2 cardVector = second.transform.position;

            Vector2 l1 = new Vector2(
                deckVector.x - (pivot.x * w) + cardOffsetX,
                deckVector.y + h * (1-pivot.y) - cardOffsetY
            );

            Vector2 r1 = new Vector2(
                deckVector.x + w * (1 - pivot.x) - cardOffsetX,
                deckVector.y - (pivot.y * h) + cardOffsetY
            );
            
            Vector2 l2 = new Vector2(
                cardVector.x - (pivot.x * w) + cardOffsetX,
                cardVector.y + h * (1-pivot.y) - cardOffsetY
            );

            Vector2 r2 = new Vector2(
                cardVector.x + w * (1 - pivot.x) - cardOffsetX,
                cardVector.y - (pivot.y * h) + cardOffsetY
            );

            float epsilon = 0.0001f;

            if (Mathf.Abs(l1.x - r1.x) < epsilon || Mathf.Abs(l1.y - r1.y) < epsilon ||
                Mathf.Abs(r2.x - l2.x) < epsilon || Mathf.Abs(l2.y - r2.y) < epsilon)
            {
                return false;
            }

            if (l1.x > r2.x || l2.x > r1.x)
            {
                return false;
            }

            if (r1.y > l2.y || r2.y > l1.y)
            {
                return false;
            }

            return true;
        }

        public bool AcceptCard(Card first, Card second)
        {
            switch (Type)
            {
                case DeckType.DECK_TYPE_WASTE:
                case DeckType.DECK_TYPE_PYRAMID:
                case DeckType.DECK_TYPE_PACK:
                    if (first.IsDraggable && second.IsDraggable &&
                        first.Number + second.Number == Public.PYRAMID_ACCEPT_NUMBERS_SUM)
                    {
                        return true;
                    }

                    break;
            }

            return false;
        }

        public override void UpdateDraggableStatus()
        {
            for (int i = 0; i < CardsArray.Count; i++)
            {
                PyramidCard card = CardsArray[i] as PyramidCard;

                if (Type == DeckType.DECK_TYPE_PYRAMID)
                {
                    List<int> overlapIdsInTrash = _pyramidCardLogic.PyramidTrashDeck.CardsArray.Select(x => ((PyramidCard)x).Info.Id).Distinct().ToList();
                    bool isOverlapped = _pyramidCardLogic.CheckOverlapping(card) && !card.OverlapsAlreadyInTrash(overlapIdsInTrash);
                    card.IsDraggable = !isOverlapped;

                    card.UpdateCardImg();
                }

                if (Type == DeckType.DECK_TYPE_PACK || Type == DeckType.DECK_TYPE_WASTE)
                {
                    card.IsDraggable = i == CardsArray.Count - 1;
                }

                if (Type == DeckType.DECK_TYPE_PYRAMID_TRASH)
                {
                    card.IsDraggable = false;
                }
            }

            UpdateBackgroundColor();
        }

        public override void UpdateBackgroundColor()
        {
            if (CardsCount == 0)
            {
                return;
            }

            if (Type == DeckType.DECK_TYPE_PYRAMID)
            {
                for (int i = 0; i < CardsArray.Count; i++)
                {
                    Card card = CardsArray[i];
                    var colorBg = card.IsDraggable || !CardLogicComponent.HighlightDraggable
                        ? CardLogicComponent.DraggableColor
                        : CardLogicComponent.NondraggableColor;
                    card.SetBackgroundColor(colorBg);
                }
            }
            else if (Type == DeckType.DECK_TYPE_ACE || Type == DeckType.DECK_TYPE_PACK)
            {
                for (int i = 0; i < CardsArray.Count; i++)
                {
                    Card card = CardsArray[i];
                    card.SetBackgroundColor(CardLogicComponent.DraggableColor);
                }
            }
        }

        public override void SetCardsToTop(Card card)
        {
            card.transform.SetAsLastSibling();
        }

        public override void SetPositionFromCard(Card card, float x, float y)
        {
            card.SetPosition(new Vector3(x, y, 0));
        }

        public override void RestoreInitialState()
        {
            for (int i = 0; i < CardsCount; i++)
            {
                PyramidCard card = CardsArray[i] as PyramidCard;
                card.Info = new CardPositionInfo();
                card.UpdateCardImg();
            }

            CardsArray.Clear();
        }
    }
}