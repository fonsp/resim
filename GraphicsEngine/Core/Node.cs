using System;
using System.Collections.Generic;
using OpenTK;

namespace GraphicsLibrary.Core
{
	public class Node
	{
		public Vector3 position = Vector3.Zero;
		public Vector3 derivedPosition = Vector3.Zero;

		public Vector3 velocity = Vector3.Zero;
		public Vector3 acceleration = Vector3.Zero;
		public Vector3 friction = new Vector3(1, 1, 1);

		public Vector3 scale = new Vector3(1, 1, 1);
		public Vector3 derivedScale = new Vector3(1, 1, 1);

		public Quaternion orientation = Quaternion.Identity;
		public Quaternion derivedOrientation = Quaternion.Identity;


		//TODO: direction and Euler angles
		/*public Vector3 rotation
		{
			get
			{
				throw new NotImplementedException();
				
				double sqw = orientation.W * orientation.W;
				double sqy = orientation.Y * orientation.Y;
				double sqz = orientation.Z * orientation.Z;

				return new Vector3(
					(float)Math.Asin(2f * (orientation.X * orientation.Z - orientation.W * orientation.Y)), //Pitch
					(float)Math.Atan2(2f * orientation.X * orientation.W + 2f * orientation.Y * orientation.Z, 1 - 2f * (sqz + sqw)), //Yaw
					(float)Math.Atan2(2f * orientation.X * orientation.Y + 2f * orientation.Z * orientation.W, 1 - 2f * (sqy + sqz)) //Roll
					);
			}
		}

		private Vector3 direction
		{
			get
			{
				throw new NotImplementedException();

				Vector3 rot = rotation;
				return new Vector3(
					(float)Math.Cos(rot.X), 
					(float)Math.Tan(rot.Y),
					(float)Math.Sin(rot.X)
					);
			}
		}*/

		public Node parent;
		public string name;

		public bool debugRendering = false;
		public int renderPass = 0;

		public Dictionary<string, Node> children = new Dictionary<string, Node>();

		public Node(string name)
		{
			this.name = name;
		}

		public virtual void Update(float timeSinceLastUpdate)
		{
			velocity += Vector3.Multiply(acceleration, timeSinceLastUpdate);
			velocity = Vector3.Multiply(velocity, new Vector3((float)Math.Pow(friction.X, timeSinceLastUpdate), (float)Math.Pow(friction.Y, timeSinceLastUpdate), (float)Math.Pow(friction.Z, timeSinceLastUpdate)));
			position += Vector3.Multiply(velocity, timeSinceLastUpdate);

			if(parent == null)
			{
				derivedOrientation = orientation;
				derivedPosition = position;
				derivedScale = scale;
			}
			else
			{
				if(parent == Camera.Instance)
				{
					derivedOrientation = Quaternion.Conjugate(parent.derivedOrientation) * orientation;
				}
				else
				{
					derivedOrientation = parent.derivedOrientation * orientation;
				}
				Vector3 t = 2 * Vector3.Cross(derivedOrientation.Xyz, position);
				derivedPosition = parent.derivedPosition + (position + derivedOrientation.W * t + Vector3.Cross(derivedOrientation.Xyz, t));
				derivedScale = Vector3.Multiply(parent.derivedScale, scale);

			}
			foreach(Node n in children.Values)
			{
				n.Update(timeSinceLastUpdate);
			}
		}

		public void Update()
		{
			if(parent == null)
			{
				derivedOrientation = orientation;
				derivedPosition = position;
				derivedScale = scale;
			}
			else
			{
				/*derivedOrientation = orientation * parent.derivedOrientation;
				derivedPosition = parent.derivedPosition + position;
				derivedScale = Vector3.Multiply(parent.derivedScale, scale);*/
				if(parent == Camera.Instance)
				{
					derivedOrientation = Quaternion.Conjugate(parent.derivedOrientation) * orientation;
				}
				else
				{
					derivedOrientation = parent.derivedOrientation * orientation;
				}
				Vector3 t = 2 * Vector3.Cross(derivedOrientation.Xyz, position);
				derivedPosition = parent.derivedPosition + (position + derivedOrientation.W * t + Vector3.Cross(derivedOrientation.Xyz, t));
				derivedScale = Vector3.Multiply(parent.derivedScale, scale);
			}
			foreach(Node n in children.Values)
			{
				n.Update();
			}
		}

		public void Add(Node node, string newName)
		{
			node.name = newName;
			if(node == this)
			{

			}
			else
			{
				node.parent = this;
				children.Add(node.name, node);
			}
		}

		public void Add(Node node)
		{
			if(node == this)
			{

			}
			else
			{
				node.parent = this;
				children.Add(node.name, node);
			}
		}

		public bool HasChild(Node node)
		{
			return this == node.parent; // TODO: performance check
			//return children.ContainsValue(node);
		}

		public bool HasChild(string childName)
		{
			return children.ContainsKey(childName);
		}

		public Node GetChild(string childName)
		{
			return children[childName];
		}

		public void RemoveChild(string childName)
		{
			if(children.ContainsKey(childName))
			{
				children[childName] = null;
				children.Remove(childName);
			}
			else
			{
				//TODO: Child does not exits
			}
		}

		public void RemoveChild(Node node)
		{
			if(children.ContainsValue(node))
			{
				node = null;
				children.Remove(node.name); //TODO: performance
			}
			else
			{
				//TODO: Child does not exist
			}
		}

		public bool Equals(Node obj)
		{
			return name == obj.name;
		}

		public void Rotate(Vector3 axis, float angle)
		{
			Quaternion q = Quaternion.FromAxisAngle(axis, angle);
			Rotate(q);
		}

		public void Rotate(Quaternion quaternion)
		{
			orientation.Normalize(); //Fix drift
			orientation = orientation * quaternion;
		}

		public void Yaw(float angle)
		{
			Rotate(Vector3.UnitY, angle);
		}

		public void Pitch(float angle)
		{
			Rotate(Vector3.UnitX, angle);
		}

		public void Roll(float angle)
		{
			Rotate(Vector3.UnitZ, angle);
		}

		public void ResetOrientation()
		{
			orientation = Quaternion.Identity;
		}

		/// <summary>
		/// Renders the Node (if possible) and all its children.
		/// </summary>
		public void StartRender(int pass)
		{
			Render(pass);
			foreach(Node n in children.Values)
			{
				n.StartRender(pass);
			}
		}

		public virtual void Render(int pass)
		{
		}
	}
}