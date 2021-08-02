using Sandbox;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace superterryball
{
	public class MarbleCamera : Camera
	{
		private float orbitDistance = 115f;
		private bool resetRotation = true;

		public override void Update()
		{
			var pawn = Local.Pawn as AnimEntity;

			if ( pawn == null )
				return;

			// rotation
			var targetRot = Input.Rotation;
			Rot = targetRot;

			// position
			Pos = pawn.Position;
			var targetPos = Pos + Input.Rotation.Right;
			targetPos += Input.Rotation.Forward * -orbitDistance;

			var tr = Trace.Ray( Pos, targetPos )
					.Ignore( pawn )
					.WorldOnly()
					.Radius( 8 )
					.Run();

			Pos = tr.EndPos;

			ZFar = 5000f;
			FieldOfView = 90;
			Viewer = null;
		}

		public override void BuildInput( InputBuilder input )
		{
			if ( resetRotation )
			{
				input.ViewAngles = Rotation.From( 25, 90, 0 ).Angles();
				resetRotation = false;
			}
				

			base.BuildInput( input );
		}
	}
}
