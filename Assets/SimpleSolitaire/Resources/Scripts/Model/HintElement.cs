using SimpleSolitaire.Controller;
using UnityEngine;

namespace SimpleSolitaire.Model
{
    public enum HintAnimationType
    {
        Move = 0,
        Click = 1,
    }

    [System.Serializable]
    public class HintElement
    {
        public Card HintCard;
        public Deck DestinationDeck;
        public Vector3 FromPosition;
        public Vector3 ToPosition;
        public HintAnimationType Type;
        
        public HintElement(Card hintCard, Vector3 fromPosition, Vector3 toPosition, Deck destinationDeck, HintAnimationType type = HintAnimationType.Move)
        {
            HintCard = hintCard;
            FromPosition = fromPosition;
            ToPosition = toPosition;
            DestinationDeck = destinationDeck;
            Type = type;
        }
    }
}