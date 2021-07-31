using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace superterryball
{
	[Library( "trigger_end_goal" )]
	public partial class EndGoal : BaseTrigger
	{
		public override void StartTouch( Entity other )
		{
			if ( other is MarblePlayer client )
			{
				if ( client.isPlaying )
					client.OnWin();
			}

			base.StartTouch( other );
		}
	}
}
