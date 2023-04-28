using Godot;
using TribesOfDust.Core;

namespace TribesOfDust.UI.Pages
{
	public partial class MainPage : Node2D
	{
		void OnPlayPressed() => Context.GetRootContext(this)?.Navigator.GoTo("play");
		void OnEditorPressed() => Context.GetRootContext(this)?.Navigator.GoTo("editor");
		void OnSettingsPressed() => Context.GetRootContext(this)?.Navigator.GoTo("settings");
		void OnQuitPressed() => GetTree().Quit();
	}
}
