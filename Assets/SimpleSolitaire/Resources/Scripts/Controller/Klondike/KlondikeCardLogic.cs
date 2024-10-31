using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleSolitaire.Model.Config;
using SimpleSolitaire.Model.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
    public enum KlondikeDifficultyType
    {
        Easy = 0,
        Tricky = 1,
        Genius = 2,
    }

    public class KlondikeCardLogic : CardLogic
    {
        protected override int CardNums => Public.KLONDIKE_CARD_NUMS;

        public DeckRule TempRule { get; set; }
        public DeckRule CurrentRule;

        public KlondikeDifficultyType CurrentDifficultyType;
        public int DifficultyReplaceAmount = 16;

        [Header("Rule toggles:")] [SerializeField]
        private Toggle _oneDrawRuleToggle;

        [SerializeField] private Toggle _threeDrawRuleToggle;
        private KlondikeStatisticsController KlondikeStatisticsController => StatisticsComponent as KlondikeStatisticsController;

        [SerializeField] private bool _mobile;

        private void ChangeRuleTypeByToggle(DeckRule rule)
        {
            if (CurrentRule == rule) return;

            TempRule = rule;
        }

        public void InitRuleToggles()
        {
            TempRule = CurrentRule;

            _oneDrawRuleToggle.SetIsOnWithoutNotify(CurrentRule == DeckRule.ONE_RULE);
            _threeDrawRuleToggle.SetIsOnWithoutNotify(CurrentRule == DeckRule.THREE_RULE);
        }

        public override void InitCardLogic()
        {
            InitRuleToggles();

            base.InitCardLogic();
        }

        public void SetDifficulty(int difficulty)
        {
            CurrentDifficultyType = (KlondikeDifficultyType)difficulty;
        }

        protected override void GenerateRandomCardNums()
        {
            switch (CurrentDifficultyType)
            {
                case KlondikeDifficultyType.Genius:
                {
                    base.GenerateRandomCardNums();
                    break;
                }
                case KlondikeDifficultyType.Tricky:
                {
                    //Reorders cards on easier way, higher on waste deck, lower on rows
                    base.GenerateRandomCardNums();
                    int replaceAmount = DifficultyReplaceAmount;


                    int lastReplaceIndex = 0;
                    int bottomDeckCardsCounter = 28;
                    for (int i = CardNumberArray.Length - 1; i > 0; i--)
                    {
                        if (replaceAmount <= 0 || bottomDeckCardsCounter <= 0)
                        {
                            break;
                        }

                        bottomDeckCardsCounter--;

                        if (CardNumberArray[i] % 13 > 5)
                        {
                            replaceAmount--;

                            while (CardNumberArray[lastReplaceIndex] % 13 > 5)
                            {
                                if (lastReplaceIndex > CardNums)
                                {
                                    break;
                                }

                                lastReplaceIndex++;
                                replaceAmount--;
                            }

                            if (replaceAmount <= 0)
                            {
                                break;
                            }

                            int currentCardValue = CardNumberArray[i];
                            int cardForReplaceValue = CardNumberArray[lastReplaceIndex];

                            CardNumberArray[lastReplaceIndex] = currentCardValue;
                            CardNumberArray[i] = cardForReplaceValue;

                            /* Test debug.
                            Debug.LogError($"lastReplaceIndex {lastReplaceIndex} replaceAmount {replaceAmount} Replace {currentCardValue} with {cardForReplaceValue} ");
                            */
                        }
					}

					int aces = 3;
					int kings = 2;
					int[] fronPosIndex = { 51, 49, 46, 42, 37, 31, 24 };
					List<int> replacedOnes = new List<int>();

					for (int i = CardNumberArray.Length - 1; i > 0; i--)
					{
						if (aces <= 0)
						{
							break;
						}

						if (replacedOnes.Contains(i) || fronPosIndex.Contains(i))
						{
							//For already replaced aces or aces on valid positions
							continue;
						}


						if (CardNumberArray[i] == 0 || CardNumberArray[i] % 13 == 0)
						{
							aces--;

							int indexToReplace;
							while (true)
							{
								indexToReplace = fronPosIndex[UnityEngine.Random.Range(0, fronPosIndex.Length)];
								if (!replacedOnes.Contains(indexToReplace))
								{
									replacedOnes.Add(indexToReplace);
									break;
								}
							}

							int currentCardValue = CardNumberArray[i];
							int cardForReplaceValue = CardNumberArray[indexToReplace];

							/*Debug.Log("Replacing index " + i + "which is ace with value of" + CardNumberArray[i] +
								"with index " + indexToReplace + "with value of " + CardNumberArray[indexToReplace]);*/

							CardNumberArray[indexToReplace] = currentCardValue;
							CardNumberArray[i] = cardForReplaceValue;

							/* Test debug.
							Debug.LogError($"lastReplaceIndex {lastReplaceIndex} replaceAmount {replaceAmount} Replace {currentCardValue} with {cardForReplaceValue} ");
							*/
						}
					}

					for (int i = CardNumberArray.Length - 1; i > 0; i--)
					{
						if (kings <= 0)
						{
							break;
						}

						if (replacedOnes.Contains(i) || fronPosIndex.Contains(i))
						{
							//For already replaced aces or aces on valid positions
							continue;
						}


						if (CardNumberArray[i] == 12 || CardNumberArray[i] % 13 == 12)
						{
							kings--;

							int indexToReplace;
							while (true)
							{
								indexToReplace = fronPosIndex[UnityEngine.Random.Range(0, fronPosIndex.Length)];
								if (!replacedOnes.Contains(indexToReplace))
								{
									replacedOnes.Add(indexToReplace);
									break;
								}
							}

							int currentCardValue = CardNumberArray[i];
							int cardForReplaceValue = CardNumberArray[indexToReplace];

							/*Debug.Log("Replacing index " + i + "which is ace with value of" + CardNumberArray[i] +
								"with index " + indexToReplace + "with value of " + CardNumberArray[indexToReplace]);*/

							CardNumberArray[indexToReplace] = currentCardValue;
							CardNumberArray[i] = cardForReplaceValue;

							/* Test debug.
							Debug.LogError($"lastReplaceIndex {lastReplaceIndex} replaceAmount {replaceAmount} Replace {currentCardValue} with {cardForReplaceValue} ");
							*/
						}
					}

					break;
                }
				case KlondikeDifficultyType.Easy:
				{
                    //Same as tricky with the addition of two granted aces on the first pos of two rows
					base.GenerateRandomCardNums();
					int replaceAmount = DifficultyReplaceAmount;


					int lastReplaceIndex = 0;
					int bottomDeckCardsCounter = 28;
					for (int i = CardNumberArray.Length - 1; i > 0; i--)
					{
						if (replaceAmount <= 0 || bottomDeckCardsCounter <= 0)
						{
							break;
						}

						bottomDeckCardsCounter--;

						if (CardNumberArray[i] % 13 > 5)
						{
							replaceAmount--;

							while (CardNumberArray[lastReplaceIndex] % 13 > 5)
							{
								if (lastReplaceIndex > CardNums)
								{
									break;
								}

								lastReplaceIndex++;
								replaceAmount--;
							}

							if (replaceAmount <= 0)
							{
								break;
							}

							int currentCardValue = CardNumberArray[i];
							int cardForReplaceValue = CardNumberArray[lastReplaceIndex];

							CardNumberArray[lastReplaceIndex] = currentCardValue;
							CardNumberArray[i] = cardForReplaceValue;

							/* Test debug.
							Debug.LogError($"lastReplaceIndex {lastReplaceIndex} replaceAmount {replaceAmount} Replace {currentCardValue} with {cardForReplaceValue} ");
							*/
						}
					}

					int aces = 4;
                    int kings = 2;
					int[] fronPosIndex = { 51, 49, 46, 42, 37, 31, 24 };
					List<int> replacedOnes = new List<int>();

					for (int i = CardNumberArray.Length - 1; i > 0; i--)
					{
						if (aces <= 0)
						{
							break;
						}

						if (replacedOnes.Contains(i) || fronPosIndex.Contains(i))
						{
							//For already replaced aces or aces on valid positions
							continue;
						}


						if (CardNumberArray[i] == 0 || CardNumberArray[i] % 13 == 0)
						{
							aces--;

							int indexToReplace;
							while (true)
							{
								indexToReplace = fronPosIndex[UnityEngine.Random.Range(0, fronPosIndex.Length)];
								if (!replacedOnes.Contains(indexToReplace))
								{
									replacedOnes.Add(indexToReplace);
									break;
								}
							}

							int currentCardValue = CardNumberArray[i];
							int cardForReplaceValue = CardNumberArray[indexToReplace];

							/*Debug.Log("Replacing index " + i + "which is ace with value of" + CardNumberArray[i] +
								"with index " + indexToReplace + "with value of " + CardNumberArray[indexToReplace]);*/

							CardNumberArray[indexToReplace] = currentCardValue;
							CardNumberArray[i] = cardForReplaceValue;

							/* Test debug.
							Debug.LogError($"lastReplaceIndex {lastReplaceIndex} replaceAmount {replaceAmount} Replace {currentCardValue} with {cardForReplaceValue} ");
							*/
						}
					}

					for (int i = CardNumberArray.Length - 1; i > 0; i--)
					{
						if (kings <= 0)
						{
							break;
						}

						if (replacedOnes.Contains(i) || fronPosIndex.Contains(i))
						{
							//For already replaced aces or aces on valid positions
							continue;
						}


						if (CardNumberArray[i] == 12 || CardNumberArray[i] % 13 == 12)
						{
							kings--;

							int indexToReplace;
							while (true)
							{
								indexToReplace = fronPosIndex[UnityEngine.Random.Range(0, fronPosIndex.Length)];
								if (!replacedOnes.Contains(indexToReplace))
								{
									replacedOnes.Add(indexToReplace);
									break;
								}
							}

							int currentCardValue = CardNumberArray[i];
							int cardForReplaceValue = CardNumberArray[indexToReplace];

							/*Debug.Log("Replacing index " + i + "which is ace with value of" + CardNumberArray[i] +
								"with index " + indexToReplace + "with value of " + CardNumberArray[indexToReplace]);*/

							CardNumberArray[indexToReplace] = currentCardValue;
							CardNumberArray[i] = cardForReplaceValue;

							/* Test debug.
							Debug.LogError($"lastReplaceIndex {lastReplaceIndex} replaceAmount {replaceAmount} Replace {currentCardValue} with {cardForReplaceValue} ");
							*/
						}
					}

					break;
				}
			}
        }

        public override void InitializeSpacesDictionary()
        {
            base.InitializeSpacesDictionary();

            if (_mobile)
            {
				SpacesDict.Add(DeckSpacesTypes.DECK_SPACE_VERTICAL_BOTTOM_OPENED, DeckHeight / 3.5f);
				SpacesDict.Add(DeckSpacesTypes.DECK_SPACE_VERTICAL_BOTTOM_CLOSED, DeckHeight / 3.5f / 2);
				SpacesDict.Add(DeckSpacesTypes.DECK_SPACE_HORIONTAL_WASTE, DeckHeight / 3.5f / 1.8f);
			}
            else
            {
				SpacesDict.Add(DeckSpacesTypes.DECK_SPACE_VERTICAL_BOTTOM_OPENED, DeckHeight / 3.5f / 1.8f);
				SpacesDict.Add(DeckSpacesTypes.DECK_SPACE_VERTICAL_BOTTOM_CLOSED, DeckHeight / 3.5f / 2);
				SpacesDict.Add(DeckSpacesTypes.DECK_SPACE_HORIONTAL_WASTE, DeckHeight / 3.5f / 1.8f);
			}
        }

        public override void SubscribeEvents()
        {
            _oneDrawRuleToggle.onValueChanged.AddListener(delegate { ChangeRuleTypeByToggle(DeckRule.ONE_RULE); });
            _threeDrawRuleToggle.onValueChanged.AddListener(delegate { ChangeRuleTypeByToggle(DeckRule.THREE_RULE); });
        }

        public override void UnsubscribeEvents()
        {
            _oneDrawRuleToggle.onValueChanged.RemoveAllListeners();
            _threeDrawRuleToggle.onValueChanged.RemoveAllListeners();
        }

        public override void OnNewGameStart()
        {
            CurrentRule = TempRule;
            KlondikeStatisticsController.InitRuleToggle(CurrentRule);
            IsGameStarted = true;
        }

        public void SetRuleImmediately(DeckRule rule)
        {
            TempRule = rule;
            CurrentRule = TempRule;
        }

        /// <summary>
        /// Initialize deck of cards.
        /// </summary>
        protected override void InitDeckCards()
        {
            for (int i = 0; i < BottomDeckArray.Length; i++)
            {
                Deck bottomDeck = BottomDeckArray[i];

                for (int j = 0; j < i + 1; j++)
                {
                    bottomDeck.PushCard(PackDeck.Pop());
                }

                bottomDeck.UpdateCardsPosition(true);
                bottomDeck.UpdateDraggableStatus();
            }

            PackDeck.UpdateCardsPosition(true);
            PackDeck.UpdateDraggableStatus();

            WasteDeck.UpdateCardsPosition(true);
        }

        /// <summary>
        /// Call when we drop card.
        /// </summary>
        /// <param name="card">Dropped card</param>
        public override Task OnDragEnd(Card card)
        {
            bool isPackWasteNotFound = false;
            bool isHasTarget = false;
            for (int i = 0; i < AllDeckArray.Length; i++)
            {
                Deck targetDeck = AllDeckArray[i];
                if (targetDeck.Type == DeckType.DECK_TYPE_BOTTOM || targetDeck.Type == DeckType.DECK_TYPE_ACE)
                {
                    if (targetDeck.OverlapWithCard(card))
                    {
                        isHasTarget = true;
                        Deck srcDeck = card.Deck;

                        if (targetDeck.AcceptCard(card))
                        {
                            WriteUndoState();
                            Card[] popCards = srcDeck.PopFromCard(card);
                            targetDeck.PushCardArray(popCards);
                            targetDeck.UpdateCardsPosition(false);
                            srcDeck.UpdateCardsPosition(false);

                            ActionAfterEachStep();

                            if (targetDeck.Type == DeckType.DECK_TYPE_ACE)
                            {
                                GameManagerComponent.AddScoreValue(Public.SCORE_MOVE_TO_ACE);
                                if (AudioCtrl != null)
                                {
                                    AudioCtrl.Play(AudioController.AudioType.MoveToAce);
                                }
                            }
                            else
                            {
                                if (AudioCtrl != null)
                                {
                                    AudioCtrl.Play(AudioController.AudioType.Move);
                                }
                            }

                            return Task.CompletedTask;
                        }
                    }
                }
                else
                {
                    isPackWasteNotFound = true;
                }
            }

            if (isPackWasteNotFound &&
                (card.Deck.Type != DeckType.DECK_TYPE_PACK && card.Deck.Type != DeckType.DECK_TYPE_WASTE) ||
                isHasTarget)
            {
                if (AudioCtrl != null)
                {
                    AudioCtrl.Play(AudioController.AudioType.Error);
                }
            }

            return Task.CompletedTask;
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

            switch (CurrentRule)
            {
                case DeckRule.ONE_RULE:
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

                    break;
                case DeckRule.THREE_RULE:
                    for (int i = 0; i < 3; i++)
                    {
                        IsNeedResetPack = !PackDeck.HasCards;

                        if (IsNeedResetPack)
                        {
                            MoveFromWasteToPack();
                            IsNeedResetPack = false;
                            break;
                        }

                        if (PackDeck.HasCards)
                        {
                            WasteDeck.PushCard(PackDeck.Pop());
                            PackDeck.UpdateCardsPosition(false);
                            WasteDeck.UpdateCardsPosition(false);
                            IsNeedResetPack = !PackDeck.HasCards;
                            if (IsNeedResetPack) break;
                            if (AudioCtrl != null)
                            {
                                AudioCtrl.Play(AudioController.AudioType.MoveToWaste);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    break;
            }

            ActionAfterEachStep();
        }
    }
}