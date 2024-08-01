using System.Collections;
using System.Collections.Generic;
using SimpleSolitaire.Controller.Additional;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    [System.Serializable]
    public class HowToPlayData
    {
        public Sprite Preview;

        [TextArea(3, 10)] public string Text;
    }

    public abstract class HowToPlayManager : MonoBehaviour
    {
        [SerializeField] private HowToPlayDataContainer _container;
        [SerializeField] private HowToPlayItem _item;
        [SerializeField] private RectTransform _content;
        [SerializeField] private ScrollSnapRect _scrollSnap;
        [SerializeField] private GameManager _gameManagerComponent;

        protected abstract string FirstPlayKey { get; }

        public void SetFirstPage()
        {
            _scrollSnap.SetPage(0);
        }

        /// <summary>
        /// Activate tutorial if player game first time.
        /// </summary>
        private IEnumerator Start()
        {
            GeneratePages();
            _scrollSnap.Initialize();
            
            yield return new WaitForSeconds(0.1f);

            if (!IsHasKey())
            {
                PlayerPrefs.SetInt(FirstPlayKey, 1);
                _gameManagerComponent.ShowHowToPlayLayer();
            }
        }

        /// <summary>
        /// Is first play or not.
        /// </summary>
        public bool IsHasKey()
        {
            return PlayerPrefs.HasKey(FirstPlayKey);
        }

        private void GeneratePages()
        {
            for (int i = 0; i < _container.Pages.Count; i++)
            {
                HowToPlayData page = _container.Pages[i];
                HowToPlayItem item = Instantiate(_item, _content);
                item.Initialize(page);
            }
        }
    }
}