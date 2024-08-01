using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public interface ILayoutContainer<out T> where T : LayoutData 
    {
        LayoutData LoadLayout(int id);
        T CreateNewPuzzle();
    }

    public abstract class BaseLayoutsContainer<T> : ScriptableObject, ILayoutContainer<T> where T : LayoutData 
    {
        public abstract T CurrentLayout { get; set; }

        public abstract List<T> Layouts { get; }
        public HashSet<int> ActiveLayouts = new HashSet<int>();

        public abstract string LayoutsKey { get;} 

        public CardPositionInfo GetInfoById(int id)
        {
            return CurrentLayout.GetInfoById(id);
        }

        public CardPositionInfo GetInfoByIndex(int positionIndex)
        {
            return CurrentLayout.GetInfoByIndex(positionIndex);
        }

        public void SetRandomLayout()
        {
            int randomId = Random.Range(0, ActiveLayouts.Count);
            int layoutId = ActiveLayouts.ElementAt(randomId);

            CurrentLayout = Layouts.FirstOrDefault(x=>x.LayoutId == layoutId);
        }

        public void SetCurrentLayout(int id)
        {
            for (int i = 0; i < Layouts.Count; i++)
            {
                if (Layouts[i].LayoutId == id)
                {
                    CurrentLayout = Layouts[i];
                    return;
                }
            }
        }

        public void SetDefaultLayout()
        {
            CurrentLayout = Layouts.FirstOrDefault(x => x.IsDefault);
        }

        public bool IsDefaultLayoutActive()
        {
            T defaultLayout = Layouts.FirstOrDefault(x => x.IsDefault);
            return defaultLayout != null && ActiveLayouts.Contains(defaultLayout.LayoutId);
        }
        
        public void GetLayoutsSettings()
        {
            if (PlayerPrefs.HasKey(LayoutsKey))
            {
                string layoutsData = PlayerPrefs.GetString(LayoutsKey);
                ActiveLayouts = JsonConvert.DeserializeObject<HashSet<int>>(layoutsData);
            }
            else
            {
                Layouts.ForEach(x => ActiveLayouts.Add(x.LayoutId));
                SaveLayouts();
            }
        }

        public void SaveLayouts()
        {
            string layoutsData = JsonConvert.SerializeObject(ActiveLayouts);
            PlayerPrefs.SetString(LayoutsKey, layoutsData);
        }

        public abstract T CreateNewPuzzle();

        public LayoutData LoadLayout(int id)
        {
            return Layouts.FirstOrDefault(x => x.LayoutId == id);
        }

        public void ClearLayoutById(int id)
        {
            var layout = LoadLayout(id);
            if (layout != null)
            {
                layout.Infos = new List<CardPositionInfo>();
            }
        }

        public bool IsActiveLayout(int id) => ActiveLayouts.Contains(id);
        public bool HasOneOrLessLayout() => ActiveLayouts.Count <= 1;

        public bool RemoveLayout(int id) => ActiveLayouts.Remove(id);
        public bool AddLayout(int id) => ActiveLayouts.Add(id);
    }
}