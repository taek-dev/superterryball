using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace superterryball
{
	/// <summary>
	/// The position where a player's marble can spawn
	/// </summary>
	[Library( "info_marble_start" )]
	[Hammer.EditorModel( "models/player_marble.vmdl" )]
	[Hammer.EntityTool( "Player Marble Spawnpoint", "Player", "Defines a point where the player can (re)spawn" )]
	public class STBSpawnPoint : Entity
	{
		/// <summary>
		/// The number spawn point that it is
		/// </summary>
		[Property( Title = "Spawn Point Number" )] public int SpawnPointNum { get; set; } = 1;
	}
}
