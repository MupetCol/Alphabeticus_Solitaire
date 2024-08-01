using System.Collections;
using SimpleSolitaire.Model;
using SimpleSolitaire.Model.Enum;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public class PyramidHintManager : HintManager
    {
        private PyramidCardLogic Logic => _cardLogicComponent as PyramidCardLogic;

        protected override IEnumerator HintTranslate(HintData data)
        {
            IsHintProcess = true;

            List<HintElement> hints = data.Type == HintType.AutoComplete ? AutoCompleteHints : Hints;
            if (data.Type == HintType.AutoComplete) CurrentHintIndex = 0;
            if (data.Card != null) CurrentHintIndex = hints.FindIndex(x => x.HintCard == data.Card);

            if (data.Card != null && CurrentHintIndex == -1)
            {
                AudioController audioCtrl = AudioController.Instance;
                if (audioCtrl != null)
                {
                    audioCtrl.Play(AudioController.AudioType.Error);
                }

                Debug.LogWarning("After double tap! This Card: " + data.Card.CardNumber +
                                 " is not available for complete to ace pack.");
                IsHintProcess = false;
                CurrentHintIndex = 0;
                yield break;
            }

            var t = 0f;
            HintElement hint = hints[CurrentHintIndex];
            Card hintCard = hint.HintCard;
            hintCard.Deck.UpdateCardsPosition(false);

            CurrentHintSiblingIndex = hintCard.transform.GetSiblingIndex();

            hintCard.Deck.SetCardsToTop(hintCard);

            while (t < 1)
            {
                t += Time.deltaTime / data.HintTime;
                
                if (data.Type == HintType.Hint)
                {
                    if (hint.Type == HintAnimationType.Move)
                    {
                        hintCard.transform.position = Vector3.Lerp(hint.FromPosition,
                            hint.ToPosition, t);

                        yield return new WaitForEndOfFrame();
                        hintCard.Deck.SetPositionFromCard(hintCard,
                            hintCard.transform.position.x,
                            hintCard.transform.position.y);
                    }
                    else if (hint.Type == HintAnimationType.Click)
                    {
                        float scale = MathUtils.EvaluateQuadraticValue(0.2f, t);
                        hintCard.transform.localScale = new Vector3(scale, scale, scale);

                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    if (hint.Type == HintAnimationType.Move)
                    {
                        hintCard.transform.position = Vector3.Lerp(hint.FromPosition,
                            hint.ToPosition, t);

                        yield return new WaitForEndOfFrame();
                        hintCard.Deck.SetPositionFromCard(hintCard,
                            hintCard.transform.position.x,
                            hintCard.transform.position.y);
                    }
                    else if (hint.Type == HintAnimationType.Click)
                    {
                       break;
                    }
                }
            }

            hintCard.transform.localScale = Vector3.one;

            if (IsHasHint() && data.Type == HintType.Hint)
            {
                hintCard.Deck.UpdateCardsPosition(false);
                hintCard.transform.position = hints[CurrentHintIndex].FromPosition;
                hintCard.transform.SetSiblingIndex(CurrentHintSiblingIndex);
                CurrentHintIndex = CurrentHintIndex == hints.Count - 1 ? CurrentHintIndex = 0 : CurrentHintIndex + 1;
            }

            if (data.Type != HintType.Hint)
            {
                _cardLogicComponent.OnDragEnd(hintCard);
            }

            IsHintProcess = false;
        }

        /// <summary>
        /// Generate new hint depending on available for move cards.
        /// </summary>
        protected override void GenerateHints(bool isAutoComplete = false)
        {
            CurrentHintIndex = 0;
            AutoCompleteHints = new List<HintElement>();
            Hints = new List<HintElement>();

            if (IsAvailableForMoveCardArray.Count > 0)
            {
                foreach (var card in IsAvailableForMoveCardArray)
                {
                    if (card == null)
                    {
                        continue;
                    }

                    if (card.Number == 13)
                    {
                        Hints.Add(new HintElement(card, card.transform.position, card.transform.position, null, HintAnimationType.Click));
                        continue;
                    }

                    for (int i = 0; i < _cardLogicComponent.AllDeckArray.Length; i++)
                    {
                        Deck targetDeck = _cardLogicComponent.AllDeckArray[i];

                        if (targetDeck.Type == DeckType.DECK_TYPE_WASTE ||
                            targetDeck.Type == DeckType.DECK_TYPE_PACK)
                        {
                            Card topTargetDeckCard = targetDeck.GetTopCard();

                            if (targetDeck.AcceptCard(card))
                            {
                                Hints.Add(new HintElement(card, card.transform.position,
                                    topTargetDeckCard != null
                                        ? topTargetDeckCard.transform.position
                                        : targetDeck.transform.position, targetDeck));
                            }
                        }
                        else if (targetDeck.Type == DeckType.DECK_TYPE_PYRAMID)
                        {
                            var cards = new List<Card>(IsAvailableForMoveCardArray);
                            cards.Remove(card);
                            cards.RemoveAll(card => card.Deck.Type != DeckType.DECK_TYPE_PYRAMID);

                            foreach (var pyramidCard in cards)
                            {
                                PyramidCard pCard = pyramidCard as PyramidCard;
                                PyramidDeck pyramidDeck = pyramidCard.Deck as PyramidDeck;

                                if (pyramidDeck.AcceptCard(pCard, card))
                                {
                                    Hints.Add(new HintElement(card, card.transform.position,
                                        pCard.transform.position, targetDeck));
                                }
                            }
                        }
                    }
                }
            }

            ActivateHintButton(IsHasHint());
            ActivateAutoCompleteHintButton(IsHasAutoCompleteHint());
        }
    }
}