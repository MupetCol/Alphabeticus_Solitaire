using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    [CreateAssetMenu(fileName = "TripeaksLayoutContainer", menuName = "Tripeaks/TripeaksLayoutContainer")]
    public class TripeaksLayoutContainer : BaseLayoutsContainer<TripeaksLayoutData>
    {
        public override TripeaksLayoutData CurrentLayout { get; set; }
        public override List<TripeaksLayoutData> Layouts => _layouts;

        [SerializeField] private List<TripeaksLayoutData> _layouts;

        public override string LayoutsKey => "TripeaksLayouts";

        public override TripeaksLayoutData CreateNewPuzzle()
        {
            if (_layouts == null)
            {
                _layouts = new List<TripeaksLayoutData>();
            }

            var created = new TripeaksLayoutData()
            {
                LayoutId = _layouts.Count + 1,
                IsDefault = _layouts.Count == 0,
                Infos = new List<CardPositionInfo>(),
                Preview = null
            };

            _layouts.Add(created);

            return created;
        }
    }
}