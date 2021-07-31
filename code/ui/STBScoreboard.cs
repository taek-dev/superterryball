
using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace superterryball.ui
{
	public partial class STBScoreboard<T> : Panel where T : STBScoreboardEntry, new()
	{
		public Panel Canvas { get; protected set; }
		Dictionary<int, T> Entries = new ();

		public Panel Header { get; protected set; }

		public STBScoreboard()
		{
			StyleSheet.Load( "/ui/STBScoreboard.scss" );
			AddClass( "scoreboard" );

			AddHeader();

			Canvas = Add.Panel( "canvas" );

			PlayerScore.OnPlayerAdded += AddPlayer;
			PlayerScore.OnPlayerUpdated += UpdatePlayer;
			PlayerScore.OnPlayerRemoved += RemovePlayer;

			foreach ( var player in PlayerScore.All )
			{
				AddPlayer( player );
			}
		}

		public override void Tick()
		{
			base.Tick();
			SetClass( "open", Input.Down( InputButton.Score ) );
		}


		protected virtual void AddHeader() 
		{
			Header = Add.Panel( "header" );
			Header.Add.Label( "Name", "name" );
			Header.Add.Label( "Wins", "wins" );
			Header.Add.Label( "Deaths", "deaths" );
		}

		protected virtual void AddPlayer( PlayerScore.Entry entry )
		{
			var p = Canvas.AddChild<T>();
			p.UpdateFrom( entry );

			Entries[entry.Id] = p;
		}

		protected virtual void UpdatePlayer( PlayerScore.Entry entry )
		{
			if ( Entries.TryGetValue( entry.Id, out var panel ) )
			{
				panel.UpdateFrom( entry );
			}
		}

		protected virtual void RemovePlayer( PlayerScore.Entry entry )
		{
			if ( Entries.TryGetValue( entry.Id, out var panel ) )
			{
				panel.Delete();
				Entries.Remove( entry.Id );
			}
		}
	}

	public partial class STBScoreboardEntry : Panel
	{
		public PlayerScore.Entry Entry;

		public Label PlayerName;
		public Label Wins;
		public Label Deaths;


		public STBScoreboardEntry()
		{
			AddClass( "entry" );

			PlayerName = Add.Label( "PlayerName", "name" );
			Wins = Add.Label( "", "wins" );
			Deaths = Add.Label( "", "deaths" );
		}

		public virtual void UpdateFrom( PlayerScore.Entry entry )
		{
			Entry = entry;
			PlayerName.Text = entry.GetString( "name" );
			Wins.Text = entry.Get<int>( "wins", 0 ).ToString();
			Deaths.Text = entry.Get<int>( "deaths", 0 ).ToString();
			SetClass( "me", Local.Client != null && entry.Get<ulong>( "steamid", 0 ) == Local.Client.SteamId );
		}
	}
}
