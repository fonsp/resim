// HudBase.cs
//
// Copyright 2013 Fons van der Plas
// Fons van der Plas, fonsvdplas@gmail.com

namespace GraphicsLibrary.Hud
{
	/* Dit is hetzelfde als RootNode, maar dan voor 2D (zie RootNode.cs)
	 */
	public class HudBase : HudElement
	{
		#region SingleTon
		private static HudBase instance;

		public static HudBase Instance
		{
			get { return instance ?? (instance = new HudBase("HudBase")); }
		}
		#endregion

		public HudBase(string name) : base(name)
		{
			
		}
	}
}