using System;
using System.Collections.Generic;
using OpenTK;

namespace GraphicsLibrary.Hud
{
	public class HudElement
	{
		/// <summary>
		/// Position relative to the parent.
		/// </summary>
		public Vector2 position = Vector2.Zero;
		/// <summary>
		/// Position relative to the world.
		/// </summary>
		public Vector2 derivedPosition = Vector2.Zero;

		/// <summary>
		/// Velocity relative to parent.
		/// </summary>
		public Vector2 velocity = Vector2.Zero;
		/// <summary>
		/// Velocity relative to the world.
		/// </summary>
		public Vector2 acceleration = Vector2.Zero;

		/// <summary>
		/// Scale relative to parent.
		/// </summary>
		public Vector2 scale = new Vector2(1, 1);
		/// <summary>
		/// Scale relative to the world.
		/// </summary>
		public Vector2 derivedScale = new Vector2(1, 1);

		/// <summary>
		/// Not implemented.
		/// </summary>
		public float rotation = 0f; //TODO: hudelement rotation
		/// <summary>
		/// Not implemented.
		/// </summary>
		public float derivedRotation = 0f;

		public bool isVisible = true;
		public bool debugRendering = false;

		public HudElement parent;
		public string name;

		public Dictionary<string, HudElement> children = new Dictionary<string, HudElement>();

		public HudElement(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// Updates position, velocity and orientation of this hudElement, and all its children. Override this method to add custom update functionality.
		/// </summary>
		/// <param name="timeSinceLastUpdate">Time since last update, in seconds</param>
		public virtual void Update(float timeSinceLastUpdate)
		{
			velocity += Vector2.Multiply(acceleration, timeSinceLastUpdate);
			position += Vector2.Multiply(velocity, timeSinceLastUpdate);

			if(parent == null)
			{
				derivedRotation = rotation;
				derivedPosition = position;
				derivedScale = scale;
			}
			else
			{
				derivedRotation = parent.derivedRotation + rotation;
				derivedPosition = parent.derivedPosition + position;//TODO: *derivedorientation
				derivedScale = Vector2.Multiply(parent.derivedScale, scale);
			}
			foreach(HudElement n in children.Values)
			{
				n.Update(timeSinceLastUpdate);
			}
		}


		/// <summary>
		/// Add specified hudElement as child, and rename it.
		/// </summary>
		/// <param name="hudElement">The hudElement to be added</param>
		/// <param name="newName">The new hudElement name</param>
		public void Add(HudElement hudElement, string newName)
		{
			hudElement.name = newName;
			if(hudElement == this)
			{

			}
			else
			{
				hudElement.parent = this;
				children.Add(hudElement.name, hudElement);
			}
		}

		/// <summary>
		/// Add specified hudElement as child.
		/// </summary>
		/// <param name="hudElement">The hudElement to be added</param>
		public void Add(HudElement hudElement)
		{
			if(hudElement == this)
			{

			}
			else
			{
				hudElement.parent = this;
				children.Add(hudElement.name, hudElement);
			}
		}

		/// <summary>
		/// Checks if this hudElement contains the specified hudElement. 
		/// </summary>
		/// <param name="hudElement">The hudElement</param>
		/// <returns>True if this hudElement contains the specified hudElement</returns>
		public bool HasChild(HudElement hudElement)
		{
			return this == hudElement.parent; // TODO: performance check
			//return children.ContainsValue(hudElement);
		}

		/// <summary>
		/// Checks if this hudElement contains the specified hudElement. 
		/// </summary>
		/// <param name="childName">The hudElement's name</param>
		/// <returns>True if this hudElement contains the specified hudElement</returns>
		public bool HasChild(string childName)
		{
			return children.ContainsKey(childName);
		}

		/// <summary>
		/// Gets a child.
		/// </summary>
		/// <param name="childName">The child name</param>
		/// <returns>Child</returns>
		public HudElement GetChild(string childName)
		{
			return children[childName];
		}

		/// <summary>
		/// Delete and detach specified child.
		/// </summary>
		/// <param name="childName">The child name</param>
		public void RemoveChild(string childName)
		{
			if(children.ContainsKey(childName))
			{
				HudElement child = children[childName];
				children.Remove(childName);
				child = null;
			}
		}

		/// <summary>
		/// Delete and detach specified child.
		/// </summary>
		/// <param name="hudElement">The child</param>
		public void RemoveChild(HudElement hudElement)
		{
			if(children.ContainsValue(hudElement))
			{
				hudElement = null;
				children.Remove(hudElement.name); //TODO: performance
			}
		}

		public bool Equals(HudElement obj)
		{
			return name == obj.name;
		}

		public void Rotate(float angle)
		{
			rotation += angle;
			throw new NotImplementedException();
		}

		public void ResetOrientation()
		{
			rotation = 0f;
			throw new NotImplementedException();
		}

		/// <summary>
		/// Renders the HudElement (if possible) and all its children. Recursive call.
		/// </summary>
		public void StartRender()
		{
			if(isVisible)
			{
				Render();
				foreach(HudElement h in children.Values)
				{
					h.StartRender();
				}
			}
		}

		public virtual void Render()
		{

		}
	}
}