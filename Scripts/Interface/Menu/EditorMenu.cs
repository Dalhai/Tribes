using System;
using System.Collections.Generic;
using Godot;
using TribesOfDust.Core;
using TribesOfDust.Core.Modes;

namespace TribesOfDust.Interface.Menu;

public partial class EditorMenu : MarginContainer
{
    // Node references
    private Button _tundraButton = null!;
    private Button _rocksButton  = null!;
    private Button _dunesButton  = null!;
    private Button _canyonButton = null!;
    private Button _openButton   = null!;
    private Button _exitButton   = null!;

    // Dictionary for easier button access
    private readonly Dictionary<TileType, Button> _tileButtons = new();

    public override void _Ready()
    {
        // Get node references
        _tundraButton = GetNode<Button>("MenuContainer/TilesContainer/Tundra/Button");
        _rocksButton  = GetNode<Button>("MenuContainer/TilesContainer/Rocks/Button");
        _dunesButton  = GetNode<Button>("MenuContainer/TilesContainer/Dunes/Button");
        _canyonButton = GetNode<Button>("MenuContainer/TilesContainer/Canyons/Button");
        _openButton   = GetNode<Button>("MenuContainer/TilesContainer/Open/Button");
        _exitButton   = GetNode<Button>("MenuContainer/Exit/Button");

        // Set up dictionary for easier access
        _tileButtons[TileType.Tundra] = _tundraButton;
        _tileButtons[TileType.Rocks]  = _rocksButton;
        _tileButtons[TileType.Dunes]  = _dunesButton;
        _tileButtons[TileType.Canyon] = _canyonButton;
        _tileButtons[TileType.Open]   = _openButton;

        // Set up click handlers for tile buttons
        _tundraButton.Pressed += () => OnButtonPressed(TileType.Tundra);
        _rocksButton.Pressed  += () => OnButtonPressed(TileType.Rocks);
        _dunesButton.Pressed  += () => OnButtonPressed(TileType.Dunes);
        _canyonButton.Pressed += () => OnButtonPressed(TileType.Canyon);
        _openButton.Pressed   += () => OnButtonPressed(TileType.Open);

        // Set up exit button
        _exitButton.Pressed += OnExitPressed;

        base._Ready();
    }

    public void UpdateTileCount(TileType tileType, int count)
    {
        if (_tileButtons.TryGetValue(tileType, out var button))
        {
            button.Text = $"{count} {GetTileTypeName(tileType)}";
        }
    }

    private void OnButtonPressed(TileType tileType)
    {
        foreach (var (buttonType, button) in _tileButtons)
        {
            if (buttonType != tileType)
                button.SetPressed(false);
        }

        EditorMode.Instance?.SetActiveTileType(tileType);
    }

    private void OnExitPressed()
    {
        Context.Instance.Navigator.GoTo("main");
    }

    private static string GetTileTypeName(TileType tileType)
    {
        return tileType switch
        {
            TileType.Tundra => "Tundra",
            TileType.Rocks  => "Rocks",
            TileType.Dunes  => "Dunes",
            TileType.Canyon => "Canyon",
            TileType.Open   => "Open",
            _               => tileType.ToString()
        };
    }
}