using Godot;

namespace TribesOfDust.Core;

/// <summary>
/// Defines 32 fixed core colors that can be used throughout the game for overlays and systems.
/// These colors are supported by assets and provide consistent theming.
/// </summary>
public enum CoreColor
{
    // Primary colors
    Red,
    Green, 
    Blue,
    Yellow,
    
    // Secondary colors
    Cyan,
    Magenta,
    Orange,
    Purple,
    
    // Tertiary colors
    Pink,
    Lime,
    Teal,
    Indigo,
    
    // Neutral colors
    White,
    LightGray,
    Gray,
    DarkGray,
    Black,
    
    // Warm colors
    Crimson,
    Coral,
    Gold,
    Amber,
    
    // Cool colors
    Navy,
    SkyBlue,
    Aqua,
    Mint,
    
    // Earth tones
    Brown,
    Tan,
    Olive,
    Maroon,
    
    // Accent colors
    Violet,
    Turquoise,
    Silver
}

/// <summary>
/// Extension methods for CoreColor enum.
/// </summary>
public static class CoreColorExtensions
{
    /// <summary>
    /// Converts a CoreColor to a Godot Color.
    /// </summary>
    /// <param name="coreColor">The core color to convert</param>
    /// <returns>The corresponding Godot Color</returns>
    public static Color ToColor(this CoreColor coreColor)
    {
        return coreColor switch
        {
            // Primary colors
            CoreColor.Red => Colors.Red,
            CoreColor.Green => Colors.Green,
            CoreColor.Blue => Colors.Blue,
            CoreColor.Yellow => Colors.Yellow,
            
            // Secondary colors
            CoreColor.Cyan => Colors.Cyan,
            CoreColor.Magenta => Colors.Magenta,
            CoreColor.Orange => Colors.Orange,
            CoreColor.Purple => Colors.Purple,
            
            // Tertiary colors
            CoreColor.Pink => Colors.Pink,
            CoreColor.Lime => Colors.Lime,
            CoreColor.Teal => Colors.Teal,
            CoreColor.Indigo => Colors.Indigo,
            
            // Neutral colors
            CoreColor.White => Colors.White,
            CoreColor.LightGray => Colors.LightGray,
            CoreColor.Gray => Colors.Gray,
            CoreColor.DarkGray => Colors.DarkGray,
            CoreColor.Black => Colors.Black,
            
            // Warm colors
            CoreColor.Crimson => Colors.Crimson,
            CoreColor.Coral => Colors.Coral,
            CoreColor.Gold => Colors.Gold,
            CoreColor.Amber => Color.FromHtml("#FFBF00"),
            
            // Cool colors
            CoreColor.Navy => Color.FromHtml("#000080"),
            CoreColor.SkyBlue => Colors.SkyBlue,
            CoreColor.Aqua => Colors.Aqua,
            CoreColor.Mint => Color.FromHtml("#00FF7F"),
            
            // Earth tones
            CoreColor.Brown => Color.FromHtml("#8B4513"),
            CoreColor.Tan => Colors.Tan,
            CoreColor.Olive => Color.FromHtml("#808000"),
            CoreColor.Maroon => Colors.Maroon,
            
            // Accent colors
            CoreColor.Violet => Colors.Violet,
            CoreColor.Turquoise => Colors.Turquoise,
            CoreColor.Silver => Colors.Silver,
            
            _ => throw new ArgumentOutOfRangeException(nameof(coreColor), coreColor, "Unhandled CoreColor value.")
        };
    }
}