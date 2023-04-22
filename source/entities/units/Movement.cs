using Godot;

namespace TribesOfDust.Core.Entities
{
	public partial class Movement : Resource
	{
		[Export(PropertyHint.Range, "0,10,1")]
		public double TundraCost = 1.0;

		[Export(PropertyHint.Range, "0,10,1")]
		public double DunesCost = 1.0;

		[Export(PropertyHint.Range, "0,10,1")]
		public double MountainsCost = 1.0;

		[Export(PropertyHint.Range, "0,10,1")]
		public double CanyonsCost = 1.0;
	}
}
