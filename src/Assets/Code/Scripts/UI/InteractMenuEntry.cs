using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InteractMenuEntry : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _text;

        [Tooltip("An image only shown when this entry is selected.")]
        [SerializeField]
        private Image _selectionImage;


        public void Clear()
        {
            if (_text == null)
                return;
            
            _text.text = "";
            SetSelected(false);
        }


        public void SetText(int index, string menuName)
        {
            _text.text = $"{index + 1}. {menuName}";
        }
        
        
        public void SetSelected(bool selected)
        {
            _selectionImage.gameObject.SetActive(selected);
        }
    }
}