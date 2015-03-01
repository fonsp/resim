using System;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using OpenTK;

namespace GraphicsLibrary.Core
{
	public class Node
	{
		/// <summary>
		/// Position relative to parent.
		/// </summary>
		public Vector3 position = Vector3.Zero;
		/// <summary>
		/// Position relative to the world.
		/// </summary>
		public Vector3 derivedPosition = Vector3.Zero;
		public Vector3 prevRelativeToCam = Vector3.Zero;

		/// <summary>
		/// Velocity relative to parent.
		/// </summary>
		public Vector3 velocity = Vector3.Zero;
		/// <summary>
		/// Velocity relative to world.
		/// </summary>
		public Vector3 derivedVelocity = Vector3.Zero;

		public Vector3 acceleration = Vector3.Zero;
		/// <summary>
		/// Friction, use (1,1,1) to disable.
		/// </summary>
		public Vector3 friction = new Vector3(1, 1, 1);

		/// <summary>
		/// Scale relative to parent.
		/// </summary>
		public Vector3 scale = new Vector3(1, 1, 1);
		/// <summary>
		/// Scale relative to the world.
		/// </summary>
		public Vector3 derivedScale = new Vector3(1, 1, 1);

		/// <summary>
		/// Orientation relative to parent.
		/// </summary>
		public Quaternion orientation = Quaternion.Identity;
		/// <summary>
		/// Orientation relative to the world.
		/// </summary>
		public Quaternion derivedOrientation = Quaternion.Identity;

		/// <summary>
		/// The parent node.
		/// </summary>
		public Node parent;
		/// <summary>
		/// Node name (must be unique).
		/// </summary>
		public string name;


		public bool debugRendering = false;
		/// <summary>
		/// Rendering pass, starting at 0.
		/// </summary>
		public int renderPass = 0;

		/// <summary>
		/// All children attached to this node.
		/// </summary>
		public Dictionary<string, Node> children = new Dictionary<string, Node>();

		public Node(string name)
		{
			this.name = name;
		}

		private List<QueueItem> eventQueue = new List<QueueItem>();

		/// <summary>
		/// Add a method to be queued. The method will be called when the event (traveling at c) has reached the camera.
		/// </summary>
		/// <param name="method">The method to be called</param>
		public void QueueMethod(QueueEvent method)
		{
			eventQueue.Add(new QueueItem(method));
		}

		/// <summary>
		/// Updates position, velocity, orientation and queued methods of this node, and all its children. Override Update to add custom update functionality.
		/// </summary>
		/// <param name="timeSinceLastUpdate">Time since last update, in seconds</param>
		public virtual void UpdateNode(float timeSinceLastUpdate)
		{
			velocity += Vector3.Multiply(acceleration, timeSinceLastUpdate);
			velocity = Vector3.Multiply(velocity, new Vector3((float)Math.Pow(friction.X, timeSinceLastUpdate / RenderWindow.Instance.lf), (float)Math.Pow(friction.Y, timeSinceLastUpdate / RenderWindow.Instance.lf), (float)Math.Pow(friction.Z, timeSinceLastUpdate / RenderWindow.Instance.lf)));
			position += Vector3.Multiply(velocity, timeSinceLastUpdate);

			if(parent == null)
			{
				derivedOrientation = orientation;
				derivedPosition = position;
				derivedScale = scale;
				derivedVelocity = velocity;
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
				derivedVelocity = parent.derivedVelocity + velocity;

			}
			foreach(Node n in children.Values)
			{
				n.UpdateNode(timeSinceLastUpdate);
			}
			
			Vector3 relativeToCam = derivedPosition - Camera.Instance.position;

			

			for (int i = 0; i < eventQueue.Count; i++)
			{
				if(eventQueue[i].Update(timeSinceLastUpdate / RenderWindow.Instance.lf, relativeToCam.Length))
				{
					eventQueue[i].method(this);
					eventQueue.RemoveAt(i);
					i--;
				}
			}

			// Relativity of time and space:
			// TODO: mult by tau?

			float tau = (prevRelativeToCam.Length - relativeToCam.Length) / RenderWindow.Instance.c;
			Update(timeSinceLastUpdate + tau);
			prevRelativeToCam = relativeToCam;
		}

		/// <summary>
		/// Override this method to add custom update functionality.
		/// </summary>
		/// <param name="timeSinceLastUpdate">Time since last update, in seconds</param>
		public virtual void Update(float timeSinceLastUpdate)
		{

		}

		/// <summary>
		/// Add specified node as child, and rename it.
		/// </summary>
		/// <param name="node">The node to be added</param>
		/// <param name="newName">The new node name</param>
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

		/// <summary>
		/// Add specified node as child.
		/// </summary>
		/// <param name="node">The node to be added</param>
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

		/// <summary>
		/// Checks if this node contains the specified node. 
		/// </summary>
		/// <param name="node">The node</param>
		/// <returns>True if this node contains the specified node</returns>
		public bool HasChild(Node node)
		{
			return this == node.parent; // TODO: performance check
			//return children.ContainsValue(node);
		}

		/// <summary>
		/// Checks if this node contains the specified node. 
		/// </summary>
		/// <param name="childName">The node's name</param>
		/// <returns>True if this node contains the specified node</returns>
		public bool HasChild(string childName)
		{
			return children.ContainsKey(childName);
		}

		/// <summary>
		/// Gets a child.
		/// </summary>
		/// <param name="childName">The child name</param>
		/// <returns>Child</returns>
		public Node GetChild(string childName)
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
				Node child = children[childName];
				children.Remove(childName);
				child = null;
			}
		}

		/// <summary>
		/// Delete and detach specified child.
		/// </summary>
		/// <param name="node">The child</param>
		public void RemoveChild(Node node)
		{
			if(children.ContainsValue(node))
			{
				node = null;
				children.Remove(node.name); //TODO: performance
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
		/// Move the node through space, without moving through time.
		/// </summary>
		/// <param name="newPos">New position</param>
		public void Teleport(Vector3 newPos)
		{
			prevRelativeToCam += newPos - position;
			position = newPos;
		}

		/// <summary>
		/// Renders the Node (if possible) and all its children. Recursive call.
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

	public delegate void QueueEvent(Node node);

	/// <summary>
	/// QueueItem holds the method to be called, along with the time since the call.
	/// </summary>
	public class QueueItem
	{
		public QueueEvent method;
		public float age = 0f;

		public QueueItem(QueueEvent method)
		{
			this.method = method;
		}

		public bool Update(float localTimeSinceLastUpdate, float currentDistance)
		{
			age += localTimeSinceLastUpdate;
			return age * RenderWindow.Instance.c >= currentDistance;
		}
	}
}