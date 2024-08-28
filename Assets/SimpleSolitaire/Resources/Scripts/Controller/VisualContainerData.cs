using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    [System.Serializable]
    public class GameVisual
    {
        public VisualiseElement Prefab;
        public List<VisualContentData> Content;

        public string SaveName;

        public string GetDefault()
        {
			var defaultVisual = Content.FirstOrDefault(x => x.IsDefault);
            return defaultVisual != null ? defaultVisual.Name : Content.First().Name;
        }
    }

    [System.Serializable]
    public class VisualContentData
    {
        public string Name;
        public Sprite Preview;
        public bool IsDefault;
    }

    [CreateAssetMenu(fileName = "BaseVisualContainerData", menuName = "Visual/Visual Container Data")]
    public class VisualContainerData : ScriptableObject
    {
        public bool IsAutoNameValidationEnabled = false;
        
        public GameVisual BackgroundVisual;
        public GameVisual CardBackVisual;
        public GameVisual CardFrontVisual;

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (!IsAutoNameValidationEnabled)
            {
                return;
            }

            BackgroundVisual.Content.ForEach(UpdateVisualContentDataName);
            CardBackVisual.Content.ForEach(UpdateVisualContentDataName);
            CardFrontVisual.Content.ForEach(UpdateVisualContentDataName);
            
            void UpdateVisualContentDataName(VisualContentData contentData)
            {
                if (contentData.Preview != null)
                {
                    if (string.IsNullOrEmpty(contentData.Name) || contentData.Name != contentData.Preview.name)
                    {
                        contentData.Name = contentData.Preview.name;
                    }
                }
                else
                {
                    contentData.Name = string.Empty;
                }
            }
#endif
        }
    }
}