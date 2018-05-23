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
            get { return _text.text; }
            set { SetText(value); }
        }

        public void SetText(string text)
        {
            var builder = new StringBuilder();
            for(var i = 0; i < Mathf.Min(text.Length, _useGlobalSettings ? GlobalMaxLength : _maxLength); i++)
            {
                if (!_cuttoffAfter.Contains(text[i]) && (!_useGlobalSettings || !GlobalCutoffAfter.Contains(text[i])))
                {
                    builder.Append(text[i]);
                }
                else
                {
                    break;
                }
            }

			if (text.Length > (_useGlobalSettings ? GlobalMaxLength : _maxLength) && builder.ToString().Length == (_useGlobalSettings ? GlobalMaxLength : _maxLength))
			{
				builder.Append("...");
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
