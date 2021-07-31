using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace superterryball.ui
{
	public partial class LevelTimer : Panel
	{
		public Label Label;

		public LevelTimer()
		{
			Label = Add.Label( "", "text" );
		}

		public override void Tick()
		{
			var player = Local.Pawn;
			if ( player == null ) return;

			Label.Text = $"{MarbleGame.Current.CurrentTimer.ToString( "F2" ).Replace(".", ":")}";
		}
	}
}
