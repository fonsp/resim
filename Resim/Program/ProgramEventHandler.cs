using System;
using System.Globalization;
using System.Threading;
using GraphicsLibrary;
using GraphicsLibrary.Hud;
using OpenTK;

namespace Resim.Program
{
	/* Hier staan alle event handlers
	 */
	public partial class Game
	{
		/* Deze event wordt opgeroepen als een command in de console wordt getypt
		 */
		private void DebugInputReceived(object sender, HudDebugInputEventArgs e)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			if(!String.IsNullOrEmpty(e.InputArray[0]))
			{
				switch(e.InputArray[0])
				{
					case "exit":
					case "stop":
					case "close":
						RenderWindow.Instance.Exit();
						break;
					case "set":
						if(!String.IsNullOrEmpty(e.InputArray[1]))
						{
							switch(e.InputArray[1])
							{
								case "timeMult":
									if(!String.IsNullOrEmpty(e.InputArray[2]))
									{
										RenderWindow.Instance.timeMultiplier = Convert.ToDouble(e.InputArray[2]);
										hudDebug.AddLine("timeMult was set to " + RenderWindow.Instance.timeMultiplier);
									}
									break;
								case "walkSpeed":
									if(!String.IsNullOrEmpty(e.InputArray[2]))
									{
										walkSpeed = (int)Convert.ToDouble(e.InputArray[2]);
										hudDebug.AddLine("walkSpeed was set to " + walkSpeed);
									}
									break;
							}
						}
						break;
					case "reset":
						if(!String.IsNullOrEmpty(e.InputArray[1]))
						{
							switch(e.InputArray[1])
							{
								case "timeMult":
									RenderWindow.Instance.timeMultiplier = 1;
									hudDebug.AddLine("timeMult was reset to " + RenderWindow.Instance.timeMultiplier);

									break;
								case "walkSpeed":
									walkSpeed = 400;
									hudDebug.AddLine("walkSpeed was reset to " + walkSpeed);

									break;
							}
						}
						break;
					case "get":
						if(!String.IsNullOrEmpty(e.InputArray[1]))
						{
							switch(e.InputArray[1])
							{
								case "timeMult":
									hudDebug.AddLine("timeMult = " + RenderWindow.Instance.timeMultiplier);
									break;
								case "walkSpeed":
									hudDebug.AddLine("walkSpeed = " + walkSpeed);
									break;
							}
						}
						break;
					case "clear":
						hudDebug.ClearScreen();
						break;
					case "reload":
						config.Reload();
						break;

				}
			}
		}

		/* Deze event wordt opgeroepen als een letter wordt getypt
		 * Dit is niet heel handig voor een spel, omdat e.KeyChar afhankelijk is van de taal waarin het toetsenbord is ingesteld
		 */
		private void HandleKeyPress(object sender, KeyPressEventArgs e)
		{
			//Console.WriteLine("--"+e.KeyChar+"--");
			if(e.KeyChar == '`' || e.KeyChar == '~' || e.KeyChar == '	') //Tilda werkt niet op europese toetsenborden, daarom werkt tab ook
			{
				if(hudDebug.enabled)
				{
					hudDebug.enabled = false;
					hudDebug.isVisible = false;
				}
				else
				{
					hudDebug.enabled = true;
					hudDebug.isVisible = true;
					if(hudDebug.input.Length > 0)
					{
						hudDebug.input = hudDebug.input.Remove(hudDebug.input.Length - 1);
					}
				}
			}
		}
	}
}