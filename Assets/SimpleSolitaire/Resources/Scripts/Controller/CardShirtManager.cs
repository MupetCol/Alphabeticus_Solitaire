using System.Collections.Generic;
using SimpleSolitaire.Model.Config;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
    [System.Serializable]
    public class CurrentVisualInfo
    {
        public string Background;
        public string CardBack;
        public string CardFront;
    }

    public class CardShirtManager : MonoBehaviour
    {
        public VisualContainerData Container;

        [Header("Visual containers:")] public RectTransform BackgroundsContainer;
        public RectTransform CardBacksContainer;
        public RectTransform CardFrontsContainer;

        [Header("Components:")] public Image GameBG;
        public CardLogic CLComponent;

        public CurrentVisualInfo CurrentInfo => _currentInfo;

        private List<VisualiseElement> _backgroundVisual = new List<VisualiseElement>();
        private List<VisualiseElement> _cardBackVisual = new List<VisualiseElement>();
        private List<VisualiseElement> _cardFrontVisual = new List<VisualiseElement>();
        private CurrentVisualInfo _currentInfo = new CurrentVisualInfo();

        private void Awake()
        {
            InitializeButtons();
            GetSettings();
        }
        
        private void Start()
        {
            SetCurrentVisual();
        }

        private void InitializeButtons()
        {
            foreach (var data in Container.BackgroundVisual.Content)
            {
                var item = Instantiate(Container.BackgroundVisual.Prefab, BackgroundsContainer);
                item.Btn.onClick.AddListener(() =>
                {
                    SetBackGround(data);
                    AudioController.Instance.Play(AudioController.AudioType.ButtonClick);
                });

                item.VisualImage.sprite = data.Preview;
                item.name = data.Name;

                _backgroundVisual.Add(item);
            }

            foreach (var data in Container.CardFrontVisual.Content)
            {
                var item = Instantiate(Container.CardFrontVisual.Prefab, CardFrontsContainer);
                item.Btn.onClick.AddListener(() =>
                {
                    SetFront(data);
                    AudioController.Instance.Play(AudioController.AudioType.ButtonClick);
                });

                item.VisualImage.sprite = data.Preview;
                item.name = data.Name;

                _cardFrontVisual.Add(item);
            }

            foreach (var data in Container.CardBackVisual.Content)
            {
                var item = Instantiate(Container.CardBackVisual.Prefab, CardBacksContainer);
                item.Btn.onClick.AddListener(() =>
                {
                    SetBack(data);
                    AudioController.Instance.Play(AudioController.AudioType.ButtonClick);
                });

                item.VisualImage.sprite = data.Preview;
                item.name = data.Name;

                _cardBackVisual.Add(item);
            }
        }

        private void SetCurrentVisual()
        {
            SetBackGround(Container.BackgroundVisual.Content.Find(y => y.Name == _currentInfo.Background));
            SetBack(Container.CardBackVisual.Content.Find(y => y.Name == _currentInfo.CardBack));
            SetFront(Container.CardFrontVisual.Content.Find(y => y.Name == _currentInfo.CardFront));
        }

        /// <summary>
        /// Get settings values from player prefs.
        /// </summary>
        private void GetSettings()
        {
            _currentInfo = new CurrentVisualInfo()
            {
                Background = GetVisualSettings(Container.BackgroundVisual),
                CardBack = GetVisualSettings(Container.CardBackVisual),
                CardFront = GetVisualSettings(Container.CardFrontVisual)
            };
        }

        /// <summary>
        /// Get visual data save from prefs.
        /// </summary>
        private string GetVisualSettings(GameVisual visual)
        {
            string result;

            if (PlayerPrefs.HasKey(visual.SaveName))
            {
                result = PlayerPrefs.GetString(visual.SaveName);
            }
            else
            {
                result = visual.GetDefault();
                PlayerPrefs.SetString(visual.SaveName, result);
            }

            return result;
        }

        /// <summary>
        /// Set up background of game.
        /// </summary>
        private void SetFront(VisualContentData data)
        {
            _currentInfo.CardFront = data.Name;
            PlayerPrefs.SetString(Container.CardFrontVisual.SaveName, _currentInfo.CardFront);

            ActionWithAnimationForObjects(_cardFrontVisual, _currentInfo.CardFront);

            foreach (var item in CLComponent.CardsArray)
            {
                item.UpdateCardImg();
            }
        }

        /// <summary>
        /// Set up background of game.
        /// </summary>
        private void SetBackGround(VisualContentData data)
        {
            _currentInfo.Background = data.Name;
            PlayerPrefs.SetString(Container.BackgroundVisual.SaveName, _currentInfo.Background);

            ActionWithAnimationForObjects(_backgroundVisual, _currentInfo.Background);

            GameBG.sprite = CLComponent.LoadSprite(Public.PATH_TO_BG_IN_RESOURCES + _currentInfo.Background);
        }

        /// <summary>
        /// Set up shirt for card objects.
        /// </summary>
        private void SetBack(VisualContentData data)
        {
            _currentInfo.CardBack = data.Name;
            PlayerPrefs.SetString(Container.CardBackVisual.SaveName, _currentInfo.CardBack);

            ActionWithAnimationForObjects(_cardBackVisual, _currentInfo.CardBack);
            
            AudioController.Instance.Play(AudioController.AudioType.ButtonClick);
            
            foreach (var item in CLComponent.CardsArray)
            {
                item.UpdateCardImg();
            }
        }

        /// <summary>
        /// Activate animation which highlight chosen background.
        /// </summary>
        private void ActionWithAnimationForObjects(List<VisualiseElement> visual, string active)
        {
            visual.ForEach(a =>
            {
                if (a.name == active)
                {
                    a.ActivateCheckmark();
                    a.UpdateAnimation();
                    a.Anim.speed = 1f;
                }
                else
                {
                    a.DeactivateCheckmark();
                    a.UpdateAnimation();
                    a.transform.localRotation = Quaternion.identity;
                }
            });
        }
    }
}