using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    [CreateAssetMenu(fileName = "HowToPlayDataContainer", menuName = "HowToPlay/HowToPlayDataContainer")]
    public class HowToPlayDataContainer : ScriptableObject
    {
        public List<HowToPlayData> Pages = new List<HowToPlayData>();
    }
}