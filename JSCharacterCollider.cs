using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(CapsuleCollider))]
public class JSCharacterCollider : JSColliderBase {
	public CapsuleCollider SelfCollider {
		get { 
			return (CapsuleCollider)selfCollider;
		}
	}

	protected override void JSStart ()
	{
		base.JSStart ();

		colliderType = ColliderType.Character;
	}
}
