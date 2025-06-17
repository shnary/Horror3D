using UnityEngine;
using System.Collections.Generic;
using Core.UI.Elements;
using UnityEngine.Events;

namespace Core.UI.Groups {
    public class TabGroup : MonoBehaviour {
        private List<TabElement> tabElements = new List<TabElement>();

        private TabElement selectedElement;

        public void Register(TabElement tabElement) {
            tabElements.Add(tabElement);
        }

        public void Select(TabElement tabElement) {
            if (selectedElement == tabElement) {
                return;
            }
            foreach (var element in tabElements) {
                if (tabElement == element) {
                    continue;
                }
                Deselect(element);
            }
            tabElement.Select();
            selectedElement = tabElement;
        }

        public void Deselect(TabElement tabElement) {
            tabElement.Deselect();
        }
    }
}