using System;
using System.Collections.Generic;
using Godot;
using TribesOfDust.Core;
using TribesOfDust.Core.Modes;

namespace TribesOfDust.Interface.Menu;

public partial class EditorMenu : MarginContainer
{
    // Node references
    private Label _tundraLabel = null!;
    private Label _rocksLabel = null!;
    private Label _dunesLabel = null!;
    private Label _canyonLabel = null!;
    private Label _openLabel = null!;
    private Button _exitButton = null!;
    
    // Color constants for highlighting
    private static readonly Color ActiveColor = Colors.LightBlue;
    private static readonly Color HoverColor = Colors.LightBlue;
    private static readonly Color NormalColor = Colors.White;
    
    // Dictionary for easier label access
    private Dictionary<TileType, Label> _tileLabels = new();
    
    public override void _Ready()
    {
        // Get node references
        _tundraLabel = GetNode<Label>("VBoxContainer/TileCountsContainer/TundraLabel");
        _rocksLabel = GetNode<Label>("VBoxContainer/TileCountsContainer/RocksLabel");
        _dunesLabel = GetNode<Label>("VBoxContainer/TileCountsContainer/DunesLabel");
        _canyonLabel = GetNode<Label>("VBoxContainer/TileCountsContainer/CanyonLabel");
        _openLabel = GetNode<Label>("VBoxContainer/TileCountsContainer/OpenLabel");
        _exitButton = GetNode<Button>("VBoxContainer/ExitButton");
        
        // Set up dictionary for easier access
        _tileLabels[TileType.Tundra] = _tundraLabel;
        _tileLabels[TileType.Rocks] = _rocksLabel;
        _tileLabels[TileType.Dunes] = _dunesLabel;
        _tileLabels[TileType.Canyon] = _canyonLabel;
        _tileLabels[TileType.Open] = _openLabel;
        
        // Set up click handlers for tile labels
        SetupLabelInteractions(_tundraLabel, TileType.Tundra);
        SetupLabelInteractions(_rocksLabel, TileType.Rocks);
        SetupLabelInteractions(_dunesLabel, TileType.Dunes);
        SetupLabelInteractions(_canyonLabel, TileType.Canyon);
        SetupLabelInteractions(_openLabel, TileType.Open);
        
        // Set up exit button
        _exitButton.Pressed += OnExitPressed;
        
        base._Ready();
    }
    
    private void SetupLabelInteractions(Label label, TileType tileType)
    {
        // Enable mouse input
        label.MouseFilter = Control.MouseFilterEnum.Pass;
        
        // Set up mouse enter/exit for hover effects
        label.MouseEntered += () => OnLabelMouseEntered(label, tileType);
        label.MouseExited += () => OnLabelMouseExited(label, tileType);
        
        // Set up click handling
        label.GuiInput += (InputEvent @event) => OnLabelInput(@event, tileType);
    }
    
    private void OnLabelMouseEntered(Label label, TileType tileType)
    {
        if (EditorMode.Instance?.GetActiveTileType() != tileType)
        {
            label.Modulate = HoverColor;
        }
    }
    
    private void OnLabelMouseExited(Label label, TileType tileType)
    {
        if (EditorMode.Instance?.GetActiveTileType() != tileType)
        {
            label.Modulate = NormalColor;
        }
    }
    
    private void OnLabelInput(InputEvent @event, TileType tileType)
    {
        if (@event is InputEventMouseButton mouseButton && 
            mouseButton.Pressed && 
            mouseButton.ButtonIndex == MouseButton.Left)
        {
            EditorMode.Instance?.SetActiveTileType(tileType);
        }
    }
    
    private void OnExitPressed()
    {
        Context.Instance?.Navigator.GoTo("main");
    }
    
    public void UpdateTileCount(TileType tileType, int count)
    {
        if (_tileLabels.TryGetValue(tileType, out var label))
        {
            label.Text = $"{count} {GetTileTypeName(tileType)}";
        }
    }
    
    public void UpdateActiveHighlight(TileType activeTileType)
    {
        // Reset all labels to normal color
        foreach (var (tileType, label) in _tileLabels)
        {
            label.Modulate = tileType == activeTileType ? ActiveColor : NormalColor;
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