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
	public partial class SpeedCounter : Panel
	{
		public Label text;

		public SpeedCounter()
		{
			text = Add.Label( "" );
		}

		public override void Tick()
		{
			var player = Local.Pawn;
			if ( player == null ) return;

			if ( !(player as MarblePlayer).Spectating )
				text.Text = $"{(player as MarblePlayer).speedString} mph";
			else if ( Client.All.Count > 1 )
				text.Text = $"{(player as MarblePlayer).spectateSpeedString} mph";
		}
	}
}
