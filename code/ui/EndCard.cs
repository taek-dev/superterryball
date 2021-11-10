
using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

namespace superterryball.ui
{
	public partial class Endcard<T> : Panel where T : EndCardEntry, new()
	{
		public Panel Canvas { get; protected set; }
		Dictionary<Client, T> Rows = new();

		public Panel Header { get; protected set; }
		public Label Heading { get; set; }

		public Endcard()
		{
			StyleSheet.Load( "/ui/EndCard.scss" );
			AddClass( "scoreboard" );

			AddHeader();
			Heading = Add.Label( "", "heading" );
			Heading.Text = "Player Rankings:";

			Canvas = Add.Panel( "canvas" );
		}

		public override void Tick()
		{
			base.Tick();

			SetClass( "open", !MarbleGame.Current.GameActive );

			if ( !IsVisible )
				return;

			//
			// Clients that were added
			//
			foreach ( var client in Client.All.Except( Rows.Keys ) )
			{
				var entry = AddClient( client );
				Rows[client] = entry;
			}

			foreach ( var client in Rows.Keys.Except( Client.All ) )
			{
				if ( Rows.TryGetValue( client, out var row ) )
				{
					row?.Delete();
					Rows.Remove( client );
				}
			}
		}

		protected virtual T AddClient( Client entry )
		{
			var p = Canvas.AddChild<T>();
			p.Client = entry;
			return p;
		}

		protected virtual void AddHeader()
		{
			Header = Add.Panel( "header" );
			Header.Add.Label( "Name", "name" );
			Header.Add.Label( "Wins", "wins" );
			Header.Add.Label( "Deaths", "deaths" );
		}
	}

	public partial class EndCardEntry : Panel
	{
		public Client Client;

		public Label PlayerName;
		public Label Wins;
		public Label Deaths;

		public EndCardEntry()
		{
			AddClass( "entry" );

			PlayerName = Add.Label( "PlayerName", "name" );
			Wins = Add.Label( "", "wins" );
			Deaths = Add.Label( "", "deaths" );
		}

		RealTimeSince TimeSinceUpdate = 0;

		public override void Tick()
		{
			base.Tick();

			if ( !IsVisible )
				return;

			if ( !Client.IsValid() )
				return;

			if ( TimeSinceUpdate < 0.1f )
				return;

			TimeSinceUpdate = 0;
			UpdateData();
		}

		public virtual void UpdateData()
		{
			PlayerName.Text = Client.Name;
			Deaths.Text = Client.GetInt( "deaths" ).ToString();
			Wins.Text = Client.GetInt( "wins" ).ToString();
			SetClass( "me", Client == Local.Client );
		}

		public virtual void UpdateFrom( Client client )
		{
			Client = client;
			UpdateData();
		}
	}
}
