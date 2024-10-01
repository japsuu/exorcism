using System;
using UnityEditor;
using UnityEngine;

namespace Tools.Editor
{
    public class EditorInputDialog : EditorWindow
    {
        private string _descriptionText;
        private string _inputText;
        private string _okButtonText;
        private string _cancelButtonText;
        private bool _initializedPosition;
        private Vector2 _maxScreenPos;
        private Action _onOkButton;

        private bool _shouldClose;


        private void OnGUI()
        {
            // Check if Esc/Return have been pressed
            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                switch (e.keyCode)
                {
                    // Escape pressed
                    case KeyCode.Escape:
                        _shouldClose = true;
                        break;

                    // Enter pressed
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        _onOkButton?.Invoke();
                        _shouldClose = true;
                        break;
                }
            }

            if (_shouldClose) // Close this dialog
                Close();

            // Draw our control
            Rect rect = EditorGUILayout.BeginVertical();

            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField(_descriptionText);

            EditorGUILayout.Space(8);
            GUI.SetNextControlName("inText");
            _inputText = EditorGUILayout.TextField("", _inputText);
            GUI.FocusControl("inText"); // Focus text field
            EditorGUILayout.Space(12);

            // Draw OK / Cancel buttons
            Rect r = EditorGUILayout.GetControlRect();
            r.width /= 2;
            if (GUI.Button(r, _okButtonText))
            {
                _onOkButton?.Invoke();
                _shouldClose = true;
            }

            r.x += r.width;
            if (GUI.Button(r, _cancelButtonText))
            {
                _inputText = null; // Cancel - delete inputText
                _shouldClose = true;
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.EndVertical();

            // Force change size of the window
            if (rect.width != 0 && minSize != rect.size) minSize = maxSize = rect.size;

            // Set dialog position next to mouse position
            if(!_initializedPosition && e.type == EventType.Layout)
            {
                _initializedPosition = true;
 
                // Move window to a new position. Make sure we're inside visible window
                Vector2 mousePos = GUIUtility.GUIToScreenPoint( Event.current.mousePosition );
                mousePos.x += 32;
                if( mousePos.x + position.width > _maxScreenPos.x ) mousePos.x -= position.width + 64; // Display on left side of mouse
                if( mousePos.y + position.height > _maxScreenPos.y ) mousePos.y = _maxScreenPos.y - position.height;
 
                position = new Rect( mousePos.x, mousePos.y, position.width, position.height );
 
                // Focus current window
                Focus();
            }
        }

        
        public static string Show(string title, string description, string inputText, string okButton = "OK", string cancelButton = "Cancel")
        {
            // Make sure our popup is always inside parent window, and never offscreen.
            Vector2 maxPos = GUIUtility.GUIToScreenPoint(new Vector2(Screen.width, Screen.height));
            
            string ret = null;

            EditorInputDialog window = CreateInstance<EditorInputDialog>();
            window._maxScreenPos = maxPos;
            window.titleContent = new GUIContent(title);
            window._descriptionText = description;
            window._inputText = inputText;
            window._okButtonText = okButton;
            window._cancelButtonText = cancelButton;
            window._onOkButton += () => ret = window._inputText;
            window.ShowModal();

            return ret;
        }
    }
}