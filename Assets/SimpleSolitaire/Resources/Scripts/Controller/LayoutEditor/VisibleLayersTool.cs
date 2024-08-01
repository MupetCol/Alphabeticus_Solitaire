using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
    public class VisibleLayersTool : MonoBehaviour
    {
        [SerializeField] private RectTransform _layersContainer;
        [SerializeField] private VisibleLayerItem _layerItem;
        [SerializeField] private Button _showLayersBtn;
        [SerializeField] private Text _showLayersBtnTxt;

        private Dictionary<int, bool> _layers = new Dictionary<int, bool>();
        private List<VisibleLayerItem> _items = new List<VisibleLayerItem>();

        public event Action<List<int>> OnVisibleLayersChanged;
        public event Action<List<int>> OnRegenerateLayers;

        private readonly string _showLayersLabel = "SHOW LAYERS";
        private readonly string _hideLayersLabel = "HIDE LAYERS";

        public void Initialize()
        {
            _showLayersBtn.interactable = false;
            _showLayersBtnTxt.text = _showLayersLabel;

            if (_showLayersBtn != null)
                _showLayersBtn.onClick.AddListener(OnShowLayersBtnClicked);
        }

        public void Deinitialize()
        {
            if (_showLayersBtn != null)
                _showLayersBtn.onClick.RemoveListener(OnShowLayersBtnClicked);
        }

        private void OnShowLayersBtnClicked()
        {
            bool isActive = !_layersContainer.gameObject.activeSelf;
            _layersContainer.gameObject.SetActive(isActive);
            _showLayersBtnTxt.text = isActive ? _hideLayersLabel : _showLayersLabel;
        }

        public void RegenerateLayers(List<LayoutCard> cards, bool isNewLayout = false)
        {
            if (cards == null || cards.Count < 0)
            {
                Debug.LogError("Has no card for change layer. Select first.");
                _showLayersBtn.interactable = false;
                return;
            }

            _showLayersBtn.interactable = true;

            foreach (var child in _items)
            {
                Destroy(child.gameObject);
            }

            _items.Clear();

            if (isNewLayout)
            {
                _layers.Clear();
            }

            HashSet<int> currentLayers = cards.Select(card => card.CardInfo.Layer).ToHashSet();

            _layers.Keys.Except(currentLayers).ToList().ForEach(layer => _layers.Remove(layer));
            foreach (int layer in currentLayers)
            {
                _layers.TryAdd(layer, true);
            }

            _layers = _layers.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            GenerateLayers();
        }

        private void GenerateLayers()
        {
            for (int i = 0; i < _layers.Count; i++)
            {
                var data = _layers.ElementAt(i);
                OnNewLayerAdded(data.Key, data.Value);
            }

            OnRegenerateLayers?.Invoke(_layers.Where(x => x.Value).Select(x => x.Key).ToList());
        }

        private void OnNewLayerAdded(int layer, bool active)
        {
            VisibleLayerItem item = Instantiate(_layerItem, _layersContainer);
            item.gameObject.SetActive(true);
            item.Initialize(new VisibleLayerData() { Layer = layer, Active = active });
            item.OnClick += () =>
            {
                if (_layers.TryGetValue(layer, out bool value))
                {
                    _layers[layer] = !value;
                    item.ActivateCheckmark(_layers[layer]);
                    OnVisibleLayersChanged?.Invoke(_layers.Where(x => x.Value).Select(x => x.Key).ToList());
                }
            };
            _items.Add(item);
        }
    }
}