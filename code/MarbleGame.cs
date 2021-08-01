using superterryball.ui;
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace superterryball
{

	/// <summary>
	/// This is your game class. This is an entity that is created serverside when
	/// the game starts, and is replicated to the client. 
	/// 
	/// You can use this to create things like HUDs and declare which player class
	/// to use for spawned players.
	/// 
	/// Your game needs to be registered (using [Library] here) with the same name 
	/// as your game addon. If it isn't then we won't be able to find it.
	/// </summary>
	[Library ( "superterryball" ) ]
	public partial class MarbleGame : Sandbox.Game
	{
		public static new MarbleGame Current => Game.Current as MarbleGame;

		[Net] public int FloorNumber { get; set; } = 1;
		[Net] public int FloorAmount { get; set; }
		[Net] public int PlayersFinished { get; set; }
		[Net] public float LevelTimer { get; set; }
		[Net] public bool GameActive { get; set; } = true;
		[Net] [Range(1,3, 0.01f, true)] public int Difficulty { get; set; }

		public MarbleGame()
		{
			if ( IsServer )
				new MinimalHudEntity();
		}

		[Net] public float CurrentTimer { get; set; } = 60f;
		bool setLevelTimer = true;
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if ( PlayersFinished >= Client.All.Count && IsServer )
				FloorEnd( 5f );

			//if ( Input.Pressed( InputButton.Slot0 ) && IsServer )
			//	FloorEnd( 0f );

			if ( setLevelTimer )
			{
				CurrentTimer = LevelTimer;
				setLevelTimer = false;
			}

			CurrentTimer = MathX.Clamp( CurrentTimer, 0f, float.MaxValue );

			if ( GameActive && IsServer )
			{
				CurrentTimer -= Time.Delta / Client.All.Count;
				if (CurrentTimer <= 0f && IsServer)
					FloorEnd( 5f );
			}

		}

		/// <summary>
		/// A client has joined the server. Make them a pawn to play with
		/// </summary>
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			var player = new MarblePlayer();
			client.Pawn = player;

			player.Respawn();
		}

		/// <summary>
		/// Transition to the next floor
		/// </summary>
		public async void FloorEnd( float delay )
		{
			Log.Info( "Changing levels..." );
			GameActive = false;
			PlayersFinished = 0;

			await Task.DelaySeconds( delay );

			CurrentTimer = LevelTimer;
			FloorNumber++;

			if ( FloorNumber > FloorAmount )
				FloorNumber = 1;

			foreach ( Client cl in Client.All )
			{
				(cl.Pawn as MarblePlayer).Respawn();
			}

			GameActive = true;
		}

		/// <summary>
		/// Finds all the spawnpoints, gets only the ones for the set floor, randomizes the spawnpoint, and sets the player's position to the spawnpoint
		/// </summary>
		public void GetSpawnPoint( Entity pawn )
		{
			if ( (pawn as MarblePlayer).isPlaying == false )
				return;

			var spawnpoint = Entity.All;
			spawnpoint.OfType<STBSpawnPoint>();             // get all SpawnPoint entities

			// sort out the bad ones
			var curatedSpawnpoint = new List<Entity>();
			foreach ( Entity point in spawnpoint )
				if (point is STBSpawnPoint)
					if ( (point as STBSpawnPoint).SpawnPointNum == FloorNumber )
						curatedSpawnpoint.Add( point );

			if ( curatedSpawnpoint == null )
			{
				Log.Warning( $"Couldn't find spawnpoint for {pawn}!" );
				return;
			}

			Entity finalPoint = Rand.FromList( curatedSpawnpoint ); // get a random one
			pawn.Position = finalPoint.Position;
		}
	}

	/// <summary>
	/// The setup entity for the game. This is for difficulty, etc.
	/// </summary>
	[Library("terryball_setup")]
	public partial class GameSetup : Sandbox.Entity
	{
		/// <summary>
		/// The difficulty level of the map. 1 = Beginner, 2 = Advanced, 3 = Expert
		/// </summary>
		[Property(Title = "Difficulty Level")] [Range( 1, 3, 0.01f, true )] public int Difficulty { get; set; }

		/// <summary>
		/// How many floors (levels) are in the map?
		/// </summary>
		[Property(Title = "Floor Amount")] public int FloorAmount { get; set; }

		/// <summary>
		/// How long will players have to complete each floor?
		/// </summary>
		[Property(Title = "Level Timer")] public int LevelTimer { get; set; }

		[Event.Tick]
		public void Tick()
		{
			MarbleGame.Current.Difficulty = Difficulty;
			MarbleGame.Current.FloorAmount = FloorAmount;
			MarbleGame.Current.LevelTimer = LevelTimer;
		}
	}

}
