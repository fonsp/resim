using System.Collections.Generic;
using OpenTK;

namespace GraphicsLibrary.Hud
{
	public class HudElement
	{
		public Vector2 position = Vector2.Zero;
		public Vector2 derivedPosition = Vector2.Zero;

		public Vector2 velocity = Vector2.Zero;
		public Vector2 acceleration = Vector2.Zero;

		public Vector2 scale = new Vector2(1, 1);
		public Vector2 derivedScale = new Vector2(1, 1);

		public float rotation = 0f;
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

		public void Update()
		{
			if(parent == null)
			{
				derivedRotation = rotation;
				derivedPosition = position;
				derivedScale = scale;
			}
			else
			{
				derivedRotation = parent.derivedRotation + rotation;
				derivedPosition = parent.derivedPosition + position;
				derivedScale = Vector2.Multiply(parent.derivedScale, scale);
			}
			foreach(HudElement n in children.Values)
			{
				n.Update();
			}
		}

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

		public bool HasChild(HudElement hudElement)
		{
			return this == hudElement.parent; // TODO: performance check
			//return children.ContainsValue(node);
		}

		public bool HasChild(string childName)
		{
			return children.ContainsKey(childName);
		}

		public HudElement GetChild(string childName)
		{
			return children[childName];
		}

		public void RemoveChild(string childName)
		{
			if(children.ContainsKey(childName))
			{
				children.Remove(childName);
			}
			else
			{
				//TODO: Child does not exits
			}
			//TODO: Cleanup
		}

		public void RemoveChild(HudElement hudElement)
		{
			if(children.ContainsValue(hudElement))
			{
				children.Remove(hudElement.name); //TODO: performance
			}
			else
			{
				//TODO: Child does not exist
			}
			//TODO: Cleanup
		}

		public void Rotate(float angle)
		{
			rotation += angle;
		}

		public void ResetOrientation()
		{
			rotation = 0f;
		}

		public void StartRender()
		{
			Render();
			foreach(HudElement h in children.Values)
			{
				h.StartRender();
			}
		}

		public virtual void Render()
		{

		}
	}
}