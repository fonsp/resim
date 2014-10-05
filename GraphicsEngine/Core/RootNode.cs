// RootNode.cs
//
// Copyright 2013 Fons van der Plas
// Fons van der Plas, fonsvdplas@gmail.com

namespace GraphicsLibrary.Core
{
	/* Aan deze node moeten alle andere node verbonden zijn
	 * anders krijgen ze geen Update() en StartRender()
	 * 
	 * De Camera class werkt ook als RootNode (zie RenderWindow.cs)
	 */
	public class RootNode : Node
	{
		#region SingleTon
		private static RootNode instance;
		public static RootNode Instance
		{
			get { return instance ?? (instance = new RootNode()); }
		}
		#endregion

		public RootNode() : base("root")
		{
		}
	}
}

