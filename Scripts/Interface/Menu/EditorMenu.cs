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
    private Button _rocksButton = null!;
    private Button _dunesButton = null!;
    private Button _canyonButton = null!;
    private Button _openButton = null!;
    private Button _exitButton = null!;
    
    // Color constants for highlighting
    private static readonly Color ActiveColor = Colors.LightBlue;
    private static readonly Color HoverColor = Colors.LightBlue;
    private static readonly Color NormalColor = Colors.White;
    
    // Dictionary for easier button access
    private Dictionary<TileType, Button> _tileButtons = new();
    
    public override void _Ready()
    {
        // Get node references
        _tundraButton = GetNode<Button>("MenuContainer/LabelsContainer/TundraButton");
        _rocksButton = GetNode<Button>("MenuContainer/LabelsContainer/RocksButton");
        _dunesButton = GetNode<Button>("MenuContainer/LabelsContainer/DunesButton");
        _canyonButton = GetNode<Button>("MenuContainer/LabelsContainer/CanyonButton");
        _openButton = GetNode<Button>("MenuContainer/LabelsContainer/OpenButton");
        _exitButton = GetNode<Button>("MenuContainer/ExitButton");
        
        // Set up dictionary for easier access
        _tileButtons[TileType.Tundra] = _tundraButton;
        _tileButtons[TileType.Rocks] = _rocksButton;
        _tileButtons[TileType.Dunes] = _dunesButton;
        _tileButtons[TileType.Canyon] = _canyonButton;
        _tileButtons[TileType.Open] = _openButton;
        
        // Set up click handlers for tile buttons
        SetupButtonInteractions(_tundraButton, TileType.Tundra);
        SetupButtonInteractions(_rocksButton, TileType.Rocks);
        SetupButtonInteractions(_dunesButton, TileType.Dunes);
        SetupButtonInteractions(_canyonButton, TileType.Canyon);
        SetupButtonInteractions(_openButton, TileType.Open);
        
        // Set up exit button
        _exitButton.Pressed += OnExitPressed;
        
        base._Ready();
    }
    
    private void SetupButtonInteractions(Button button, TileType tileType)
    {
        // Set up mouse enter/exit for hover effects
        button.MouseEntered += () => OnButtonMouseEntered(button, tileType);
        button.MouseExited += () => OnButtonMouseExited(button, tileType);
        
        // Set up click handling
        button.Pressed += () => OnButtonPressed(tileType);
    }
    
    private void OnButtonMouseEntered(Button button, TileType tileType)
    {
        if (EditorMode.Instance?.GetActiveTileType() != tileType)
        {
            button.Modulate = HoverColor;
        }
    }
    
    private void OnButtonMouseExited(Button button, TileType tileType)
    {
        if (EditorMode.Instance?.GetActiveTileType() != tileType)
        {
            button.Modulate = NormalColor;
        }
    }
    
    private void OnButtonPressed(TileType tileType)
    {
        EditorMode.Instance?.SetActiveTileType(tileType);
    }
    
    private void OnExitPressed()
    {
        Context.Instance?.Navigator.GoTo("main");
    }
    
    public void UpdateTileCount(TileType tileType, int count)
    {
        if (_tileButtons.TryGetValue(tileType, out var button))
        {
            button.Text = $"{count} {GetTileTypeName(tileType)}";
        }
    }
    
    public void UpdateActiveHighlight(TileType activeTileType)
    {
        // Reset all buttons to normal color
        foreach (var (tileType, button) in _tileButtons)
        {
            button.Modulate = tileType == activeTileType ? ActiveColor : NormalColor;
        }
    }
    
    private static string GetTileTypeName(TileType tileType)
    {
        return tileType switch
        {
            TileType.Tundra => "Tundra",
            TileType.Rocks => "Rocks", 
            TileType.Dunes => "Dunes",
            TileType.Canyon => "Canyon",
            TileType.Open => "Open",
            _ => tileType.ToString()
        };
    }
}