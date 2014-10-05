using System;
using System.Diagnostics;
using GraphicsLibrary;

namespace Resim.Program
{
	/* De Game class is opgesplitst in verschillende .cs-bestanden, vanwege de hoeveelheid regels code
	 */
	public partial class Game:GraphicsProgram
	{
		public Game(string[] arguments, bool enableLogging, string logFilename)
			: base(arguments, enableLogging, logFilename)
		{

		}

		public Game(string[] arguments)
			: this(arguments, true, "game.log")
		{

		}
		/* De start van het programma
		 * Na game.Run() pauzeert de thread, dus hier kan verder niets staan (zie InitGame en LoadResources).
		 */
		[STAThread]
		static void Main(string[] args)
		{

			using(Game game = new Game(args))
			{
				game.Run();
			}

			Debug.WriteLine("Closing..");
		}

	}
}
