using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
	public class VisualiseElement : MonoBehaviour
	{
		public Image VisualImage;
		public Image CheckMark;
		public Animator Anim;
		public Button Btn;

		private readonly string _animationActionTrigger = "Action";
		private bool _isActive = false;

		public void OnEnable()
		{
			UpdateAnimation();
		}
		
		public void UpdateAnimation()
		{
			if (Anim == null)
			{
				return;
			}

			if (_isActive)
			{
				Anim.enabled = true;
				Anim.SetTrigger(_animationActionTrigger);
			}
			else
			{
				Anim.enabled = false;
				Anim.ResetTrigger(_animationActionTrigger);
			}
		}

		public void ActivateCheckmark()
		{
			_isActive = true;
			CheckMark.enabled = true;
		}

		public void DeactivateCheckmark()
		{
			_isActive = false;
			CheckMark.enabled = false;
		}
	}
}