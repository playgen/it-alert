using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Components
{
    [RequireComponent(typeof(Text))]
    public class TextCutoff : MonoBehaviour
    {
        [SerializeField] private int _maxLength;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [SerializeField] private char[] _cuttoffAfter = { };
        [SerializeField] private bool _useGlobalSettings = true;
        private Text _text;

        public static int GlobalMaxLength = 10;
        public static char[] GlobalCutoffAfter = {'@'};

        // Mimic Unity's API
        public string text
        {
            get => _text.text;
            set => SetText(value);
        }

        public void SetText(string uncutText)
        {
            var builder = new StringBuilder();
            for(var i = 0; i < Mathf.Min(uncutText.Length, _useGlobalSettings ? GlobalMaxLength : _maxLength); i++)
            {
                if (!_cuttoffAfter.Contains(uncutText[i]) && (!_useGlobalSettings || !GlobalCutoffAfter.Contains(uncutText[i])))
                {
                    builder.Append(uncutText[i]);
                }
                else
                {
                    break;
                }
            }

            var cutText = builder.ToString();
            _text.text = cutText;
        }

        private void Awake()
        {
            _text = GetComponent<Text>();

            if (!string.IsNullOrWhiteSpace(_text.text))
            {
                SetText(_text.text);
            }
        }
    }
}
