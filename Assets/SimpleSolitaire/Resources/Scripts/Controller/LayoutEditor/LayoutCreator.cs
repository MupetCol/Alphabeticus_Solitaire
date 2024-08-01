using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using SimpleSolitaire.Model.Config;
using Object = UnityEngine.Object;

namespace SimpleSolitaire.Controller
{
    public class LayoutCreator : MonoBehaviour
    {
        public static LayoutCreator Instance;

        [Header("Settings:")] public string RelativePreviewsFolderPath;
        public string PreviewsResourcesFolderPath;
        [Header("References:")] public Object ContainerObj;
        public ILayoutContainer<LayoutData> Container => ContainerObj as ILayoutContainer<LayoutData>;

        public LayoutCard Card;
        public RectTransform CardsContainer;

        public List<LayoutCard> Cards = new List<LayoutCard>();
        public Text CardInfo;

        [Header("Tools:")] public TransportTool Transport;
        public SelectTool Select;
        public LayerTool Layer;
        public PreviewTool Preview;
        public OverlapingTool Overlaping;
        public UndoTool Undo;
        public MoveAllTool MoveAll;
        public VisibleLayersTool VisibleLayers;

        public LayoutCard LastCard { get; private set; }

        [Header("Inputs:")] [SerializeField] private InputField _xInput;
        [SerializeField] private InputField _yInput;
        [SerializeField] private InputField _layerInput;
        [SerializeField] private InputField _layoutIdInput;
        [SerializeField] private InputField _moveAllInput;

        [Header("Outputs: ")] [SerializeField] private Text _currentLayoutText;

        [Header("Buttons:")] [SerializeField] private Button _removeCardBtn;
        [SerializeField] private Button _deselectAreaBtn;

        private LayoutData _tempLayoutData;
        private int _currentPuzzleId;

        private void Awake()
        {
            Instance = this;
            _tempLayoutData = null;
            SetCurrentPuzzleId(-1);

            Overlaping.Initialize();
            VisibleLayers.Initialize();
            VisibleLayers.OnVisibleLayersChanged += OnVisibleLayersChanged;
            VisibleLayers.OnRegenerateLayers += OnVisibleLayersChanged;

            if (_deselectAreaBtn != null)
                _deselectAreaBtn.onClick.AddListener(DeselectCard);
        }

        private void OnDestroy()
        {
            VisibleLayers.Deinitialize();
            VisibleLayers.OnVisibleLayersChanged -= OnVisibleLayersChanged;
            VisibleLayers.OnRegenerateLayers -= OnVisibleLayersChanged;

            if (_deselectAreaBtn != null)
                _deselectAreaBtn.onClick.RemoveListener(DeselectCard);
        }

        private void SetCurrentPuzzleId(int id)
        {
            _currentPuzzleId = id;
            _currentLayoutText.text = $"CURRENT LAYOUT: {_currentPuzzleId}";
        }

        public void Update()
        {
            UpdateLastCardInfo();
            UpdateLastCardDependencies();

            if (Input.GetKeyDown(KeyCode.P))
            {
                MakeLayoutPreview();
            }
        }

        public void SetTransport(LayoutCard card)
        {
            if (card == null)
            {
                return;
            }

            RegisterUndoState();
            LastCard = card;

            int lastSiblingIndexByLayer = 0;

            for (int i = Cards.Count - 1; i >= 0; i--)
            {
                if (Cards[i].CardInfo.Layer == LastCard.CardInfo.Layer)
                {
                    lastSiblingIndexByLayer = i;
                    break;
                }
            }

            Transport.SetCard(card, lastSiblingIndexByLayer);
        }

        public void SelectCard(LayoutCard card)
        {
            if (card == null)
            {
                return;
            }

            if (card == LastCard)
            {
                DeselectCard();
                return;
            }

            if (LastCard != null)
            {
                DeselectCard();
            }

            LastCard = card;

            Select.SelectCard(card);

            UpdateInputsValues();
        }

        private void DeselectCard()
        {
            Select.DeselectCard();
            LastCard = null;
        }

        public void ResetTransport()
        {
            Transport.ResetCard();
            Overlaping.ClearCardOverlaps(LastCard);

            UpdateInputsValues();
            UpdateOverlapping();

            for (int i = 0; i < Cards.Count; i++)
            {
                var card = Cards[i];
                card.transform.SetSiblingIndex(i);
            }
        }

        private void UpdateInputsValues()
        {
            float x = LastCard != null ? LastCard.CardInfo.AnchoredPos.X : 0;
            float y = LastCard != null ? LastCard.CardInfo.AnchoredPos.Y : 0;
            float layer = LastCard != null ? LastCard.CardInfo.Layer : 0;

            _xInput.text = x.ToString();
            _yInput.text = y.ToString();
            _layerInput.text = layer.ToString();
        }

        public void ResetLayout()
        {
            RegisterUndoState();
            InternalResetLayout();
            RegenerateVisibleLayers();
        }

        private void InternalResetLayout()
        {
            ClearLayout();

            _tempLayoutData.Infos.Clear();
            _tempLayoutData.Preview = null;
        }

        private void ClearLayout()
        {
            if (!Cards.Any())
            {
                Debug.LogError("Has no cards for refreshing.");
                return;
            }

            DeselectCard();
            for (int i = 0; i < Cards.Count; i++)
            {
                var card = Cards[i];

                Destroy(card.gameObject);
            }

            Cards.Clear();
        }

        public void AddCard()
        {
            if (Cards.Count >= Public.TRIPEAKS_CARD_NUMS)
            {
                Debug.LogError($"Cards limit reached. Maximum cards ampunt = {Public.TRIPEAKS_CARD_NUMS}.");
                return;
            }

            if (_tempLayoutData == null)
            {
                Debug.LogError($"Create layout at first.");
                return;
            }

            RegisterUndoState();

            LayoutCard card = Instantiate(Card, CardsContainer);

            if (card != null)
            {
                int maxId = Cards.Any() ? Cards.Select(x => x.CardInfo.Id).Max() : 0;
                int newId = maxId + 1;
                CardPositionInfo cardInfo = new CardPositionInfo(newId);
                card.Init(cardInfo);
                card.UpdateInfo();
                card.name = $"Card {cardInfo.Id}";
                if (!card.gameObject.activeInHierarchy)
                {
                    card.gameObject.SetActive(true);
                }

                Cards.Add(card);
                _tempLayoutData.Infos.Add(cardInfo);

                InternalSetLayer(card, shouldUpdateOverlaping: false);
                InternalSetPosition(card, new Vector2Int(0, 500), shouldUpdateOverlapping: false);
                UpdateOverlapping();
                RegenerateVisibleLayers();
            }
        }

        public void RemoveCard()
        {
            if (_tempLayoutData == null)
            {
                Debug.LogError($"Create layout at first.");
                return;
            }

            RegisterUndoState();

            if (LastCard == null)
            {
                Debug.LogError("Has no card for remove.");
                return;
            }

            var card = LastCard;

            UpdateOverlapping();

            _tempLayoutData.Infos.RemoveAll(x => x.Id == card.CardInfo.Id);
            Cards.RemoveAll(x => x.CardInfo.Id == card.CardInfo.Id);

            DeselectCard();
            Destroy(card.gameObject);
            RegenerateVisibleLayers();
        }

        public void UpdateLastCardInfo()
        {
            if (LastCard == null)
            {
                CardInfo.text = $"Please choose a card.";
                return;
            }

            CardInfo.text = LastCard.CardInfo.ToOneLineFormat;
        }

        public void UpdateLastCardDependencies()
        {
            if (LastCard == null)
            {
                if (_removeCardBtn.interactable)
                {
                    _removeCardBtn.interactable = false;
                }

                return;
            }

            if (!_removeCardBtn.interactable)
            {
                _removeCardBtn.interactable = true;
            }
        }

        public void SetPosition()
        {
            RegisterUndoState();

            InternalSetPosition(LastCard);
        }

        private void InternalSetPosition(LayoutCard card, Vector2Int? customPosition = null, bool shouldUpdateOverlapping = true)
        {
            if (_tempLayoutData == null)
            {
                Debug.LogError($"Create layout at first.");
                return;
            }

            if (!int.TryParse(_xInput.text, out int x))
            {
                x = 0;
            }

            if (!int.TryParse(_yInput.text, out int y))
            {
                y = 0;
            }

            Vector2Int position = customPosition ?? new Vector2Int(x, y);

            Transport.SetPosition(card, position);
            Overlaping.ClearCardOverlaps(card);
            if (shouldUpdateOverlapping)
            {
                UpdateOverlapping();
            }
        }

        public void SetLayer()
        {
            RegisterUndoState();

            var layer = InternalSetLayer(LastCard);

            RegenerateVisibleLayers();
        }

        public void ValidateLayers()
        {
            if (_tempLayoutData == null)
            {
                Debug.LogError($"Choose layout first.");
                return;
            }

            for (int i = 0; i < Cards.Count; i++)
            {
                var card = Cards[i];
                Overlaping.ClearCardOverlaps(card);
            }

            for (int i = 0; i < Cards.Count; i++)
            {
                var card = Cards[i];

                Layer.SetLayer(card, card.CardInfo.Layer);
                Layer.Reorganize(_tempLayoutData, ref Cards);
                Overlaping.RemoveCardOverlapping(Cards, card.CardInfo.Id);
                Overlaping.UpdateOverlapsForCard(Cards);
                Overlaping.VisualizeOverlappedCards(Cards);
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(ContainerObj);
#endif
        }

        private int InternalSetLayer(LayoutCard card, int? customLayer = null, bool shouldUpdateOverlaping = true)
        {
            if (_tempLayoutData == null)
            {
                Debug.LogError($"Create layout at first.");
                return -1;
            }

            if (!int.TryParse(_layerInput.text, out int layer))
            {
                layer = 0;
            }

            Layer.SetLayer(card, customLayer ?? layer);
            Layer.Reorganize(_tempLayoutData, ref Cards);
            Overlaping.ClearCardOverlaps(card);
            if (shouldUpdateOverlaping)
            {
                UpdateOverlapping();
            }

            return layer;
        }

        public void MakeLayoutPreview()
        {
            if (_tempLayoutData == null)
            {
                Debug.LogError($"Create layout at first.");
                return;
            }

            string pathToPreview = $"{RelativePreviewsFolderPath}Preview_{_tempLayoutData.LayoutId}.png";
            Preview.MakePreview(Cards, Application.dataPath + pathToPreview, OnPreviewCreated);

            void OnPreviewCreated(Texture2D texture)
            {
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif

                string spriteAssetPath = $"{PreviewsResourcesFolderPath}Preview_{_tempLayoutData.LayoutId}";
                Sprite sprite = Resources.Load<Sprite>(spriteAssetPath);
                _tempLayoutData.Preview = sprite;
            }
        }

        public void LoadLayout()
        {
            if (!int.TryParse(_layoutIdInput.text, out int id))
            {
                id = -1;
            }

            var data = Container.LoadLayout(id);
            if (data != null)
            {
                if (_tempLayoutData != null)
                {
                    ClearLayout();
                }

                _tempLayoutData = data;
                SetCurrentPuzzleId(id);

                SetCardsPositions(_tempLayoutData.Infos);

                Overlaping.VisualizeOverlappedCards(Cards);
                Undo.Clear();
                RegenerateVisibleLayers(true);
            }
            else
            {
                Debug.LogError($"Can't find layout with given id: {id}.");
            }
        }

        public void LoadPositionsInLayout(List<CardPositionInfo> positions)
        {
            if (_tempLayoutData == null)
            {
                return;
            }

            ClearLayout();

            SetCardsPositions(positions);
            _tempLayoutData.Infos = new List<CardPositionInfo>(positions);

            ValidateLayers();
            RegenerateVisibleLayers();
        }

        private void SetCardsPositions(List<CardPositionInfo> positions)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                LayoutCard card = Instantiate(Card, CardsContainer);
                if (card)
                {
                    CardPositionInfo cardInfo = positions[i];

                    card.Init(cardInfo);
                    card.UpdateInfo();

                    Transport.SetPosition(card, (Vector2Int)cardInfo.AnchoredPos.VectorPosInt);
                    Layer.SetLayer(card, cardInfo.Layer);

                    if (!card.gameObject.activeInHierarchy)
                    {
                        card.gameObject.SetActive(true);
                    }

                    Cards.Add(card);

                    card.name = $"Card {cardInfo.Id}";
                }
            }
        }

        public void MoveAllInDirection(int direction)
        {
            RegisterUndoState();

            if (!int.TryParse(_moveAllInput.text, out int value))
            {
                value = 0;
            }

            Vector2Int vector;

            switch ((MoveAllDirection)direction)
            {
                case MoveAllDirection.Left:
                    vector = Vector2Int.left * value;
                    break;
                case MoveAllDirection.Right:
                    vector = Vector2Int.right * value;
                    break;
                case MoveAllDirection.Up:
                    vector = Vector2Int.up * value;
                    break;
                case MoveAllDirection.Down:
                    vector = Vector2Int.down * value;
                    break;
                default:
                    vector = Vector2Int.zero;
                    break;
            }

            MoveAll.MoveAllCardsInDirection(Cards, vector);
        }

        public void CreateNewLayout()
        {
            _tempLayoutData = Container.CreateNewPuzzle();
            SetCurrentPuzzleId(_tempLayoutData.LayoutId);
            InternalResetLayout();
            RegenerateVisibleLayers(true);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(ContainerObj);
#endif
        }

        private void UpdateOverlapping()
        {
            if (LastCard == null)
            {
                return;
            }

            Overlaping.RemoveCardOverlapping(Cards, LastCard.CardInfo.Id);
            Overlaping.UpdateOverlapsForCard(Cards);
            Overlaping.VisualizeOverlappedCards(Cards);
        }

        private void RegisterUndoState()
        {
            Undo.RegisterState(new List<CardPositionInfo>(Cards.Select(x => x.CardInfo).ToList()));
        }

        private void RegenerateVisibleLayers(bool isNewLayout = false)
        {
            VisibleLayers.RegenerateLayers(Cards, isNewLayout);
        }

        private void OnVisibleLayersChanged(List<int> obj)
        {
            if (Cards == null || Cards.Count < 0)
            {
                return;
            }

            foreach (var card in Cards)
            {
                card.SetVisible(obj.Contains(card.CardInfo.Layer));
            }
        }   
    }
}