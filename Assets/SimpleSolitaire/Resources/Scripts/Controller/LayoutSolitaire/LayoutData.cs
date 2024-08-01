using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    [System.Serializable]
    public class LayoutData
    {
        public int LayoutId;
        public bool IsDefault = false;
        public Sprite Preview;
        public List<CardPositionInfo> Infos;

        protected CardPositionInfo DefaultInfo = new CardPositionInfo(0, Vector3.zero);

        public CardPositionInfo GetInfoById(int id)
        {
            if (Infos == null)
            {
                return DefaultInfo;
            }

            CardPositionInfo info = DefaultInfo;

            for (int i = 0; i < Infos.Count; i++)
            {
                CardPositionInfo layoutInfo = Infos[i];

                if (Infos[i].Id == id)
                {
                    info = layoutInfo;
                    break;
                }
            }

            return info;
        }

        public CardPositionInfo GetInfoByIndex(int positionIndex)
        {
            if (Infos == null || Infos.Count - 1 < positionIndex)
            {
                return DefaultInfo;
            }

            return Infos[positionIndex];
        }
    }
}