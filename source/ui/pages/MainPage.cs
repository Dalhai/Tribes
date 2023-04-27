using Godot;
using TribesOfDust.Core;

namespace TribesOfDust.UI.Pages
{
	public partial class MainPage : Node2D
	{
		void OnPlayPressed() => Context.Get(this)?.Navigator.GoTo("play");
		void OnEditorPressed() => Context.Get(this)?.Navigator.GoTo("editor");
		void OnSettingsPressed() => Context.Get(this)?.Navigator.GoTo("settings");
		void OnQuitPressed() => GetTree().Quit();
	}
}
