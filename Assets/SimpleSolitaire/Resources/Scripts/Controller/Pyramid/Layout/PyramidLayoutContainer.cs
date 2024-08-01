using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    [CreateAssetMenu(fileName = "PyramidLayoutContainer", menuName = "Pyramid/PyramidLayoutContainer")]
    public class PyramidLayoutContainer : BaseLayoutsContainer<PyramidLayoutData>
    {
        public override PyramidLayoutData CurrentLayout { get; set; }
        public override List<PyramidLayoutData> Layouts => _layouts;

        [SerializeField] private List<PyramidLayoutData> _layouts;

        public override string LayoutsKey => "PyramidLayouts";

        public override PyramidLayoutData CreateNewPuzzle()
        {
            if (Layouts == null)
            {
                _layouts = new List<PyramidLayoutData>();
            }

            var created = new PyramidLayoutData()
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