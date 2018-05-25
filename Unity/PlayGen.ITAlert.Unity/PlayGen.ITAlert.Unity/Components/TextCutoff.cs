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

        public static int GlobalMaxLength = 15;
        public static char[] GlobalCutoffAfter = {'@'};

        // Mimic Unity's API
        public string text
        {
            get => _text.text;
            set => SetText(value);
        }

        public void SetText(string uncutText)
        {
	        var cutoffAfter = _useGlobalSettings ? GlobalCutoffAfter : _cuttoffAfter;
	        var maxLength = _useGlobalSettings ? GlobalMaxLength : _maxLength;

			_text.text = uncutText.Cutoff(cutoffAfter, maxLength);
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
