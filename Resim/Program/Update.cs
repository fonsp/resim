using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Input;
using GraphicsLibrary;
using GraphicsLibrary.Collision;
using GraphicsLibrary.Core;
using GraphicsLibrary.Input;

namespace Resim.Program
{
	public partial class Game
	{
		private Vector2 fpsCam = Vector2.Zero;
		private double bobbingTimer;
		private double bobbingFactor;
		private int walkSpeed = 400;
		private int playerHeight = 170;
		private Vector2 mouseSensitivity = new Vector2(200, 200); //TODO: config
		private Vector3 cameraBobDelta = Vector3.Zero;
		private bool mouseDown = false;
		private bool windowKeyDown = false;
		private bool freezeKeyDown = false;
		private double timeMultPreFreeze = 1.0;
		private int comboFieldCounter;
		private CollisionAABB monsterAABB = new CollisionAABB(new Vector3(-50, 0, -50), new Vector3(50, 110, 50));

		public override void Update(float timeSinceLastUpdate)
		{
			skybox.position = Camera.Instance.position;
			Camera.Instance.position -= cameraBobDelta;

			InputManager.UpdateToggleStates();

			if(InputManager.IsKeyDown(Key.R))
			{
				Respawn();
			}

			#region FPS camera

			if(!InputManager.IsKeyToggled(Key.Delete))
			{
				Vector2 mouseDelta = InputManager.GetMousePosition();
				mouseDelta.Y /= mouseSensitivity.Y;
				mouseDelta.X /= mouseSensitivity.X;
				fpsCam += mouseDelta;
				fpsCam.Y = (float)Math.Max(-1.57, Math.Min(1.57, fpsCam.Y));

				Camera.Instance.ResetOrientation();
				Camera.Instance.Pitch(-fpsCam.Y);
				Camera.Instance.Yaw(fpsCam.X);
				InputManager.HideCursor();
			}
			else
			{
				InputManager.ShowCursor();
			}

			/*if(InputManager.IsButtonDown(MouseButton.Right))
			{
				if(Camera.Instance.Fov != 50)
				{
					Camera.Instance.Fov = 50;
				}
				crossHair.isVisible = false;
			}
			else
			{
				if(Camera.Instance.Fov != 90)
				{
					Camera.Instance.Fov = 90;
				}
				crossHair.isVisible = true;
			}*/
			#endregion
			#region Movement

			walkSpeed = config.GetInt(InputManager.IsKeyDown(Key.ShiftLeft) ? "runningSpeed" : "walkingSpeed");

			bool w = InputManager.IsKeyDown(Key.W);
			bool a = InputManager.IsKeyDown(Key.A);
			bool s = InputManager.IsKeyDown(Key.S);
			bool d = InputManager.IsKeyDown(Key.D);
			Vector2 delta = new Vector2(0, 0);
			if(w)
			{
				delta.Y += 1;
			}
			if(a)
			{
				delta.X -= 1;
			}
			if(s)
			{
				delta.Y -= 1;
			}
			if(d)
			{
				delta.X += 1;
			}

			delta.NormalizeFast();

			// Y
			Camera.Instance.velocity.X += delta.Y * walkSpeed * timeSinceLastUpdate * (float)Math.Sin(fpsCam.X); //0
			Camera.Instance.velocity.Z -= delta.Y * walkSpeed * timeSinceLastUpdate * (float)Math.Cos(fpsCam.X); //1

			// X
			Camera.Instance.velocity.X += delta.X * walkSpeed * timeSinceLastUpdate * (float)Math.Cos(fpsCam.X); //1
			Camera.Instance.velocity.Z += delta.X * walkSpeed * timeSinceLastUpdate * (float)Math.Sin(fpsCam.X); //0

			#endregion
			#region Collision

			bool grounded = false;
			playerHeight = config.GetInt(InputManager.IsKeyDown(Key.LControl) ? "crouchHeight" : "walkHeight");
			Vector3 feetPos = Camera.Instance.position;
			feetPos.Y -= playerHeight * 0.75f;

			foreach(CollisionAABB collisionBox in mapCollision)
			{
				// Y
				CollisionRay collisionRay = new CollisionRay(Camera.Instance.position, new Vector3(.00001f, -1, .00001f));
				float dist = collisionBox.Intersect(collisionRay);
				if(dist <= playerHeight && dist != -1)
				{
					grounded = true;
					Camera.Instance.position.Y += Math.Min(config.GetInt("riseSpeed") * timeSinceLastUpdate, playerHeight - dist);
				}

				// X vel
				if(Camera.Instance.velocity.X > 0)
				{
					if(collisionBox.isInside(feetPos + new Vector3(Camera.Instance.velocity.X * timeSinceLastUpdate + 30, 0, 0)))
					{
						Camera.Instance.velocity.X = 0;
					}
				}
				else
				{
					if(collisionBox.isInside(feetPos + new Vector3(Camera.Instance.velocity.X * timeSinceLastUpdate - 30, 0, 0)))
					{
						Camera.Instance.velocity.X = 0;
					}
				}
				// Z
				if(Camera.Instance.velocity.Z > 0)
				{
					if(collisionBox.isInside(feetPos + new Vector3(0, 0, Camera.Instance.velocity.Z * timeSinceLastUpdate + 30)))
					{
						Camera.Instance.velocity.Z = 0;
					}
				}
				else
				{
					if(collisionBox.isInside(feetPos + new Vector3(0, 0, Camera.Instance.velocity.Z * timeSinceLastUpdate - 30)))
					{
						Camera.Instance.velocity.Z = 0;
					}
				}
				Vector3 testPoint;


				// X+
				testPoint = feetPos;
				testPoint.X += 15;
				if(collisionBox.isInside(testPoint))
				{
					collisionRay = new CollisionRay(feetPos, new Vector3(1f, 0.00001f, 0.00001f));
					dist = collisionBox.Intersect(collisionRay);
					if(dist != -1)
					{
						Camera.Instance.position.X -= dist;
					}
				}
				// X-
				testPoint = feetPos;
				testPoint.X -= 15;
				if(collisionBox.isInside(testPoint))
				{
					collisionRay = new CollisionRay(feetPos, new Vector3(-1f, 0.00001f, 0.00001f));
					dist = collisionBox.Intersect(collisionRay);
					if(dist != -1)
					{
						Camera.Instance.position.X += dist;
					}
				}
				// Z+
				testPoint = feetPos;
				testPoint.Z += 15;
				if(collisionBox.isInside(testPoint))
				{
					collisionRay = new CollisionRay(feetPos, new Vector3(0.00001f, 0.00001f, 1f));
					dist = collisionBox.Intersect(collisionRay);
					if(dist != -1)
					{
						Camera.Instance.position.Z -= dist;
					}
				}
				// Z-
				testPoint = feetPos;
				testPoint.Z -= 15;
				if(collisionBox.isInside(testPoint))
				{
					collisionRay = new CollisionRay(feetPos, new Vector3(0.00001f, 0.00001f, -1f));
					dist = collisionBox.Intersect(collisionRay);
					if(dist != -1)
					{
						Camera.Instance.position.Z += dist;
					}
				}
			}

			if(Camera.Instance.position.Y <= playerHeight)
			{
				grounded = true;
				Camera.Instance.position.Y += Math.Min(config.GetInt("riseSpeed") * timeSinceLastUpdate, playerHeight - Camera.Instance.position.Y);
			}
			#endregion
			#region Jumping/Gravity

			if(grounded)
			{
				Camera.Instance.velocity.Y = Math.Max(Camera.Instance.velocity.Y, 0);
				Camera.Instance.position.Y -= (float)config.GetDouble("groundedCorrection") * timeSinceLastUpdate; //roundoff error correction
				if(InputManager.IsKeyDown(Key.Space))
				{
					Camera.Instance.velocity.Y = config.GetInt("jumpForce");
				}
			}
			else
			{
				Camera.Instance.velocity.Y -= config.GetInt("gravity") * timeSinceLastUpdate;
			}

			#endregion
			#region Bobbing

			if(delta == Vector2.Zero || !grounded)
			{
				bobbingFactor = bobbingFactor * Math.Pow(0.1, timeSinceLastUpdate);
			}
			else
			{
				bobbingFactor = -((1 - bobbingFactor) * Math.Pow(0.1, timeSinceLastUpdate) - 1);
				bobbingTimer += timeSinceLastUpdate;
			}

			Vector2 bobbingStrength = new Vector2(10, 10); //TODO: config

			cameraBobDelta = new Vector3
			(
				(float)(Math.Cos(fpsCam.X) * bobbingFactor * Math.Sin(bobbingTimer * 6)) * bobbingStrength.X,
				(float)(bobbingFactor * Math.Cos(bobbingTimer * 12)) * bobbingStrength.Y,
				(float)(Math.Sin(fpsCam.X) * bobbingFactor * Math.Sin(bobbingTimer * 6)) * bobbingStrength.X
			);

			cameraBobDelta = Vector3.Zero;

			Camera.Instance.position += cameraBobDelta;

			#endregion
			#region Other
			Vector3 raydir = Vector3.Zero;
			raydir.Z = (float)-Math.Cos(fpsCam.X);
			raydir.X = (float)Math.Sin(fpsCam.X);
			raydir.Y = (float)Math.Tan(fpsCam.Y);
			raydir.Normalize();

			if(!InputManager.IsButtonDown(MouseButton.Right))
			{
				if(mouseDown)
				{
					Camera.Instance.position += 500f * raydir;
				}
				mouseDown = false;
				monster.isVisible = false;
			}
			else
			{
				mouseDown = true;
				monster.isVisible = true;
				monster.position = 500f * raydir + Camera.Instance.derivedPosition;
				monster.velocity = RenderWindow.Instance.smoothedVelocity;
				//monster.position = new Vector3(500, 0, 0);
			}

			collisionVisuals.isVisible = InputManager.IsKeyToggled(Key.Number1);
			map1.mesh.shader = InputManager.IsKeyToggled(Key.Number2) ? Shader.depthShaderCompiled : null;
			if(InputManager.IsKeyDown(Key.P))
			{
				if(!freezeKeyDown)
				{
					timeMultPreFreeze = RenderWindow.Instance.timeMultiplier;
					RenderWindow.Instance.timeMultiplier = 0.0;
				}
				freezeKeyDown = true;
			}
			else
			{
				if(freezeKeyDown)
				{
					RenderWindow.Instance.timeMultiplier = timeMultPreFreeze;

					foreach(BasicClock clock in clocks.children.Values)
					{
						clock.QueueMethod(new QueueEvent(BasicClock.Jump));
					}
				}
				freezeKeyDown = false;
			}
			if(InputManager.IsKeyDown(Key.Number0) || InputManager.IsKeyDown(Key.Number8) || InputManager.IsKeyDown(Key.Number9))
			{
				if(!windowKeyDown)
				{
					if(InputManager.IsKeyDown(Key.Number8))
					{
						RenderWindow.Instance.WindowBorder = WindowBorder.Resizable;
						RenderWindow.Instance.WindowState = WindowState.Normal;
					}
					else if(InputManager.IsKeyDown(Key.Number9))
					{
						RenderWindow.Instance.WindowBorder = WindowBorder.Hidden;
						RenderWindow.Instance.WindowState = WindowState.Maximized;
					}
					else if(InputManager.IsKeyDown(Key.Number0))
					{
						RenderWindow.Instance.WindowBorder = WindowBorder.Resizable;
						RenderWindow.Instance.WindowState = WindowState.Fullscreen;
					}
				}
				windowKeyDown = true;
			}
			else
			{
				windowKeyDown = false;
			}

			RenderWindow.Instance.enableRelAberration = !InputManager.IsKeyToggled(Key.Number5);
			RenderWindow.Instance.enableRelBrightness = !InputManager.IsKeyToggled(Key.Number6);
			RenderWindow.Instance.enableDoppler = !InputManager.IsKeyToggled(Key.Number7);

			#endregion
			#region HUD
			crossHair.position.X = 640 + (int)(200 * Math.Cos(RenderWindow.Instance.worldTime * 0.2 * 3.14159265358979));
			crossHair.position.Y = 360 + (int)(200 * Math.Sin(RenderWindow.Instance.worldTime * 0.2 * 3.14159265358979));
			crossHair1.position.X = 640 + (int)(200 * Math.Cos(RenderWindow.Instance.localTime * 0.2 * 3.14159265358979));
			crossHair1.position.Y = 360 + (int)(200 * Math.Sin(RenderWindow.Instance.localTime * 0.2 * 3.14159265358979));

			hudDebug.isVisible = 
#if DEBUG
				!
#endif
				InputManager.IsKeyToggled(Key.Minus);
			#endregion
		}
	}
}