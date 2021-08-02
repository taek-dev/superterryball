using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace superterryball
{
	public class SpectateCamera : Camera
	{
		public Client cl;
		private float orbitDistance = 115f;

		public override void Update()
		{
			if ( cl == null )
				return;

			// rotation
			var targetRot = Input.Rotation;
			Rot = targetRot;

			// position
			Pos = cl.Pawn.Position;
			var targetPos = Pos + Input.Rotation.Right;
			targetPos += Input.Rotation.Forward * -orbitDistance;

			var tr = Trace.Ray( Pos, targetPos )
					.Ignore( cl.Pawn )
					.WithoutTags( "player" )
					.WorldOnly()
					.Radius( 8 )
					.Run();

			Pos = tr.EndPos;

			ZFar = 5000f;
			FieldOfView = 90;
			Viewer = null;
		}
	}
}
