using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace superterryball.ui
{
	public partial class DifficultyIcon : Panel
	{
		public Image image;

		public DifficultyIcon()
		{
			StyleSheet.Load( "ui/difficultyicon.scss" );
			image = Add.Image( "" );
		}

		public override void Tick()
		{
			MarbleGame.Current.Difficulty = 1;
			if ( MarbleGame.Current.Difficulty == 1 )
				image.Texture = Texture.Load( "materials/ui/icons/beginner.png" );
			else if ( MarbleGame.Current.Difficulty == 2 )
				image.Texture = Texture.Load( "materials/ui/icons/advanced.png" );
			else if ( MarbleGame.Current.Difficulty == 3 )
				image.Texture = Texture.Load( "materials/ui/icons/expert.png" );
		}
	}
}
