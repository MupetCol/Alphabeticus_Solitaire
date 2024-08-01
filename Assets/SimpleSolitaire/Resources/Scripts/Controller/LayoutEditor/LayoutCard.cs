using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
    public class LayoutCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public RectTransform Rect;
        public Image Img;
        public CanvasGroup ContentGroup;
        public GameObject Selected;
        public Text Info;

        public Vector2 AnchoredPos => Rect.anchoredPosition;
        public bool IsSelected => Selected.activeInHierarchy;

        public CardPositionInfo CardInfo;
        private bool _isDragging;

        public void Init(CardPositionInfo info)
        {
            CardInfo = info;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            if (!IsSelected)
            {
                LayoutCreator.Instance.SelectCard(this);
                return;
            }

            LayoutCreator.Instance.SetTransport(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            LayoutCreator.Instance.ResetTransport();

            _isDragging = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isDragging)
            {
                return;
            }

            LayoutCreator.Instance.SelectCard(this);
        }

        public void UpdateInfo()
        {
            if (CardInfo == null)
                return;

            Info.text = CardInfo.ToInterpolatedFormat;
        }

        public void Select()
        {
            Selected.SetActive(true);
        }

        public void Deselect()
        {
            Selected.SetActive(false);
        }

        public void SetPreviewMode(bool state)
        {
            ContentGroup.alpha = state ? 0 : 1;
        }

        public void SetVisible(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}