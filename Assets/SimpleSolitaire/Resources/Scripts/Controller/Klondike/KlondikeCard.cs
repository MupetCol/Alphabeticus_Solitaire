﻿using SimpleSolitaire.Model.Config;
using TMPro;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public class KlondikeCard : Card
    {
     

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
    }
}