using Godot;

namespace TribesOfDust.Core.Modes;

public partial class MainMode : Node2D
{
    void OnPlayPressed() => Context.GetRootContext(this)?.Navigator.GoTo("play");
    void OnEditorPressed() => Context.GetRootContext(this)?.Navigator.GoTo("editor");
    void OnSettingsPressed() => Context.GetRootContext(this)?.Navigator.GoTo("settings");
    void OnQuitPressed() => GetTree().Quit();
}