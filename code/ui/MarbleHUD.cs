using Sandbox.UI;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace superterryball.ui
{
	/// <summary>
	/// This is the HUD entity. It creates a RootPanel clientside, which can be accessed
	/// via RootPanel on this entity, or Local.Hud.
	/// </summary>
	public partial class MinimalHudEntity : Sandbox.HudEntity<RootPanel>
	{
		public MinimalHudEntity()
		{
			if ( IsClient )
			{
				RootPanel.StyleSheet.Load( "ui/marblehud.scss" );
				
				RootPanel.AddChild<SpeedCounter>();
				RootPanel.AddChild<FloorCounter>();
				RootPanel.AddChild<DifficultyIcon>();
				RootPanel.AddChild<LevelTimer>();

				RootPanel.AddChild<STBChatBox>();
				RootPanel.AddChild<STBNameTags>();
				RootPanel.AddChild<STBScoreboard<STBScoreboardEntry>>();
				RootPanel.AddChild<Endcard<EndCardEntry>>();

				RootPanel.AddChild<VoiceList>();
			}
		}
	}

}
