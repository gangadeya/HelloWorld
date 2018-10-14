using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColliderType
{
	Character,
	Obstacle,
	Default
}

[RequireComponent (typeof (Collider))]
public class JSColliderBase : MonoBehaviour {
	public ColliderType colliderType = ColliderType.Default;

	public Transform colliderCenterTransform;
	public Collider selfCollider;

	public ColliderType SelfColliderType {
		get { 
			return colliderType;
		}
	}

	public Transform ColliderCenterTransform {
		get { 
			return colliderCenterTransform;
		}
	}

	void Start () {
		JSStart ();
	}

	protected virtual void JSStart () {
		selfCollider = GetComponent<Collider> ();
	}
}
