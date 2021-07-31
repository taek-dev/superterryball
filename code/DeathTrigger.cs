using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace superterryball
{
	[Library( "trigger_marble_death" )]
	public partial class DeathTrigger : BaseTrigger
	{
		public override void StartTouch( Entity other )
		{
			if ( other is MarblePlayer client )
			{
				if ( client.isPlaying )
					client.OnKilled();
			}

			base.StartTouch( other );
		}
	}
}
