using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    [System.Serializable]
    public class CardPositionInfo
    {
        public int Layer;
        public int Id;
        public CardPosition AnchoredPos;
        public List<int> OverlapsId;
        
        public CardPositionInfo()
        {
            Layer = 0;
            Id = 0;
            AnchoredPos = new CardPosition(Vector3.zero);
            OverlapsId = null;
        }

        public CardPositionInfo(int id)
        {
            Id = id;
            AnchoredPos = new CardPosition();
        }
        
        public CardPositionInfo(int id, Vector3 position)
        {
            Id = id;
            AnchoredPos = new CardPosition(position);
        }

        public CardPositionInfo(CardPositionInfo info)
        {
            Layer = info.Layer;
            Id = info.Id;
            AnchoredPos = new CardPosition(info.AnchoredPos);
            OverlapsId = info.OverlapsId;
        }
        
        public string ToOneLineFormat => $"ID:{Id} L:{Layer} X:{AnchoredPos.X} Y:{AnchoredPos.Y} O:{(OverlapsId != null ? string.Join("|", OverlapsId) : string.Empty)}";
        public string ToInterpolatedFormat => $"ID:{Id}\nL:{Layer}\nX:{AnchoredPos.X}\nY:{AnchoredPos.Y}\nO:{(OverlapsId != null ? string.Join("|", OverlapsId) : string.Empty)}";
    }
}