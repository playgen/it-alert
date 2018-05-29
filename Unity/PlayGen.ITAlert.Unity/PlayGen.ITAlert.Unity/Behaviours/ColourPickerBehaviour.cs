using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.Photon.Players;
using PlayGen.Unity.Utilities.Extensions;

using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Behaviours
{
	[RequireComponent(typeof(Button))]
	public class ColourPickerBehaviour : MonoBehaviour
	{
		private class PlayerGlyph
		{
			public Button Button { get; set; }

			public Image Image { get; set; }
		}

		private Dictionary<string, PlayerGlyph> _playerGlyphs;

		private GameObject _colourPickerRow;
		private GameObject _colourPickerObject;

		[SerializeField]
		private GameObject _colourPickerPanel;

		private PlayerColour _playerColour;

		private Player _currentPlayer;

		public void UpdateSelectedGlyphs(IEnumerable<ITAlertPlayer> players)
		{
			var playersByGlyph = players.ToDictionary(k => k.Glyph, v => v);
			foreach (var playerGlyph in _playerGlyphs)
			{
				if (playersByGlyph.TryGetValue(playerGlyph.Key, out var player))
				{
					if (player.PhotonId == _currentPlayer.PhotonId)
					{
						if (player.Glyph != _playerColour.Glyph
							&& playersByGlyph.ContainsKey(_playerColour.Glyph) == false)
						{
							playerGlyph.Value.Button.interactable = true;
							playerGlyph.Value.Image.color = Color.white;
						}
						else
						{
							_playerColour.Glyph = player.Glyph;
						}
						
						UpdateCurrentPlayer();
					}
					else
					{
						playerGlyph.Value.Button.interactable = false;
						playerGlyph.Value.Image.color = ColorUtility.TryParseHtmlString(player.Colour, out var selectedColour)
							? selectedColour
							: Color.gray;
					}
				}
				else if (playerGlyph.Key != _playerColour.Glyph)
				{
					playerGlyph.Value.Button.interactable = true;
					playerGlyph.Value.Image.color = Color.white;
				}
			}
		}

		private void UpdateCurrentPlayer(bool reset = false)
		{
			if (_playerGlyphs.TryGetValue(_playerColour.Glyph, out var playerGlyph))
			{
				if (reset || ColorUtility.TryParseHtmlString(_playerColour.Colour, out var selectedColour) == false)
				{
					playerGlyph.Image.color = Color.white;
				}
				else
				{
					playerGlyph.Image.color = selectedColour;
				}
			}
		}

		public void GenerateColorPicker(List<ITAlertPlayer> players, ITAlertPlayer currentPlayer)
		{
			_currentPlayer = currentPlayer;
			
			_playerColour = new PlayerColour
								{
				Colour = currentPlayer.Colour ?? "#ffffff",
				Glyph = currentPlayer.Glyph
			};
			_playerGlyphs = new Dictionary<string, PlayerGlyph>();

			if (_colourPickerRow != null && _colourPickerObject != null)
			{
				return;
			}

			_colourPickerObject = Resources.Load<GameObject>("ColorPickerObject");
			_colourPickerRow = Resources.Load<GameObject>("ColorPickerRow");

			const int cols = 3;
			var rows = Mathf.CeilToInt((float)PlayerColour.Glyphs.Length / cols);

			var index = 0;
			for (var i = 0; i < rows; i++)
			{
				if (index >= PlayerColour.Glyphs.Length)
				{
					continue;
				}

				var row = Instantiate(_colourPickerRow);
				row.transform.SetParent(_colourPickerPanel.transform, false);

				for (var j = 0; j < cols; j++)
				{
					if (index >= PlayerColour.Glyphs.Length)
					{
						continue;
					}

					var glyphName = PlayerColour.Glyphs[index];
					
					var colourPickerEntry = Instantiate(_colourPickerObject);
					colourPickerEntry.transform.SetParent(row.transform, false);

					var image = colourPickerEntry.GetComponent<Image>();
					var sprite = Resources.Load<Sprite>($"playerglyph_{glyphName}");
					var iconImage = colourPickerEntry.transform.FindImage("Icon");
					iconImage.sprite = sprite;

					var button = colourPickerEntry.GetComponent<Button>();
					button.onClick.AddListener(delegate { SetGlyph(glyphName); });
					index++;

					_playerGlyphs.Add(glyphName, new PlayerGlyph
													{
						Button = button,
						Image = image
					});
				}

				UpdateSelectedGlyphs(players);
			}
		}

		public void SetColor(Color color)
		{
			_playerColour.Colour = $"#{ColorUtility.ToHtmlStringRGB(color)}";
			UpdateCurrentPlayer();
		}

		private void SetGlyph(string glyph)
		{
			UpdateCurrentPlayer(true);
			_playerColour.Glyph = glyph;
			UpdateCurrentPlayer();
		}

		public PlayerColour GetPlayerColour()
		{
			return _playerColour;
		}
	}
}