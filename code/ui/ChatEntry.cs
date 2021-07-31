using Sandbox.UI.Construct;
using Sandbox.UI;
using Sandbox;

namespace superterryball.ui
{
	public partial class STBChatEntry : Panel
	{
		public Label NameLabel { get; internal set; }
		public Label Message { get; internal set; }
		public Image Avatar { get; internal set; }

		public RealTimeSince TimeSinceBorn = 0;

		private Color playingColor;
		private Color notPlayingColor;

		public STBChatEntry()
		{
			playingColor = Color.Cyan;
			notPlayingColor = Color.Yellow;

			if ( (Local.Client.Pawn as MarblePlayer).isPlaying )
				Style.BorderColor = playingColor;
			else
				Style.BorderColor = notPlayingColor;

			Avatar = Add.Image();
			NameLabel = Add.Label( "Name", "name" );
			if ( (Local.Client.Pawn as MarblePlayer).isPlaying )
				NameLabel.Style.FontColor = playingColor;
			else
				NameLabel.Style.FontColor = notPlayingColor;

			Message = Add.Label( "Message", "message" );
		}

		public override void Tick()
		{
			base.Tick();

			if ( TimeSinceBorn > 15 )
			{
				Delete();
			}
		}
	}

	public partial class STBJoinEntry : Panel
	{
		public Label NameLabel { get; internal set; }
		public Label Message { get; internal set; }
		public Image Avatar { get; internal set; }

		public RealTimeSince TimeSinceBorn = 0;

		public STBJoinEntry()
		{
			Style.BorderColor = Color.Green;

			Avatar = Add.Image();
			NameLabel = Add.Label( "Name", "name" );
			Message = Add.Label( "Message", "message" );
		}

		public override void Tick()
		{
			base.Tick();

			if ( TimeSinceBorn > 15 )
			{
				Delete();
			}
		}
	}
}
