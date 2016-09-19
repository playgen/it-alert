﻿using UnityEngine;
using UnityEngine.UI;

public class ColourPickerBehaviour : MonoBehaviour
{

    /// <summary>
    /// Colors to use :- From https://personal.sron.nl/~pault/colourschemes.pdf
    /// 136,204,238     ||      0.533f, 0.8f, 0.933f
    /// 68,170,153      ||      0.266f, 0.666f, 0.6f
    /// 17,119,51       ||      0.066f, 0.466f, 0.2f
    /// 153,153,51      ||      0.6f, 0.6f, 0.2f
    /// 221,204,119     ||      0.866f, 0.8f, 0.466f
    /// 204,102,119     ||      0.8f, 0.4f, 0.466f
    /// 136,34,85       ||      0.533f, 0.133f, 0.333f
    /// 170,68,153      ||      0.666f, 0.266f, 0.6f
    /// 51,34,136       ||      0.2f, 0.133f, 0.533f
    /// </summary>           
    public Color[] colors;

    private GameObject _colourPickerRow;
    private GameObject _colourPickerObject;

    public GameObject ColourPickerPanel;
    public Image Avatar;

    private Color _selectedColor;

    public void GenerateColorPicker(Color[] allColors = null)
    {
        if (allColors != null)
            colors = allColors;
        SetColorPickerObjects();

        // HACK SET COLOR
        _selectedColor = colors[0];
        SetColor(_selectedColor);

        // Get the length of the colors array
        var totalColors = colors.Length;
        // Find the square value of totalColors
        var rows = Mathf.CeilToInt(Mathf.Sqrt(totalColors));

        var index = 0;
        for (int i = 0; i < rows; i++)
        {
            if (index >= totalColors)
                continue;
            var row = Instantiate(_colourPickerRow);
            row.transform.parent = ColourPickerPanel.transform;
            for (int j = 0; j < rows; j++)
            {
                if (index >= totalColors)
                    continue;
                // Populate the row with colour picker objects
                var obj = Instantiate(_colourPickerObject);
                obj.transform.parent = row.transform;
                var image = obj.GetComponent<Image>();
                image.color = colors[index];
                var button = obj.GetComponent<Button>();

                // TODO check if the colour is available
                //button.interactable = isAvailable

                button.onClick.AddListener(delegate { SetColor(image.color); });
                index++;
            }
        }
    }
    private void SetColorPickerObjects()
    {
        if (_colourPickerRow != null && _colourPickerObject != null)
            return;

        _colourPickerObject = Resources.Load<GameObject>("ColorPickerObject");
        _colourPickerRow = Resources.Load<GameObject>("ColorPickerRow");
    }
    private void SetColor(Color color)
    {
        _selectedColor = color;
        Avatar.color = color;
    }
}