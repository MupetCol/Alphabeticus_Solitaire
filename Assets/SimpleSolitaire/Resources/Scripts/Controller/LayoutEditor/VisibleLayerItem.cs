using System;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
    public class VisibleLayerData
    {
        public int Layer;
        public bool Active;
    }

    public class VisibleLayerItem : MonoBehaviour
    {
        public Text Label;
        public Image Checkmark;
        public Button Btn;

        public event Action OnClick;

        public void Initialize(VisibleLayerData data)
        {
            if (data == null)
            {
                return;
            }

            Label.text = data.Layer.ToString();
            ActivateCheckmark(data.Active);
            Btn.onClick.RemoveAllListeners();
            Btn.onClick.AddListener(OnClickListener);
        }

        private void OnClickListener()
        {
            OnClick?.Invoke();
        }

        public void ActivateCheckmark(bool active)
        {
            Checkmark.enabled = active;
        }
    }
}