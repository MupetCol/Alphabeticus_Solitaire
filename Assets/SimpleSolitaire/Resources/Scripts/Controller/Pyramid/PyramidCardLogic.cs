using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleSolitaire.Model.Config;
using SimpleSolitaire.Model.Enum;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public class PyramidCardLogic : CardLogic
    {
        protected override int CardNums => Public.PYRAMID_CARD_NUMS;

        public Deck PyramidDeck;
        public Deck PyramidTrashDeck;
        public PyramidLayoutContainer LayoutContainer;
        private bool isFirstGeneration = true;

        public override void InitializeSpacesDictionary()
        {
            base.InitializeSpacesDictionary();

            SpacesDict.Add(DeckSpacesTypes.DECK_PACK_HORIZONTAL, -DeckWidth / 25f);
        }

        public override void InitCardLogic()
        {
            InitCurrentLayout();

            base.InitCardLogic();
        }

        public void InitCurrentLayout()
        {
        }

        public override void SubscribeEvents()
        {
        }

        public override void UnsubscribeEvents()
        {
        }

        public override void OnNewGameStart()
        {
            IsGameStarted = true;
        }
        
        public override void Shuffle(bool bReplay)
        {
            if (!bReplay)
            {
                if (isFirstGeneration && LayoutContainer.IsDefaultLayoutActive())
                {
                    LayoutContainer.SetDefaultLayout();
                    isFirstGeneration = false;
                }
                else
                {
                    LayoutContainer.SetRandomLayout();
                }
            }

            base.Shuffle(bReplay);
        }

        protected override void InitAllDeckArray()
        {
            int j = 0;

            if (PyramidDeck != null)
            {
                PyramidDeck.Type = DeckType.DECK_TYPE_PYRAMID;
                AllDeckArray[j++] = PyramidDeck;
            }

            if (PyramidTrashDeck != null)
            {
                PyramidTrashDeck.Type = DeckType.DECK_TYPE_PYRAMID_TRASH;
                AllDeckArray[j++] = PyramidTrashDeck;
            }

            if (WasteDeck != null)
            {
                WasteDeck.Type = DeckType.DECK_TYPE_WASTE;
                AllDeckArray[j++] = WasteDeck;
            }

            if (PackDeck != null)
            {
                PackDeck.Type = DeckType.DECK_TYPE_PACK;
                AllDeckArray[j++] = PackDeck;
            }

            for (int i = 0; i < AllDeckArray.Length; i++)
            {
                AllDeckArray[i].DeckNum = i;
            }
        }

        /// <summary>
        /// Initialize deck of cards.
        /// </summary>
        protected override void InitDeckCards()
        {
            if (WasteDeck != null)
            {
                WasteDeck.UpdateCardsPosition(true);
                WasteDeck.UpdateDraggableStatus();
            }

            if (PyramidDeck != null)
            {
                for (int j = 0; j < LayoutContainer.CurrentLayout.Infos.Count; j++)
                {
                    PyramidCard card = PackDeck.Pop() as PyramidCard;
                    CardPositionInfo layoutInfo = LayoutContainer.GetInfoByIndex(j);

                    card.Info = layoutInfo;
                    PyramidDeck.PushCard(card);
                }

                PyramidDeck.UpdateCardsPosition(true);
            }

            PyramidTrashDeck.UpdateCardsPosition(true);

            PackDeck.UpdateCardsPosition(true);
            PackDeck.UpdateDraggableStatus();
        }

        /// <summary>
        /// Call when we drop card.
        /// </summary>
        /// <param name="card">Dropped card</param>
        public override async Task OnDragEnd(Card card)
        {
            if (await OnDragEndKing(card) == false)
            {
                if (await OnDragEndDefault(card) == false)
                {
                    if (await OnDragEndWithPyramidDeck(card) == false)
                    {
                        if (AudioCtrl != null)
                        {
                            AudioCtrl.Play(AudioController.AudioType.Error);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Animated move using bezier curve method
        /// </summary>
        public async Task BezierMoveTo(Card target, Vector3 from, Vector3 to, Vector3 offset, float time = 0.5f)
        {
            target.transform.SetAsLastSibling();

            var t = 0f;

            while (t < 1)
            {
                t += Time.deltaTime / time;

                target.transform.position = MathUtils.EvaluateBezierValue(
                    from,
                    new Vector3(to.x + offset.x, from.y + offset.y),
                    to,
                    t);

                await Task.Yield();

                target.Deck.SetPositionFromCard(target,
                    target.transform.position.x,
                    target.transform.position.y);
            }
        }

        /// <summary>
        /// Used for King card when it was end dragging.
        /// </summary>
        private async Task<bool> OnDragEndKing(Card card)
        {
            if (card.Number != 13)
            {
                return false;
            }

            Deck srcDeck = card.Deck;

            WriteUndoState();

            srcDeck.RemoveCard(card);
            PyramidTrashDeck.PushCard(card, false);
            AudioCtrl.Play(AudioController.AudioType.MoveToAce);
            GameManagerComponent.AddScoreValue(Public.SCORE_MOVE_TO);

            await BezierMoveTo(card, card.transform.position, PyramidTrashDeck.transform.position, new Vector3(MathUtils.GetRandomBetween(-50, 50), MathUtils.GetRandomBetween(75, 125)));

            srcDeck.UpdateCardsPosition(false);
            PyramidTrashDeck.UpdateCardsPosition(false);

            ActionAfterEachStep();


            return true;
        }

        /// <summary>
        /// Used for other cards in pyramid deck when it was end dragging.
        /// </summary>
        private async Task<bool> OnDragEndWithPyramidDeck(Card card)
        {
            PyramidDeck targetDeck = PyramidDeck as PyramidDeck;
            if (targetDeck == null)
            {
                return false;
            }

            List<Card> cards = new List<Card>(targetDeck.CardsArray);
            cards.Remove(card);

            for (int i = 0; i < cards.Count; i++)
            {
                Card pyramidCard = cards[i];

                if (targetDeck.OverlapWithCard(card, pyramidCard))
                {
                    if (targetDeck.AcceptCard(card, pyramidCard))
                    {
                        Deck srcDeck = card.Deck;

                        WriteUndoState();

                        srcDeck.RemoveCard(card);
                        targetDeck.RemoveCard(pyramidCard);
                        PyramidTrashDeck.PushCardArray(new List<Card>() { pyramidCard, card }, false);
                        GameManagerComponent.AddScoreValue(Public.SCORE_MOVE_TO);
                        AudioCtrl.Play(AudioController.AudioType.MoveToAce);
                        
                        List<Task> animations = new List<Task>()
                        {
                            BezierMoveTo(pyramidCard, pyramidCard.transform.position, PyramidTrashDeck.transform.position, new Vector3(MathUtils.GetRandomBetween(-50, 50), MathUtils.GetRandomBetween(150, 200))),
                            BezierMoveTo(card, card.transform.position, PyramidTrashDeck.transform.position, new Vector3(MathUtils.GetRandomBetween(-50, 50), MathUtils.GetRandomBetween(75, 125)))
                        };

                        await Task.WhenAll(animations);

                        targetDeck.UpdateCardsPosition(false);
                        srcDeck.UpdateCardsPosition(false);
                        PyramidTrashDeck.UpdateCardsPosition(false);

                        ActionAfterEachStep();

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Used for other cards which placed in pack and waste decks when it was end dragging.
        /// </summary>
        private async Task<bool> OnDragEndDefault(Card card)
        {
            var decks = new List<Deck>() { WasteDeck, PackDeck };
            for (int i = 0; i < decks.Count; i++)
            {
                PyramidDeck targetDeck = decks[i] as PyramidDeck;
                if (targetDeck == null)
                {
                    continue;
                }

                if (targetDeck.Type == DeckType.DECK_TYPE_WASTE ||
                    targetDeck.Type == DeckType.DECK_TYPE_PACK)
                {
                    if (targetDeck.OverlapWithCard(card))
                    {
                        Deck srcDeck = card.Deck;

                        if (targetDeck.AcceptCard(card))
                        {
                            WriteUndoState();

                            srcDeck.RemoveCard(card);
                            PyramidCard targetDeckCard = targetDeck.Pop() as PyramidCard;

                            PyramidTrashDeck.PushCardArray(new List<Card>() { targetDeckCard, card }, false);
                            GameManagerComponent.AddScoreValue(Public.SCORE_MOVE_TO);
                            AudioCtrl.Play(AudioController.AudioType.MoveToAce);
                            
                            List<Task> animations = new List<Task>()
                            {
                                BezierMoveTo(targetDeckCard, targetDeckCard.transform.position, PyramidTrashDeck.transform.position,
                                    new Vector3(MathUtils.GetRandomBetween(-50, 50), MathUtils.GetRandomBetween(75, 125))),
                                BezierMoveTo(card, card.transform.position, PyramidTrashDeck.transform.position, new Vector3(MathUtils.GetRandomBetween(-50, 50), MathUtils.GetRandomBetween(150, 175)))
                            };

                            await Task.WhenAll(animations);

                            targetDeck.UpdateCardsPosition(false);
                            srcDeck.UpdateCardsPosition(false);
                            PyramidTrashDeck.UpdateCardsPosition(false);

                            ActionAfterEachStep();

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Call when we click on pack with cards.
        /// </summary>
        public override void OnClickPack()
        {
            if (!PackDeck.HasCards && !WasteDeck.HasCards)
            {
                if (AudioCtrl != null)
                {
                    AudioCtrl.Play(AudioController.AudioType.Error);
                }

                return;
            }

            WriteUndoState();

            IsNeedResetPack = PackDeck.CardsCount == 1;

            if (PackDeck.HasCards)
            {
                WasteDeck.PushCard(PackDeck.Pop());
                PackDeck.UpdateCardsPosition(false);
                WasteDeck.UpdateCardsPosition(false);
                if (AudioCtrl != null)
                {
                    AudioCtrl.Play(AudioController.AudioType.MoveToWaste);
                }
            }
            else
            {
                if (WasteDeck.HasCards)
                {
                    MoveFromWasteToPack();
                }
            }

            ActionAfterEachStep();
        }

        protected override void CheckWinGame()
        {
            bool hasWin = !PyramidDeck.HasCards;

            if (hasWin)
            {
                GameManagerComponent.HasWinGame();
                IsGameStarted = false;
            }
        }

        public bool CheckOverlapping(PyramidCard card) => card.OverlapsByAny;
    }
}