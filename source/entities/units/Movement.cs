using Godot;
using TribesOfDust.Core.Currencies;

namespace TribesOfDust.Core.Entities
{
	public class Movement : Resource
	{
		[Export(PropertyHint.Range, "0,10,1")]
		int TundraCost
		{
			get => _tundraCost.amount;
			set => _tundraCost = new Value(Currency.Water, value);
		}

		[Export(PropertyHint.Range, "0,10,1")]
		int DunesCost
		{
			get => _dunesCost.amount;
			set => _dunesCost = new Value(Currency.Water, value);
		}

		[Export(PropertyHint.Range, "0,10,1")]
		int MountainsCost
		{
			get => _mountainsCost.amount;
			set => _mountainsCost = new Value(Currency.Water, value);
		}

		[Export(PropertyHint.Range, "0,10,1")]
		int CanyonsCost
		{
			get => _canyonsCost.amount;
			set => _canyonsCost = new Value(Currency.Water, value);
		}

		private Value _tundraCost = new Value(Currency.Water, 1);
		private Value _dunesCost = new Value(Currency.Water, 1);
		private Value _mountainsCost = new Value(Currency.Water, 1);
		private Value _canyonsCost = new Value(Currency.Water, 1);
	}
}
