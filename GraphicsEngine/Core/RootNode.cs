namespace GraphicsLibrary.Core
{
	public class RootNode:Node
	{
		#region SingleTon
		private static RootNode instance;
		public static RootNode Instance
		{
			get { return instance ?? (instance = new RootNode()); }
		}
		#endregion

		public RootNode()
			: base("root")
		{
		}
	}
}