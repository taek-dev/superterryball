﻿using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Sandbox.UI;
using Sandbox;

namespace superterryball.ui
{
	public class STBBaseNameTag : Panel
	{
		public Label NameLabel;
		public Image Avatar;

		Player player;

		public STBBaseNameTag( Player player )
		{
			this.player = player;

			var client = player.GetClientOwner();

			NameLabel = Add.Label( $"{client.Name}" );
			Avatar = Add.Image( $"avatar:{client.SteamId}" );
		}

		public virtual void UpdateFromPlayer( Player player )
		{
			// Nothing to do unless we're showing health and shit
		}
	}

	public class STBNameTags : Panel
	{
		Dictionary<Player, STBBaseNameTag> ActiveTags = new Dictionary<Player, STBBaseNameTag>();

		public float MaxDrawDistance = 400;
		public int MaxTagsToShow = 5;

		public STBNameTags()
		{
			StyleSheet.Load( "/ui/STBNameTags.scss" );
		}

		public override void Tick()
		{
			base.Tick();


			var deleteList = new List<Player>();
			deleteList.AddRange( ActiveTags.Keys );

			int count = 0;
			foreach ( var player in Entity.All.OfType<Player>().OrderBy( x => Vector3.DistanceBetween( x.Position, CurrentView.Position ) ) )
			{
				if ( UpdateNameTag( player ) )
				{
					deleteList.Remove( player );
					count++;
				}

				if ( count >= MaxTagsToShow )
					break;
			}

			foreach ( var player in deleteList )
			{
				ActiveTags[player].Delete();
				ActiveTags.Remove( player );
			}

		}

		public virtual STBBaseNameTag CreateNameTag( Player player )
		{
			if ( player.GetClientOwner() == null )
				return null;

			var tag = new STBBaseNameTag( player );
			tag.Parent = this;
			return tag;
		}

		public bool UpdateNameTag( Player player )
		{
			// Don't draw local player
			if ( player == Local.Pawn )
				return false;

			if ( player.LifeState != LifeState.Alive )
				return false;

			//
			// Where we putting the label, in world coords
			//
			var head = player.GetAttachment( "hat" ) ?? new Transform( player.EyePos );

			var labelPos = head.Position + head.Rotation.Up * 5;


			//
			// Are we too far away?
			//
			float dist = labelPos.Distance( CurrentView.Position );
			if ( dist > MaxDrawDistance )
				return false;

			//
			// Are we looking in this direction?
			//
			var lookDir = (labelPos - CurrentView.Position).Normal;
			if ( CurrentView.Rotation.Forward.Dot( lookDir ) < 0.5 )
				return false;

			// TODO - can we see them

			MaxDrawDistance = 400;

			// Max Draw Distance

			var alpha = dist.LerpInverse( MaxDrawDistance, MaxDrawDistance * 0.1f, true );

			// If I understood this I'd make it proper function
			var objectSize = 0.05f / dist / (2.0f * MathF.Tan( (CurrentView.FieldOfView / 2.0f).DegreeToRadian() )) * 1500.0f;

			objectSize = objectSize.Clamp( 0.05f, 1.0f );

			if ( !ActiveTags.TryGetValue( player, out var tag ) )
			{
				tag = CreateNameTag( player );
				if ( tag != null )
				{
					ActiveTags[player] = tag;
				}
			}

			tag.UpdateFromPlayer( player );

			var screenPos = labelPos.ToScreen();

			tag.Style.Left = Length.Fraction( screenPos.x );
			tag.Style.Top = Length.Fraction( screenPos.y );
			tag.Style.Opacity = alpha;

			var transform = new PanelTransform();
			transform.AddTranslateY( Length.Fraction( -3.2f ) );
			transform.AddScale( objectSize * 1.2f );
			transform.AddTranslateX( Length.Fraction( -0.5f ) );

			tag.Style.Transform = transform;
			tag.Style.Dirty();

			return true;
		}
	}
}
