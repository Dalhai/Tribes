using Godot;
using TribesOfDust.Core;

namespace TribesOfDust.UI.Pages
{
	public partial class MainPage : Node2D
	{
		public void OnPlayPressed() => Context.Get(this)?.Navigator.GoTo("play");

		public void OnEditorPressed() => Context.Get(this)?.Navigator.GoTo("editor");

		public void OnSettingsPressed() => Context.Get(this)?.Navigator.GoTo("settings");

		public void OnQuitPressed() => GetTree().Quit();
	}
}
