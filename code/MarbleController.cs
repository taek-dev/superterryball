using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace superterryball
{
	public partial class MarbleController : BasePlayerController
	{
		public override void Simulate()
		{
			// get the velocity
			Vector3 targetForward = Input.Rotation.Forward;
			targetForward.z = 0;

			Vector3 vel = (targetForward * Input.Forward) + (Input.Rotation.Left * Input.Left);
			vel = vel.Normal * 400;
			Velocity += vel * Time.Delta;

			// clamp the velocity
			//Velocity = new Vector3( MathX.Clamp( Velocity.x, -1000, 1000 ), MathX.Clamp( Velocity.y, -1000, 1000 ), Velocity.z );
		}
	}
}
