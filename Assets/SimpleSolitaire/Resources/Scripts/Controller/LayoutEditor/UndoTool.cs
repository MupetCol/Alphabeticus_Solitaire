using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    [System.Serializable]
    public class UndoToolState
    {
        public List<CardPositionInfo> State = new List<CardPositionInfo>();

        public UndoToolState(List<CardPositionInfo> infos)
        {
            State = new List<CardPositionInfo>();
            infos.ForEach(x => State.Add(new CardPositionInfo(x)));
        }
    }

    public class UndoTool : MonoBehaviour
    {
        public List<UndoToolState> States;

        public void RegisterState(List<CardPositionInfo> positionsState)
        {
            if (States == null)
            {
                States = new List<UndoToolState>();
            }

            States.Add(new UndoToolState(positionsState));
        }

        public void Undo()
        {
            if (States == null || States.Count == 0)
            {
                Debug.LogError("Has no states to undo.");
                return;
            }

            LayoutCreator.Instance.LoadPositionsInLayout(States[^1].State);
            States.RemoveAt(States.Count - 1);
        }

        public void Clear()
        {
            States = null;
        }
    }
}