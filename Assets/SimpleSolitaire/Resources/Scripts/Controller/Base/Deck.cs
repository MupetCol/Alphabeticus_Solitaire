using System;
using SimpleSolitaire.Model.Enum;
using System.Collections.Generic;
using System.Reflection;
using SimpleSolitaire.Model.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
    public abstract class Deck : MonoBehaviour, IPointerClickHandler
    {
        public CardLogic CardLogicComponent;
        public int DeckNum = 0;
        public DeckType Type = 0;
        public List<Card> CardsArray = new List<Card>();

        public bool HasCards => CardsArray.Count > 0;
        public int CardsCount => CardsArray.Count;

        [Space(5f)] [SerializeField] protected RectTransform Rect;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private GameManager _gameManagerComponent;

        /// <summary>
        /// Set up background image for deck <see cref="_backgroundImage"/>
        /// </summary>
        /// <param name="str">string name of deck</param>
        public void SetBackgroundImg(string str)
        {
            Sprite tempType = CardLogicComponent.LoadSprite(Public.PATH_TO_DECKS_IN_RESOURCES + str);
            _backgroundImage.sprite = tempType;
        }

        /// <summary>
        /// Show/Add in game  new card from pack.
        /// </summary>
        /// <param name="card"></param>
        public void PushCard(Card card, bool isDraggable = true, int cardStatus = 1)
        {
            card.Deck = this;
            card.IsDraggable = isDraggable;
            card.CardStatus = cardStatus;
            CardsArray.Add(card);
        }

        /// <summary>
        /// Show/Add in game new card array from pack.
        /// </summary>
        /// <param name="card"></param>
        public void PushCardArray(Card[] cardArray, bool isDraggable = true, int cardStatus = 1)
        {
            for (int i = 0; i < cardArray.Length; i++)
            {
                cardArray[i].Deck = this;
                cardArray[i].IsDraggable = isDraggable;
                cardArray[i].CardStatus = cardStatus;
                CardsArray.Add(cardArray[i]);
            }
        }

        public void PushCardArray(List<Card> cardArray, bool isDraggable = true, int cardStatus = 1)
        {
            for (int i = 0; i < cardArray.Count; i++)
            {
                cardArray[i].Deck = this;
                cardArray[i].IsDraggable = isDraggable;
                cardArray[i].CardStatus = cardStatus;
                CardsArray.Add(cardArray[i]);
            }
        }

        /// <summary>
        /// Return last card from pack.
        /// </summary>
        public Card Pop()
        {
            Card retCard = null;
            int count = CardsCount;
            if (count > 0)
            {
                retCard = CardsArray[count - 1];
                retCard.Deck = null;
                CardsArray.Remove(retCard);
            }

            return retCard;
        }

        public void Clear()
        {
            int count = CardsCount;
            if (count == 0)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                Card toRemove = CardsArray[i];
                toRemove.Deck = null;
            }

            CardsArray.Clear();
        }

        public void RemoveFromArray(List<Card> cardsArray)
        {
            int count = CardsCount;
            if (count == 0)
            {
                return;
            }

            for (int i = 0; i < cardsArray.Count; i++)
            {
                Card toRemove = cardsArray[i];
                if (CardsArray.Contains(toRemove))
                {
                    toRemove.Deck = null;
                    CardsArray.Remove(toRemove);
                }
            }
        }

        /// <summary>
        /// Get card array from pop.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Card[] PopFromCard(Card card)
        {
            int i = 0;
            int count = CardsArray.Count;
            while (i < count)
            {
                if ((Card)CardsArray[i] == card)
                {
                    break;
                }

                i++;
            }

            Card[] cardArray = new Card[count - i];
            int k = 0;
            for (int j = i; j < count; j++)
            {
                cardArray[count - i - 1 - (k++)] = Pop();
            }

            return cardArray;
        }

        public void RemoveCard(Card card)
        {
            int index = -1;
            for (int j = 0; j < CardsCount; j++)
            {
                if (card == CardsArray[j])
                {
                    index = j;
                    break;
                }
            }

            if (index != -1)
            {
                card.Deck = null;
                CardsArray.RemoveAt(index);
            }
        }

        public int CardsAmountFromCard(Card card)
        {
            int i = 0;
            int count = CardsCount;
            while (i < count)
            {
                if (CardsArray[i] == card)
                {
                    break;
                }

                i++;
            }

            return count - i;
        }

        /// <summary>
        /// Update card position in game by solitaire style
        /// </summary>
        /// <param name="firstTime">If it first game update</param>
        public abstract void UpdateCardsPosition(bool firstTime);

        /// <summary>
        /// After set positions <see cref="UpdateCardsPosition(bool)"/> game show for user available cards and not available.
        /// </summary>
        protected virtual void UpdateCardsActiveStatus()
        {
            int compareNum = 4;
            if (Type == DeckType.DECK_TYPE_ACE || Type == DeckType.DECK_TYPE_WASTE || Type == DeckType.DECK_TYPE_PACK)
            {
                if (HasCards)
                {
                    int j = 0;
                    if (Type == DeckType.DECK_TYPE_PACK)
                    {
                        compareNum = 2;
                    }

                    for (int i = CardsCount - 1; i >= 0; i--)
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
                for (int i = CardsCount - 1; i >= 0; i--)
                {
                    (CardsArray[i]).gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Set new position for card holder.
        /// </summary>
        /// <param name="card">Card for change position</param>
        /// <param name="x">Position by X axis</param>
        /// <param name="y">Position by Y axis</param>
        public virtual void SetPositionFromCard(Card card, float x, float y)
        {
            int i;
            for (i = 0; i < CardsCount; i++)
            {
                if (CardsArray[i] == card)
                {
                    break;
                }
            }

            var verticalSpace =
                CardLogicComponent.GetSpaceFromDictionary(DeckSpacesTypes.DECK_SPACE_VERTICAL_BOTTOM_OPENED);
            int m = 0;
            for (int j = i; j < CardsCount; j++)
            {
                (CardsArray[j]).SetPosition(new Vector3(x, y - m++ * verticalSpace, 0));
            }
        }

        /// <summary>
        /// Collect card on aceDeck.
        /// </summary>
        /// <param name="card">Card for collect.</param>
        public virtual void SetCardsToTop(Card card)
        {
            bool found = false;
            for (int i = 0; i < CardsCount; i++)
            {
                if (CardsArray[i] == card)
                {
                    found = true;
                }

                if (found)
                {
                    ((Card)CardsArray[i]).transform.SetAsLastSibling();
                }
            }
        }

        /// <summary>
        /// Get last card on aceDeck.
        /// </summary>
        /// <returns></returns>
        public Card GetTopCard()
        {
            if (HasCards)
            {
                return CardsArray[CardsCount - 1];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get previous last card on deck.
        /// </summary>
        /// <returns></returns>
        public Card GetPreviousFromCard(Card fromCard)
        {
            int index = CardsArray.IndexOf(fromCard);

            if (index >= 1)
            {
                return CardsArray[index - 1];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// If we can drop card to other card it will be true.
        /// </summary>
        /// <param name="card">Checking card</param>
        /// <returns>We can drop or no</returns>
        public abstract bool AcceptCard(Card card);

        private void OnDrawGizmos()
        {
            /*
            // Draw gizmos for card borders
            float cardInPercentageX = 0.9f;
            float cardInPercentageY = 0.95f;
            Card topCard = GetTopCard();
            float x1 = transform.position.x;
            float y1 = topCard == null ? transform.position.y : topCard.transform.position.y;
            float w = CardLogicComponent.DeckWidth;
            float h = CardLogicComponent.DeckHeight;
            Vector2 pivot = Rect != null ? Rect.pivot : Vector2.one / 2;
            float cardOffsetX = ((1 - cardInPercentageX) * w);
            float cardOffsetY = ((1 - cardInPercentageY) * h);
            
            Vector2 l1 = new Vector2(
                x1 - (pivot.x * w) + cardOffsetX,
                y1 + h * (1-pivot.y) - cardOffsetY
            );

            Vector2 r1 = new Vector2(
                x1 + w * (1 - pivot.x) - cardOffsetX,
                y1 - (pivot.y * h) + cardOffsetY
            );
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(l1, 10);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(r1, 10);
           */
        }

        /// <summary>
        /// If we drop our card on other card this method return true.
        /// </summary>
        /// <param name="card">Checking card</param>
        public virtual bool OverlapWithCard(Card card)
        {
            if (card.Deck == this)
            {
                return false;
            }

            Card topCard = GetTopCard();

            float cardInPercentageX = 0.9f;
            float cardInPercentageY = 0.95f;
            float w = CardLogicComponent.DeckWidth;
            float h = CardLogicComponent.DeckHeight;
            Vector2 pivot = Rect != null ? Rect.pivot : Vector2.one / 2;
            float cardOffsetX = ((1 - cardInPercentageX) * w);
            float cardOffsetY = ((1 - cardInPercentageY) * h);
            Vector2 deckVector = topCard == null ? transform.position : topCard.transform.position;
            Vector2 cardVector = card.transform.position;

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


        public void OnPointerClick(PointerEventData eventData)
        {
            if (Type == DeckType.DECK_TYPE_PACK)
            {
                CardLogicComponent.OnClickPack();
            }
        }

        /// <summary>
        /// Initialize first game state.
        /// </summary>
        public virtual void RestoreInitialState()
        {
            for (int i = 0; i < CardsCount; i++)
            {
                Card card = CardsArray[i];
                card.RestoreBackView();
            }

            CardsArray.Clear();
        }

        public abstract void UpdateDraggableStatus();
        public abstract void UpdateBackgroundColor();
    }
}