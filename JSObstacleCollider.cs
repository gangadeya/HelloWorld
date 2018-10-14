using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider))]
public class JSObstacleCollider : JSColliderBase {

	public BoxCollider SelfCollider {
		get { 
			return (BoxCollider)selfCollider;
		}
	}

	protected override void JSStart ()
	{
		base.JSStart ();

		colliderType = ColliderType.Obstacle;
	}
}
