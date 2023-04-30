using Godot;

namespace TribesOfDust.Core.Modes;

public partial class MainMode : Node2D
{
    void OnPlayPressed() => Context.Instance.Navigator.GoTo("play");
    void OnEditorPressed() => Context.Instance.Navigator.GoTo("editor");
    void OnSettingsPressed() => Context.Instance.Navigator.GoTo("settings");
    void OnQuitPressed() => GetTree().Quit();
}