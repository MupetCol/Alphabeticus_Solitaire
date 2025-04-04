﻿using SimpleSolitaire.Model.Enum;
using System.Collections;
using SimpleSolitaire.Model.Config;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace SimpleSolitaire.Controller
{
    public abstract class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
		[SerializeField] private TMP_Text numberText;

		public int CardType = 0;
        public int CardNumber = 0;
        public int Number = 0;
        public int CardStatus = 0;
        public int CardColor = 0;
        public bool IsDraggable = false;

        public int IndexZ;

        public CardLogic CardLogicComponent;
        public Image BackgroundImage;
        public RectTransform CardRect;

        private Vector3 _lastMousePosition = Vector3.zero;
        private Vector3 _offset;
        private IEnumerator _coroutine;

        private Vector3 _newPosition;

        private bool _isDragging;
        private Deck _deck;

        private string _cachedSpritePath = string.Empty;

        public Deck Deck
        {
            get { return _deck; }
            set { _deck = value; }
        }

        private void Start()
        {
            CalculateCardSiblingIndex();
        }

        protected virtual void CalculateCardSiblingIndex()
        {
            IndexZ = transform.GetSiblingIndex();
        }

        /// <summary>
        /// Set new background image for card.
        /// </summary>
        /// <param name="path"></param>
        protected void SetBackgroundImg(string path)
        {
            if (_cachedSpritePath == path)
            {
                return;
            }

            _cachedSpritePath = path;

            Sprite tempType = CardLogicComponent.LoadSprite(path);
            BackgroundImage.sprite = tempType;
        }

        /// <summary>
        /// Show star particles.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ActivateParticle()
        {
            
            CardLogicComponent.ParticleStars.Play();
			yield return new WaitForSeconds(0.2f);
			CardLogicComponent.ParticleStars.Stop();
		}

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (CardLogicComponent.AutoCompleteComponent.IsAutoCompleteActive ||
                CardLogicComponent.HintManagerComponent.IsHintProcess ||
                !IsDraggable)
            {
                return;
            }

            _isDragging = true;

            CalculateCardSiblingIndex();
            _deck.SetCardsToTop(this);
            CardLogicComponent.ParticleStars.transform.SetParent(gameObject.transform);
            CardLogicComponent.ParticleStars.transform.SetAsFirstSibling();

			if (_coroutine != null)
				StopCoroutine(_coroutine);
			CardLogicComponent.ParticleStars.Stop();
		}

        public void OnDrag(PointerEventData eventData)
        {
            if (CardLogicComponent.AutoCompleteComponent.IsAutoCompleteActive || !IsDraggable)
            {
                return;
            }

            RectTransformUtility.ScreenPointToWorldPointInRectangle(CardRect, Input.mousePosition,
                eventData.enterEventCamera, out _newPosition);
            if (_lastMousePosition != Vector3.zero)
            {
                Vector3 offset = _newPosition - _lastMousePosition;
                transform.position += offset;
                CardLogicComponent.ParticleStars.transform.position = new Vector3(transform.position.x,
                    transform.position.y - 20f, transform.position.z);
                _deck.SetPositionFromCard(this, transform.position.x, transform.position.y);
            }

            _lastMousePosition = _newPosition;
        }

        public async void OnEndDrag(PointerEventData eventData)
        {
            if (CardLogicComponent.AutoCompleteComponent.IsAutoCompleteActive || !IsDraggable)
            {
                return;
            }

            transform.SetSiblingIndex(IndexZ);
            _lastMousePosition = Vector3.zero;

			if (_coroutine != null)
                StopCoroutine(_coroutine);
			_coroutine = ActivateParticle();
			StartCoroutine(_coroutine);
			Debug.Log("Trying to play particles");
    

            await CardLogicComponent.OnDragEnd(this);
            _deck.UpdateCardsPosition(false);

            _isDragging = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isDragging)
            {
                return;
            }

			CardLogicComponent.ParticleStars.transform.SetParent(gameObject.transform);
			CardLogicComponent.ParticleStars.transform.SetAsFirstSibling();
			CardLogicComponent.ParticleStars.transform.position = new Vector3(transform.position.x,
					transform.position.y - 20f, transform.position.z);

			switch (Deck.Type)
            {
                case DeckType.DECK_TYPE_PACK:
                {
                    OnTapToPack(eventData);
                }
                    break;
                case DeckType.DECK_TYPE_BOTTOM:
                case DeckType.DECK_TYPE_WASTE:
                case DeckType.DECK_TYPE_ACE:
                case DeckType.DECK_TYPE_FREECELL:
                case DeckType.DECK_TYPE_TRIPEAKS:
                case DeckType.DECK_TYPE_PYRAMID:
                {
                    OnTapToPlace();
                }
                    break;
            }
        }

        /// <summary>
        /// Get card texture by type.
        /// </summary>
        /// <returns> Texture string type</returns>
        protected string GetTexture()
        {
            var visualData = CardLogicComponent.CardShirtComponent.CurrentInfo;

			numberText.text = Number.ToString();

            //if (CardStatus == 0)
            //{
            //	numberText.gameObject.SetActive(false);
            //}
            //else
            //{
            numberText.gameObject.SetActive(false);
            //}

            return CardStatus == 0
                ? $"{Public.PATH_TO_CARD_BACKS_IN_RESOURCES}{visualData.CardBack}"
                : $"{Public.PATH_TO_CARD_FRONTS_IN_RESOURCES}{visualData.CardFront}/{GetTypeName()}{Number}";
        }

        /// <summary>
        /// Set default card status and background image <see cref="SetBackgroundImg"/>.
        /// </summary>
        public void RestoreBackView()
        {
            CardStatus = 0;
            var path = GetTexture();
            SetBackgroundImg(path);
        }

        /// <summary>
        /// Set card position.
        /// </summary>
        /// <param name="position">New card position.</param>
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        /// <summary>
        /// Initialize card by number.
        /// </summary>
        /// <param name="cardNum">Card number.</param>
        public abstract void InitWithNumber(int cardNum);

        /// <summary>
        /// Update card background <see cref="SetBackgroundImg"/>.
        /// </summary>
        public void UpdateCardImg()
        {
            var path = GetTexture();
            SetBackgroundImg(path);
        }

        public string GetTypeName()
        {
            switch (CardType)
            {
                case 0:
                    return Public.SpadeTextureName;
                case 1:
                    return Public.HeartTextureName;
                case 2:
                    return Public.ClubTextureName;
                case 3:
                    return Public.DiamondTextureName;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        ///Called when user click on card double times in specific interval
        /// </summary>
        protected abstract void OnTapToPlace();

        protected virtual void OnTapToPack(PointerEventData eventData)
        {
            if (CardLogicComponent.AutoCompleteComponent.IsAutoCompleteActive)
            {
                return;
            }

            Deck.OnPointerClick(eventData);
        }

        public void SetBackgroundColor(Color color)
        {
            BackgroundImage.color = color;
        }
    }
}