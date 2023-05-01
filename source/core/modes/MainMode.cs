using Godot;

namespace TribesOfDust.Core.Modes;

public partial class MainMode : Node2D, IUnique<MainMode>
{
    public static MainMode? Instance { get; private set; }

    public override void _EnterTree()
    {
        Instance = this;
        base._EnterTree();
    }

    public override void _ExitTree()
    {
        Instance = null;
        base._ExitTree();
    }

    void OnPlayPressed() => Context.Instance.Navigator.GoTo("play");
    void OnEditorPressed() => Context.Instance.Navigator.GoTo("editor");
    void OnSettingsPressed() => Context.Instance.Navigator.GoTo("settings");
    void OnQuitPressed() => GetTree().Quit();
}