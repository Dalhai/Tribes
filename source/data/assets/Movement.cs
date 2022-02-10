using Godot;
using TribesOfDust.Hex;

namespace TribesOfDust.Data.Assets
{
	public class Movement : Resource
	{
		[Export(PropertyHint.Range, "0,10,1")]
		int TundraCost = 0;

		[Export(PropertyHint.Range, "0,10,1")]
		int DunesCost = 0;

		[Export(PropertyHint.Range, "0,10,1")]
		int MountainsCost = 0;

		[Export(PropertyHint.Range, "0,10,1")]
		int CanyonsCost = 0;
	}
}
