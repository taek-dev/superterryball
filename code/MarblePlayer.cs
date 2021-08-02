using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

namespace superterryball
{
	public partial class MarblePlayer : Player
	{
		private ModelEntity citizenModel;
		private bool isColored { get; set; }
		[Net] public string speedString { get; set; }
		[Net] public string spectateSpeedString { get; set; }
		[Net] public bool isPlaying { get; set; }
		[Net] public bool Spectating { get; set; }
		[Net] public int NumOfWins { get; set; }
		[Net] public int NumOfDeaths { get; set; }


		/// <summary>
		/// It flippin' (re)spawns the player. This does a lot of stuff
		/// </summary>
		public override void Respawn()
		{
			SetModel( "models/player_marble.vmdl" );

			if ( citizenModel == null )
			{
				citizenModel = new ModelEntity();
				citizenModel.SetModel( "models/citizen/citizen.vmdl" );
				citizenModel.Parent = this;
				citizenModel.Scale *= .375f;
				citizenModel.Position += new Vector3( 0, 0, -14.8f );
			}

			Dress();

			Ascend = false;
			Spectating = false;
			Camera = new MarbleCamera();
			Controller = new MarbleController();

			isPlaying = true;

			PhysicsEnabled = true;
			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			// base respawn stuff
			LifeState = LifeState.Alive;
			Health = 100;

			CreateHull();

			MarbleGame.Current.GetSpawnPoint( this );
			ResetInterpolation();
			//

			CollisionGroup = CollisionGroup.Player;
			SetInteractsWith( CollisionLayer.Trigger );
			SetInteractsExclude( CollisionLayer.Player );
			Tags.Add( "player" );
			SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );

			if (IsServer)
			{
				Position += new Vector3( 0, 0, 25 );
				Velocity = Vector3.Zero;
				Velocity += new Vector3( 0, 0, -400 );
				Rotation = Rotation.From
					( Rand.Float( 0, 360 ),
					Rand.Float( 0, 360 ),
					Rand.Float( 0, 360 ) );
			}

		}

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			SimulateActiveChild( cl, ActiveChild );

			cl.SetScore( "wins", NumOfWins );
			cl.SetScore( "deaths", NumOfDeaths );

			// changes the marble skin
			if ( !isColored )
			{
				SetMaterialGroup( Rand.Int( 1, 10 ) );
				isColored = true;
			}

			if ( IsServer )
				speedString = (Velocity.Length / 17.6).ToString( "F0" );

			if ( Ascend )
				PhysicsGroup.AddVelocity( Vector3.Up * 30 );

			if ( Spectating && MarbleGame.Current.GameActive )
				Spectate();

			if ( IsServer )
			if ( MarbleGame.Current.CurrentTimer <= 0 && isPlaying )
				OnLose();
		}

		public override void OnKilled()
		{
			base.OnKilled();

			Controller = null;
			Camera = new DeathCamera();

			if ( isPlaying )
				NumOfDeaths++;
		}

		[Net] private bool Ascend { get; set; }

		/// <summary>
		/// The player just hit the goal, nice
		/// </summary>
		public async void OnWin()
		{
			MarbleGame.Current.PlayersFinished++;
			NumOfWins++;
			isPlaying = false;
			Ascend = true;

			Controller = null;
			Camera = new DeathCamera();

			await Task.DelaySeconds( 4f );

			if ( MarbleGame.Current.GameActive )
				Spectating = true;
		}

		/// <summary>
		/// The player ran out of time! What a pity
		/// </summary>
		public void OnLose()
		{
			isPlaying = false;
			Controller = null;

			Log.Info( "You Lose!" );
		}

		int selectedSpectateTarget;

		/// <summary>
		/// Spectate other active players
		/// </summary>
		public void Spectate()
		{
			if ( Client.All.Count == 1 )
				return;


			var activePlayerList = new List<Client>();

			foreach ( Client cl in Client.All )
			{
				if ( (cl.Pawn as MarblePlayer).isPlaying )
					activePlayerList.Add( cl );
			}

			// increasing target num
			if ( Input.Pressed( InputButton.Attack1 ) )
				selectedSpectateTarget++;

			// overflow
			if ( selectedSpectateTarget >= activePlayerList.ToArray().Length )
				selectedSpectateTarget = 0;
			// underflow
			if ( selectedSpectateTarget < 0 )
				selectedSpectateTarget = activePlayerList.ToArray().Length;

			if ( activePlayerList.ToArray().Length == 0 )
				return;

			Client currentSpectateTarget;
			currentSpectateTarget = activePlayerList[selectedSpectateTarget];

			if (IsServer)
				spectateSpeedString = (currentSpectateTarget.Pawn.Velocity.Length / 17.6).ToString( "F0" );

			var cam = new SpectateCamera();
			cam.cl = currentSpectateTarget;
			Camera = cam;
		}

		// camera magic

		public override void PostCameraSetup( ref CameraSetup setup )
		{
			base.PostCameraSetup( ref setup );
			AddCameraEffects( ref setup );
		}

		float horizontalLean;
		float verticalLean;

		private void AddCameraEffects( ref CameraSetup setup )
		{
			if ( Controller != null )
            {
				// Camera lean
				horizontalLean = horizontalLean.LerpTo( Input.Left * 10f, Time.Delta * 15.0f );
				verticalLean = verticalLean.LerpTo( -Input.Forward * 8f, Time.Delta * 15.0f );
			}

			if ( Spectating )
			{
				horizontalLean = 1f;
				verticalLean = 1f;
			}

			// apply camera lean
			setup.Rotation *= Rotation.FromRoll( horizontalLean );
			setup.Rotation *= Rotation.FromPitch( verticalLean );
			setup.Position += new Vector3( 0, 0, verticalLean * 1.5f );
			setup.Position += setup.Rotation.Forward * verticalLean;
		}


		// dressing up

		ModelEntity pants;
		ModelEntity jacket;
		ModelEntity shoes;
		ModelEntity hat;

		bool dressed = false;

		/// <summary>
		/// Give the player some random swag, until player customization is added eventually.
		/// </summary>
		public void Dress()
		{
			if ( dressed ) return;
			dressed = true;

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
				"models/citizen_clothes/trousers/trousers.jeans.vmdl",
				"models/citizen_clothes/trousers/trousers.lab.vmdl",
				"models/citizen_clothes/trousers/trousers.police.vmdl",
				"models/citizen_clothes/trousers/trousers.smart.vmdl",
				"models/citizen_clothes/trousers/trousers.smarttan.vmdl",
				"models/citizen/clothes/trousers_tracksuit.vmdl",
				"models/citizen_clothes/trousers/trousers_tracksuitblue.vmdl",
				"models/citizen_clothes/trousers/trousers_tracksuit.vmdl",
				"models/citizen_clothes/shoes/shorts.cargo.vmdl",
			} );

				pants = new ModelEntity();
				pants.SetModel( model );
				pants.SetParent( citizenModel, true );
				pants.EnableShadowInFirstPerson = true;
				pants.EnableHideInFirstPerson = true;

				citizenModel.SetBodyGroup( "Legs", 1 );
			}

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
				"models/citizen_clothes/jacket/labcoat.vmdl",
				"models/citizen_clothes/jacket/jacket.red.vmdl",
				"models/citizen_clothes/jacket/jacket.tuxedo.vmdl",
				"models/citizen_clothes/jacket/jacket_heavy.vmdl",
			} );

				jacket = new ModelEntity();
				jacket.SetModel( model );
				jacket.SetParent( citizenModel, true );
				jacket.EnableShadowInFirstPerson = true;
				jacket.EnableHideInFirstPerson = true;

				var propInfo = jacket.GetModel().GetPropData();
				if ( propInfo.ParentBodyGroupName != null )
				{
					citizenModel.SetBodyGroup( propInfo.ParentBodyGroupName, propInfo.ParentBodyGroupValue );
				}
				else
				{
					citizenModel.SetBodyGroup( "Chest", 0 );
				}
			}

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
				"models/citizen_clothes/shoes/trainers.vmdl",
				"models/citizen_clothes/shoes/shoes.workboots.vmdl"
			} );

				shoes = new ModelEntity();
				shoes.SetModel( model );
				shoes.SetParent( citizenModel, true );
				shoes.EnableShadowInFirstPerson = true;
				shoes.EnableHideInFirstPerson = true;

				citizenModel.SetBodyGroup( "Feet", 1 );
			}

			if ( true )
			{
				var model = Rand.FromArray( new[]
				{
				"models/citizen_clothes/hat/hat_hardhat.vmdl",
				"models/citizen_clothes/hat/hat_woolly.vmdl",
				"models/citizen_clothes/hat/hat_securityhelmet.vmdl",
				"models/citizen_clothes/hair/hair_malestyle02.vmdl",
				"models/citizen_clothes/hair/hair_femalebun.black.vmdl",
				"models/citizen_clothes/hat/hat_beret.red.vmdl",
				"models/citizen_clothes/hat/hat.tophat.vmdl",
				"models/citizen_clothes/hat/hat_beret.black.vmdl",
				"models/citizen_clothes/hat/hat_cap.vmdl",
				"models/citizen_clothes/hat/hat_leathercap.vmdl",
				"models/citizen_clothes/hat/hat_leathercapnobadge.vmdl",
				"models/citizen_clothes/hat/hat_securityhelmetnostrap.vmdl",
				"models/citizen_clothes/hat/hat_service.vmdl",
				"models/citizen_clothes/hat/hat_uniform.police.vmdl",
				"models/citizen_clothes/hat/hat_woollybobble.vmdl",
			} );

				hat = new ModelEntity();
				hat.SetModel( model );
				hat.SetParent( citizenModel, true );
				hat.EnableShadowInFirstPerson = true;
				hat.EnableHideInFirstPerson = true;
			}
		}
	}
}
