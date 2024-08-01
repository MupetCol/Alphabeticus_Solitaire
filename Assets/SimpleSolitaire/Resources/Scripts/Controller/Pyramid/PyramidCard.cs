using System.Collections.Generic;
using SimpleSolitaire.Model.Config;
using SimpleSolitaire.Model.Enum;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleSolitaire.Controller
{
    public class PyramidCard : Card
    {
        public CardPositionInfo Info = new CardPositionInfo();
        public bool OverlapsByAny => Info.OverlapsId != null && Info.OverlapsId.Count > 0;

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (!Application.isMobilePlatform && Application.isPlaying)
            {
                if (Info == null || Deck.Type != DeckType.DECK_TYPE_PYRAMID)
                {
                    return;
                }

                GUIStyle style = new GUIStyle(EditorStyles.textField) { normal = new GUIStyleState() { textColor = Color.blue }, fontStyle = FontStyle.Bold };

                Handles.Label(transform.position + Vector3.up * 100, $"{Info.Id}", style);
            }
#endif
        }


        /// <summary>
        /// Initialize card by number.
        /// </summary>
        /// <param name="cardNum">Card number.</param>
        public override void InitWithNumber(int cardNum)
        {
            CardNumber = cardNum;

            CardType = Mathf.FloorToInt(cardNum / Public.CARD_NUMS_OF_SUIT);

            if (CardType == 1 || CardType == 3)
            {
                CardColor = 1;
            }
            else
            {
                CardColor = 0;
            }

            Number = (cardNum % Public.CARD_NUMS_OF_SUIT) + 1;
            CardStatus = 0;

            var path = GetTexture();
            SetBackgroundImg(path);
        }

        /// <summary>
        ///Called when user click on card double times in specific interval
        /// </summary>
        protected override void OnTapToPlace()
        {
            CardLogicComponent.HintManagerComponent.HintAndSetByClick(this);
        }

        protected override void OnTapToPack(PointerEventData eventData)
        {
            if (CardLogicComponent.HintManagerComponent.IsHasHintForCard(this))
            {
                OnTapToPlace();
            }
            else
            {
                base.OnTapToPack(eventData);
            }
        }
    }

    public static class PyramidCardExtensions
    {
        public static bool OverlapsAlreadyInTrash(this PyramidCard card, List<int> idsInTrash)
        {
            bool result = true;

            for (int i = 0; i < card.Info.OverlapsId.Count; i++)
            {
                if (!idsInTrash.Contains(card.Info.OverlapsId[i]))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }
    }
}