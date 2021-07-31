using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace superterryball.ui
{
	public partial class FloorCounter : Panel
	{
		public Label Label;

		public FloorCounter()
		{
			Label = Add.Label( "" );
		}

		public override void Tick()
		{
			var player = Local.Pawn;
			if ( player == null ) return;

			Label.Text = $"FLOOR   {MarbleGame.Current.FloorNumber}";
		}
	}
}
