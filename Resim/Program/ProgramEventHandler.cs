using System;
using System.Globalization;
using System.Threading;
using GraphicsLibrary.Core;
using GraphicsLibrary.Input;
using OpenTK;
using GraphicsLibrary;
using GraphicsLibrary.Hud;
using OpenTK.Graphics;
using OpenTK.Input;

namespace Resim.Program
{
	public partial class Game
	{
		private void ConsoleInputReceived(object sender, HudConsoleInputEventArgs e)
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			if(e.InputArray.Length > 0 && !String.IsNullOrEmpty(e.InputArray[0]))
			{
				switch(e.InputArray[0].ToLower())
				{
					case "exit":
					case "stop":
					case "close":
						RenderWindow.Instance.Exit();
						break;
					case "set":
						if(e.InputArray.Length > 1 && !String.IsNullOrEmpty(e.InputArray[1]))
						{
							switch(e.InputArray[1].ToLower())
							{
								case "timemult":
									if(e.InputArray.Length > 2 && !String.IsNullOrEmpty(e.InputArray[2]))
									{
										RenderWindow.Instance.timeMultiplier = Convert.ToDouble(e.InputArray[2]);
										hudConsole.AddText("timeMult was set to " + RenderWindow.Instance.timeMultiplier);
									}
									break;
								case "c":
									if(e.InputArray.Length > 2 && !String.IsNullOrEmpty(e.InputArray[2]))
									{
										RenderWindow.Instance.c = (float)Convert.ToDouble(e.InputArray[2]);
										hudConsole.AddText("c was set to " + RenderWindow.Instance.c);
									}
									break;
								case "walkspeed":
									if(e.InputArray.Length > 2 && !String.IsNullOrEmpty(e.InputArray[2]))
									{
										walkSpeed = (int)Convert.ToDouble(e.InputArray[2]);
										hudConsole.AddText("walkSpeed was set to " + walkSpeed);
									}
									break;
								case "vsync":
									if(e.InputArray.Length > 2 && !String.IsNullOrEmpty(e.InputArray[2]))
									{
										try
										{
											RenderWindow.Instance.VSync = (VSyncMode)Enum.Parse(typeof(VSyncMode), e.InputArray[2], true);
										}
										catch(Exception exception)
										{
										}
										hudConsole.AddText("VSync was set to " + RenderWindow.Instance.VSync);
									}
									break;
								case "mouse":
									if(e.InputArray.Length > 2 && !String.IsNullOrEmpty(e.InputArray[2]))
									{
										mouseSensitivityFactor = (float)Convert.ToDouble(e.InputArray[2]);
										hudConsole.AddText("mouse sensitivity was set to " + mouseSensitivityFactor);
									}
									break;
								case "fov":
									if(e.InputArray.Length > 2 && !String.IsNullOrEmpty(e.InputArray[2]))
									{
										Camera.Instance.Fov = (float)Convert.ToDouble(e.InputArray[2]);
										hudConsole.AddText("fov was set to " + Camera.Instance.Fov);
									}
									break;
								case "skybox":
									if(e.InputArray.Length > 2 && !String.IsNullOrEmpty(e.InputArray[2]))
									{
										try
										{
											((Entity)RootNode.Instance.GetChild("skybox")).isVisible = bool.Parse(e.InputArray[2]);
										}
										catch(Exception exception)
										{
										}
										hudConsole.AddText("Skybox visibilty was set to " + ((Entity)RootNode.Instance.GetChild("skybox")).isVisible);
									}
									break;
								default:
									hudConsole.AddText("Usage: set [timeMult|walkSpeed|VSync|mouse|c|fov|skybox] [value]", Color4.LightBlue);
									break;
							}
						}
						else
						{
							hudConsole.AddText("Usage: set [timeMult|walkSpeed|VSync|mouse|c|fov|skybox] [value]", Color4.LightBlue);
						}
						break;
					case "reset":
						if(e.InputArray.Length > 1 && !String.IsNullOrEmpty(e.InputArray[1]))
						{
							switch(e.InputArray[1].ToLower())
							{
								case "timemult":
									RenderWindow.Instance.timeMultiplier = 1.0;
									hudConsole.AddText("timeMult was reset to " + RenderWindow.Instance.timeMultiplier);
									break;
								case "c":
									RenderWindow.Instance.c = 2000f;
									hudConsole.AddText("c was reset to " + RenderWindow.Instance.timeMultiplier);
									break;
								case "walkspeed":
									walkSpeed = 400;
									hudConsole.AddText("walkSpeed was reset to " + walkSpeed);
									break;
								case "vsync":
									RenderWindow.Instance.VSync = VSyncMode.On;
									hudConsole.AddText("walkSpeed was reset to " + RenderWindow.Instance.VSync);
									break;
								case "mouse":
									mouseSensitivityFactor = 1f;
									hudConsole.AddText("mouse sensitivity was reset to " + mouseSensitivityFactor);
									break;
								case "fov":
									Camera.Instance.Fov = 90f;
									hudConsole.AddText("fov was reset to " + Camera.Instance.Fov);
									break;
								case "skybox":
									((Entity)RootNode.Instance.GetChild("skybox")).isVisible = true;
									hudConsole.AddText("Skybox visibilty was reset to " + ((Entity)RootNode.Instance.GetChild("skybox")).isVisible);
									break;
								default:
									hudConsole.AddText("Usage: reset [timeMult|walkSpeed|VSync|mouse|c|fov|skybox]", Color4.LightBlue);
									break;
							}
						}
						else
						{
							hudConsole.AddText("Usage: reset [timeMult|walkSpeed|VSync|mouse|c|fov|skybox]", Color4.LightBlue);
						}
						break;
					case "get":
						if(e.InputArray.Length > 1 && !String.IsNullOrEmpty(e.InputArray[1]))
						{
							switch(e.InputArray[1].ToLower())
							{
								case "timemult":
									hudConsole.AddText("timeMult = " + RenderWindow.Instance.timeMultiplier);
									break;
								case "c":
									hudConsole.AddText("c = " + RenderWindow.Instance.c);
									break;
								case "walkspeed":
									hudConsole.AddText("walkSpeed = " + walkSpeed);
									break;
								case "vsync":
									hudConsole.AddText("VSync = " + RenderWindow.Instance.VSync);
									break;
								case "mouse":
									hudConsole.AddText("mouse = " + mouseSensitivityFactor);
									break;
								case "fov":
									hudConsole.AddText("fov = " + Camera.Instance.Fov);
									break;
								case "skybox":
									hudConsole.AddText("skybox = " + ((Entity)RootNode.Instance.GetChild("skybox")).isVisible);
									break;
								default:
									hudConsole.AddText("Usage: get [timeMult|walkSpeed|VSync|mouse|c|fov|skybox]", Color4.LightBlue);
									break;
							}
						}
						else
						{
							hudConsole.AddText("Usage: get [timeMult|walkSpeed|VSync|mouse|c|fov|skybox]", Color4.LightBlue);
						}
						break;
					case "tp":
						if(e.InputArray.Length > 3 && !String.IsNullOrEmpty(e.InputArray[1]) && !String.IsNullOrEmpty(e.InputArray[2]) &&
							!String.IsNullOrEmpty(e.InputArray[3]))
						{
							if(e.InputArray[1][0] == '#')
							{
								if(e.InputArray[1].Length > 1)
								{
									Camera.Instance.position.X +=
										(float)Convert.ToDouble(e.InputArray[1].Substring(1, e.InputArray[1].Length - 1));
								}
							}
							else
							{
								Camera.Instance.position.X = (float)Convert.ToDouble(e.InputArray[1]);
							}
							if(e.InputArray[2][0] == '#')
							{
								if(e.InputArray[2].Length > 1)
								{
									Camera.Instance.position.Y +=
										(float)Convert.ToDouble(e.InputArray[2].Substring(1, e.InputArray[2].Length - 1));
								}
							}
							else
							{
								Camera.Instance.position.Y = (float)Convert.ToDouble(e.InputArray[2]);
							}
							if(e.InputArray[3][0] == '#')
							{
								if(e.InputArray[3].Length > 1)
								{
									Camera.Instance.position.Z +=
										(float)Convert.ToDouble(e.InputArray[3].Substring(1, e.InputArray[3].Length - 1));
								}
							}
							else
							{
								Camera.Instance.position.Z = (float)Convert.ToDouble(e.InputArray[3]);
							}
						}
						else
						{
							hudConsole.AddText("Usage: tp [x] [y] [z]", Color4.LightBlue);
						}
						break;
					case "clear":
						hudConsole.ClearScreen();
						break;
					case "reload":
						config.Reload();
						InputManager.ClearToggleStates();
						fpsCam = Vector2.Zero;
						walkSpeed = 400;
						RenderWindow.Instance.worldTime = 0f;
						RenderWindow.Instance.localTime = 0f;
						RenderWindow.Instance.c = 2000f;
						RenderWindow.Instance.v = 0f;
						RenderWindow.Instance.b = 0f;
						RenderWindow.Instance.lf = 1f;
						RenderWindow.Instance.enableDoppler = true;
						RenderWindow.Instance.enableRelBrightness = true;
						RenderWindow.Instance.enableRelAberration = true;
						RenderWindow.Instance.smoothedVelocity = Vector3.Zero;
						RenderWindow.Instance.smoothFactor = 4000f;
						RenderWindow.Instance.timeMultiplier = 1.0;
						Camera.Instance.position = new Vector3(2700, 250, -6075);
						for(float x = 2000; x <= 4000; x += 500)
						{
							for(float z = -7000; z <= -5000; z += 500)
							{
								BasicClock clock = (BasicClock)clocks.GetChild("Clock" + x.ToString("F0") + "_" + z.ToString("F0"));
								clock.position.X = x;
								clock.position.Z = z;
								clock.position.Y = 100;
							}
						}
						map2a.mesh.material.baseColor = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
						map2b.mesh.material.baseColor = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
						map2c.mesh.material.baseColor = new Color4(1.0f, 0.9f, 0.6f, 1.0f);
						map2d.mesh.material.baseColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
						map2e.mesh.material.baseColor = new Color4(0.3f, 0.3f, 0.3f, 1.0f);
						map2f.mesh.material.baseColor = new Color4(0.5f, 0.9f, 0.1f, 1.0f);
						break;
					case "parteyy":
						map2a.mesh.material.baseColor = new Color4(0.4f, 1.0f, 0.4f, 1.0f);
						map2b.mesh.material.baseColor = new Color4(1.0f, 0.1f, 0.1f, 1.0f);
						map2c.mesh.material.baseColor = new Color4(0.1f, 0.4f, 0.9f, 1.0f);
						map2d.mesh.material.baseColor = new Color4(0.1f, 0.1f, 1.0f, 1.0f);
						map2e.mesh.material.baseColor = new Color4(1.0f, 0.3f, 1.0f, 1.0f);
						map2f.mesh.material.baseColor = new Color4(1.0f, 0.2f, 0.1f, 1.0f);
						break;
					case "list":
					case "help":
						hudConsole.AddText("Available commands: stop set get reset reload list clear", Color4.LightBlue);
						break;
					default:
						hudConsole.AddText("Invalid command. Type 'list' for a list of commands", Color4.Red);
						break;

				}
			}
		}

		private void HandleKeyPress(object sender, KeyPressEventArgs e)
		{
			if(e.KeyChar == '`' || e.KeyChar == '~' || e.KeyChar == '	' || e.KeyChar == '/') //Tab and '/' to support non-European keyboards
			{
				if(hudConsole.enabled)
				{
					hudConsole.enabled = false;
					hudConsole.isVisible = false;
				}
				else
				{
					hudConsole.enabled = true;
					hudConsole.isVisible = true;
					if(hudConsole.input.Length > 0)
					{
						hudConsole.input = hudConsole.input.Remove(hudConsole.input.Length - 1);
					}
				}
			}
		}
	}
}