namespace GraphicsLibrary.Hud
{
	public class HudBase:HudElement
	{
		#region SingleTon
		private static HudBase instance;
		/// <summary>
		/// The primary HudElement in ReSim.
		/// </summary>
		public static HudBase Instance
		{
			get { return instance ?? (instance = new HudBase("HudBase")); }
		}
		#endregion

		public HudBase(string name)
			: base(name)
		{

		}
	}
}