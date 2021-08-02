using Sandbox;

namespace superterryball
{
	public class DeathCamera : Camera
	{
		private float orbitDistance = 115f;
		bool atPlayer;

		public override void Update()
		{
			var pawn = Local.Pawn as AnimEntity;
			var client = Local.Client;

			if ( pawn == null )
				return;

			Vector3 dir;

			if ( !atPlayer )
			{
				Pos = pawn.Position;
				var targetPos = Pos + Input.Rotation.Right;
				targetPos += Input.Rotation.Forward * -orbitDistance;
				Pos = targetPos;
				var targetRot = Input.Rotation;
				Rot = targetRot;
				atPlayer = true;
			}

			dir = (pawn.Position - this.Pos).Normal;
			var postTargetRot = Rotation.LookAt( dir );
			Rot = Rotation.Lerp( Rot, postTargetRot, Time.Delta * 10f );

			ZFar = 5000f;
			FieldOfView = 90;
			Viewer = null;
		}	
	}
}
