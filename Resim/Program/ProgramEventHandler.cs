using System;
using System.Globalization;
using System.Threading;
using OpenTK;
using GraphicsLibrary;
using GraphicsLibrary.Hud;
using OpenTK.Graphics;

namespace Resim.Program
{
	public partial class Game
	{
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
						if (e.InputArray.Length > 1 && !String.IsNullOrEmpty(e.InputArray[1]))
						{
							switch (e.InputArray[1])
							{
								case "timeMult":
									if (!String.IsNullOrEmpty(e.InputArray[2]))
									{
										RenderWindow.Instance.timeMultiplier = Convert.ToDouble(e.InputArray[2]);
										hudDebug.AddLine("timeMult was set to " + RenderWindow.Instance.timeMultiplier);
									}
									break;
								case "walkSpeed":
									if (!String.IsNullOrEmpty(e.InputArray[2]))
									{
										walkSpeed = (int) Convert.ToDouble(e.InputArray[2]);
										hudDebug.AddLine("walkSpeed was set to " + walkSpeed);
									}
									break;
								case "VSync":
									if (!String.IsNullOrEmpty(e.InputArray[2]))
									{
										try
										{
											RenderWindow.Instance.VSync = (VSyncMode) Enum.Parse(typeof (VSyncMode), e.InputArray[2], true);
										}
										catch (Exception exception)
										{
										}
										hudDebug.AddLine("VSync was set to " + RenderWindow.Instance.VSync);
									}
									break;
								default:
									hudDebug.AddLine("Usage: set [timeMult|walkSpeed|VSync] [value]", Color4.LightBlue);
									break;
							}
						}
						else
						{
							hudDebug.AddLine("Usage: set [timeMult|walkSpeed|VSync] [value]", Color4.LightBlue);
						}
						break;
					case "reset":
						if(e.InputArray.Length > 1 && !String.IsNullOrEmpty(e.InputArray[1]))
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
								case "VSync":
									RenderWindow.Instance.VSync = VSyncMode.On;
									hudDebug.AddLine("walkSpeed was reset to " + RenderWindow.Instance.VSync);
									break;
								default:
									hudDebug.AddLine("Usage: reset [timeMult|walkSpeed|VSync]", Color4.LightBlue);
									break;
							}
						}
						else
						{
							hudDebug.AddLine("Usage: reset [timeMult|walkSpeed|VSync]", Color4.LightBlue);
						}
						break;
					case "get":
						if(e.InputArray.Length > 1 && !String.IsNullOrEmpty(e.InputArray[1]))
						{
							switch(e.InputArray[1])
							{
								case "timeMult":
									hudDebug.AddLine("timeMult = " + RenderWindow.Instance.timeMultiplier);
									break;
								case "walkSpeed":
									hudDebug.AddLine("walkSpeed = " + walkSpeed);
									break;
								case "VSync":
									hudDebug.AddLine("VSync = " + RenderWindow.Instance.VSync);
									break;
								default:
									hudDebug.AddLine("Usage: get [timeMult|walkSpeed|VSync]", Color4.LightBlue);
									break;
							}
						}
						else
						{
							hudDebug.AddLine("Usage: get [timeMult|walkSpeed|VSync]", Color4.LightBlue);
						}
						break;
					case "clear":
						hudDebug.ClearScreen();
						break;
					case "reload":
						config.Reload();
						break;
					case "list":

						break;
					default:
						hudDebug.AddLine("Invalid command. Type 'list' for a list of commands", Color4.Red);
						break;

				}
			}
		}

		private void HandleKeyPress(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar == '`' || e.KeyChar == '~' || e.KeyChar == '	') //Tab to support non-European keyboards
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