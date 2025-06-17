using UnityEngine;
using System.Collections.Generic;
using System;
using Core.UI.Elements;

namespace Core.UI.Groups {
    public class ButtonGroup : MonoBehaviour {
        public event EventHandler<SelectionEventArgs> OnSelected;
        public class SelectionEventArgs : EventArgs {
            public ButtonElement buttonElement;
        }

        private List<ButtonElement> buttonElements = new List<ButtonElement>();

        private ButtonElement selectedElement;

        public void Register(ButtonElement buttonElement) {
            buttonElements.Add(buttonElement);
        }

        public void Select(ButtonElement buttonElement) {
            if (selectedElement == buttonElement) {
                return;
            }
            foreach (var element in buttonElements) {
                if (element == buttonElement) {
                    continue;
                }
                Deselect(element);
            }
            buttonElement.Select();
            selectedElement = buttonElement;
            OnSelected?.Invoke(this, new SelectionEventArgs {buttonElement=buttonElement});
        }

        public void Deselect(ButtonElement buttonElement) {
            buttonElement.Deselect();
        }
    }
}