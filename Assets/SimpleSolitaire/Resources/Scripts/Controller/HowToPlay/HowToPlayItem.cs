using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
    public class HowToPlayItem : MonoBehaviour
    {
        [SerializeField] private Image Img;
        [SerializeField] private Text Txt;

        public void Initialize(HowToPlayData data)
        {
            if (data == null)
            {
                return;
            }

            Img.sprite = data.Preview;
            Txt.text = data.Text;
        }

        public void Activate(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}