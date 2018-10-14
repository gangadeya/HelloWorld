using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSMovePath : MonoBehaviour {

//	public Transform CapsuleCenterTrans;
//	public CapsuleCollider A;

//	public Transform Target;

	private List<Collider> colliders = new List<Collider> ();
	private Dictionary <Collider, JSColliderBase> colliderInfos = new Dictionary<Collider, JSColliderBase> ();

	private Quaternion VerticalQuaternionUp = Quaternion.AngleAxis (90.0f, Vector3.up);
	private Quaternion VerticalQuaternionDown = Quaternion.AngleAxis (90.0f, Vector3.down);
	private float Sqrt2 = Mathf.Sqrt (2.0f);

	public static JSMovePath Instance {
		get;
		private set;
	}

	// Use this for initialization
	void Start () {
		Instance = this;
		colliders.AddRange (GetComponentsInChildren <Collider> (true));
		for (int i = 0; i < colliders.Count; ++i) {
			if (!colliderInfos.ContainsKey (colliders [i])) {
				colliderInfos.Add (colliders [i], colliders [i].GetComponent<JSColliderBase> ());
			} else {
				JSHelper.DebugLogError ("Contains same collider key : " + colliders [i].name);
			}
		}
	}

	#region Auto Avoid Collider Path

	private List<Vector3> vectorInPath = new List<Vector3> ();

	public Vector3 [] GetVectorsInPath (JSColliderBase collider, Vector3 start, Vector3 end) {
		vectorInPath.Clear ();
		removeVectorIndexInPath.Clear ();
		vectorInPath.Add (start);
		SetVectorInPath (collider, start, end, vectorInPath);
		vectorInPath.Add (end);
		CheckSkipVector (collider);
		DrawAvoidLine ();
		return vectorInPath.ToArray ();
	}
		
	private void SetVectorInPath (JSColliderBase collider, Vector3 start, Vector3 end, List <Vector3> vectorInDirection) {
		JSColliderBase hitCollider = ColliderInPath (collider, start, end);
		if (hitCollider != null) {
			if (collider.SelfColliderType == ColliderType.Character &&
			    hitCollider.SelfColliderType == ColliderType.Obstacle) {
				vectorInDirection.Add (ClosestVectorBetweenCharacterAndObstacle ((JSCharacterCollider)collider, (JSObstacleCollider)hitCollider, start, ((JSCharacterCollider)collider).SelfCollider.radius, vectorInDirection));
				start = vectorInDirection [vectorInDirection.Count - 1];
				SetVectorInPath (collider, start, end, vectorInDirection);
			} else if (collider.SelfColliderType == ColliderType.Character && 
				hitCollider.SelfColliderType == ColliderType.Character) {
				vectorInDirection.Add (ClosestVectorBetweenCharacterAndCharacter ((JSCharacterCollider)collider, (JSCharacterCollider)hitCollider, start, end, vectorInDirection));
				start = vectorInDirection [vectorInDirection.Count - 1];
				SetVectorInPath (collider, start, end, vectorInDirection);
			} else {
				return;
			}
		}
	}

	private JSColliderBase ColliderInPath (JSColliderBase collider, Vector3 start, Vector3 end) {
		Collider hitCollider = null;
		float hitDistance = 0;
		Vector3 rayDirectionVector = Vector3.Normalize (end - start);
		float rayDirectionDistance = Vector3.Distance (start, end);

		foreach (RaycastHit hit in Physics.RaycastAll (start, rayDirectionVector, rayDirectionDistance)) {
			if (hit.collider != collider.selfCollider) {
				hitCollider = hit.collider;
				hitDistance = hit.distance;
				break;
			}
		}

		if (collider.SelfColliderType == ColliderType.Character) {
			Vector3 sideStart_1 = start + VerticalQuaternionUp * rayDirectionVector * ((JSCharacterCollider)collider).SelfCollider.radius;
			foreach (RaycastHit hit in Physics.RaycastAll (sideStart_1, rayDirectionVector, rayDirectionDistance)) {
				if (hit.collider != collider.selfCollider) {
					if (hitDistance == 0 || 
						hit.distance < hitDistance) {
						hitCollider = hit.collider;
						hitDistance = hit.distance;
					}
					break;
				}
			}

			Vector3 sideStart_2 = start + VerticalQuaternionDown * rayDirectionVector * ((JSCharacterCollider)collider).SelfCollider.radius;
			foreach (RaycastHit hit in Physics.RaycastAll (sideStart_2, rayDirectionVector, rayDirectionDistance)) {
				if (hit.collider != collider.selfCollider) {
					if (hitDistance == 0 ||
						hit.distance < hitDistance) {
						hitCollider = hit.collider;
						hitDistance = hit.distance;
					}
					break;
				}
			}
		}

		if (hitCollider != null) {
			return hitCollider.GetComponent<JSColliderBase> ();
		} else {
			return null;
		}
	}

	private bool CheckColliderVector (Vector3 center, Vector3 size) {
		Bounds checkBounds = new Bounds (center, size);
		foreach (Collider collider in colliders) {
			if (collider.bounds.Intersects (checkBounds)) {
				return false;
			}
		}

		return true;
	}

	private List<int> removeVectorIndexInPath = new List<int> ();

	private void CheckSkipVector (JSColliderBase collider) {
		for (int i = 0; i < vectorInPath.Count - 1; ++i) {
			if (removeVectorIndexInPath.Contains (i)) {
				continue;
			} else {
				if (collider.SelfColliderType == ColliderType.Character) {
					int[] vectors = RemoveSkipVector (i, vectorInPath.Count, ((JSCharacterCollider)collider).SelfCollider.radius);
					if (vectors != null) {
						for (int j = 0; j < vectors.Length; ++j) {
							if (!removeVectorIndexInPath.Contains (vectors [j])) {
								removeVectorIndexInPath.Add (vectors [j]);
							}
						}
					}
				}
			}
		}
		removeVectorIndexInPath.Sort ();
		for (int i = removeVectorIndexInPath.Count - 1; i >= 0; --i) {
			vectorInPath.RemoveAt (removeVectorIndexInPath [i]);
		}
	}

	private int[] RemoveSkipVector (int startIndex, int length, float radius) {
		List<int> skipVectorIndex = new List<int> ();
		for (int i = startIndex + 2; i < length; ++i) {
			if (!removeVectorIndexInPath.Contains (i)) {
				if (!CheckSkipVectorCollider (vectorInPath [startIndex], vectorInPath [i], radius)) {
					skipVectorIndex.Add (i - 1);
				}
			}
		}
		return skipVectorIndex.ToArray ();
	}

	private bool CheckSkipVectorCollider (Vector3 start, Vector3 end, float radius) {
		Vector3 rayDirectionVector = Vector3.Normalize (end - start);
		float rayDirectionDistance = Vector3.Distance (start, end);
		if (Physics.Raycast (start, rayDirectionVector, rayDirectionDistance)) {
			return true;
		}
		Vector3 start_1 = start + VerticalQuaternionUp * rayDirectionVector * radius;
		if (Physics.Raycast (start_1, rayDirectionVector, rayDirectionDistance)) {
			return true;
		}
		Vector3 start_2 = start + VerticalQuaternionDown * rayDirectionVector * radius;
		if (Physics.Raycast (start_2, rayDirectionVector, rayDirectionDistance)) {
			return true;
		}
		return false;
	}

	private Vector3 ClosestVectorBetweenCharacterAndObstacle (JSCharacterCollider character, JSObstacleCollider obstacle, Vector3 start, float radius, List<Vector3> vectorInDirection) {
		Vector3[] obstacleVertexs = new Vector3[4];
		Vector3 closestVector = Vector3.zero;
		float closestDistance = 0;
		float vectorDistance = 0;

		obstacleVertexs [0] = new Vector3 (obstacle.SelfCollider.size.x * 0.5f + radius, 
//			obstacle.ColliderCenterTransform.position.y - start.y, 
			0,
			obstacle.SelfCollider.size.z * 0.5f + radius);
		obstacleVertexs [1] = new Vector3 (obstacle.SelfCollider.size.x * 0.5f + radius, 
//			obstacle.ColliderCenterTransform.position.y - start.y, 
			0,
			-obstacle.SelfCollider.size.z * 0.5f - radius);
		obstacleVertexs [2] = new Vector3 (-obstacle.SelfCollider.size.x * 0.5f - radius, 
//			obstacle.ColliderCenterTransform.position.y - start.y, 
			0,
			obstacle.SelfCollider.size.z * 0.5f + radius);
		obstacleVertexs [3] = new Vector3 (-obstacle.SelfCollider.size.x * 0.5f - radius, 
//			obstacle.ColliderCenterTransform.position.y - start.y, 
			0,
			-obstacle.SelfCollider.size.z * 0.5f - radius);

		obstacleVertexs [0] = obstacle.ColliderCenterTransform.TransformPoint (obstacleVertexs [0]);
		obstacleVertexs [1] = obstacle.ColliderCenterTransform.TransformPoint (obstacleVertexs [1]);
		obstacleVertexs [2] = obstacle.ColliderCenterTransform.TransformPoint (obstacleVertexs [2]);
		obstacleVertexs [3] = obstacle.ColliderCenterTransform.TransformPoint (obstacleVertexs [3]);

		for (int i = 0; i < obstacleVertexs.Length; ++i) {
			if (!vectorInDirection.Contains (obstacleVertexs [i])) {
				closestVector = obstacleVertexs [i];
				break;
			}
		}
		closestDistance = Vector3.Distance (start, closestVector);
		vectorDistance = 0;
		for (int i = 0; i < obstacleVertexs.Length; ++i) {
			if (!vectorInDirection.Contains (obstacleVertexs [i]) && 
				CheckColliderVector (obstacleVertexs [i], character.SelfCollider.bounds.size)) {
				vectorDistance = Vector3.Distance (start, obstacleVertexs [i]);
				if (vectorDistance < closestDistance) {
					closestDistance = vectorDistance;
					closestVector = obstacleVertexs [i];
				}
			}
		}

		return closestVector;
	}

	private Vector3 ClosestVectorBetweenCharacterAndCharacter (JSCharacterCollider character, JSCharacterCollider obstacle, Vector3 start, Vector3 end, List<Vector3> vectorInDirection) {
		Vector3[] obstacleVertexs = new Vector3[2];
		Vector3 centerDiretcionVector = Vector3.Normalize (obstacle.ColliderCenterTransform.position - character.ColliderCenterTransform.position);
		Vector3 closestVector = Vector3.zero;
		float closestDistance = 0;
		float vectorDistance = 0;
		float characterRadius = Sqrt2 * character.SelfCollider.radius;
		float obstacleRadius = Sqrt2 * obstacle.SelfCollider.radius;

		obstacleVertexs [0] = obstacle.ColliderCenterTransform.position + VerticalQuaternionUp * centerDiretcionVector * (obstacleRadius + characterRadius);
		obstacleVertexs [1] = obstacle.ColliderCenterTransform.position + VerticalQuaternionDown * centerDiretcionVector * (obstacleRadius + characterRadius);

		for (int i = 0; i < obstacleVertexs.Length; ++i) {
			if (!vectorInDirection.Contains (obstacleVertexs [i]) && 
				CheckColliderVector (obstacleVertexs [i], character.SelfCollider.bounds.size)) {
				closestVector = obstacleVertexs [i];
				break;
			}
		}
		closestDistance = Vector3.Distance (end, closestVector);
		for (int i = 0; i < obstacleVertexs.Length; ++i) {
			if (!vectorInDirection.Contains (obstacleVertexs [i]) && 
				CheckColliderVector (obstacleVertexs [i], character.SelfCollider.bounds.size)) {
				vectorDistance = Vector3.Distance (end, obstacleVertexs [i]);
				if (vectorDistance < closestDistance) {
					closestDistance = vectorDistance;
					closestVector = obstacleVertexs [i];
				}
			}
		}

		return closestVector;
	}

	private void DrawAvoidLine () {
		for (int i = 0; i < vectorInPath.Count - 1; ++i) {
			JSHelper.DebugDrawLine (vectorInPath [i], vectorInPath [i + 1], Color.red, 5.0f);
		}
	}

	#endregion

	#region Auto Directlly Collider Path

	public Vector3[] GetDirectllyVectorInPath (JSColliderBase collider, Vector3 start, Vector3 end) {
		vectorInPath.Clear ();
		JSColliderBase hitCollider = ColliderInPath (collider, start, end);
		if (hitCollider != null) {
			if (hitCollider.selfCollider.bounds.Intersects (collider.selfCollider.bounds)) {
				return null;
			} else if (hitCollider.SelfColliderType == ColliderType.Obstacle) {
				end = GetObstacleDirectllyColliderVector ((JSObstacleCollider)hitCollider, start, end, ((JSCharacterCollider)collider).SelfCollider.radius);
			} else if (hitCollider.SelfColliderType == ColliderType.Character) {
				end = GetCharacterDirectllyColliderVector ((JSCharacterCollider)hitCollider,start, end, ((JSCharacterCollider)collider).SelfCollider.radius);
			}
		}
		vectorInPath.Add (start);
		vectorInPath.Add (end);
		DrawDirectllyLine (start, end);
		return vectorInPath.ToArray ();
	}

	private List<Vector3> crossPoint = new List<Vector3> ();
	private Vector3 GetObstacleDirectllyColliderVector (JSObstacleCollider obstacle, Vector3 start, Vector3 end, float radius) {
		crossPoint.Clear ();
		Vector3[] obstacleVertexs = new Vector3[4];

		obstacleVertexs [0] = new Vector3 (obstacle.SelfCollider.size.x * 0.5f + radius, 
			0, 
			obstacle.SelfCollider.size.z * 0.5f + radius);
		obstacleVertexs [1] = new Vector3 (obstacle.SelfCollider.size.x * 0.5f + radius, 
			0, 
			-obstacle.SelfCollider.size.z * 0.5f - radius);
		obstacleVertexs [2] = new Vector3 (-obstacle.SelfCollider.size.x * 0.5f - radius, 
			0, 
			-obstacle.SelfCollider.size.z * 0.5f - radius);
		obstacleVertexs [3] = new Vector3 (-obstacle.SelfCollider.size.x * 0.5f - radius, 
			0, 
			obstacle.SelfCollider.size.z * 0.5f + radius);

		obstacleVertexs [0] = obstacle.ColliderCenterTransform.TransformPoint (obstacleVertexs [0]);
		obstacleVertexs [1] = obstacle.ColliderCenterTransform.TransformPoint (obstacleVertexs [1]);
		obstacleVertexs [2] = obstacle.ColliderCenterTransform.TransformPoint (obstacleVertexs [2]);
		obstacleVertexs [3] = obstacle.ColliderCenterTransform.TransformPoint (obstacleVertexs [3]);

		for (int i = 0; i < obstacleVertexs.Length; ++i) {
			JSHelper.DebugDrawLine (obstacleVertexs [i % 4], obstacleVertexs [(i + 1) % 4], Color.yellow, 5.0f);
			CrossPoint (start, end, obstacleVertexs [i % 4], obstacleVertexs [(i + 1) % 4], start.y);
		}

		if (crossPoint.Count > 0) {
			Vector3 closestPoint = crossPoint [0];
			if (crossPoint.Count > 1) {
				float closestDistance = Vector3.Distance (start, crossPoint [0]);
				for (int i = 1; i < crossPoint.Count; ++i) {
					if (Vector3.Distance (start, crossPoint [i]) < closestDistance) {
						closestDistance = Vector3.Distance (start, crossPoint [i]);
						closestPoint = crossPoint [i];
					}
				}
			}
			return closestPoint;
		}

		return end;
	}

	private float EPS = Mathf.Pow (0.1f, 6.0f);

	private int DblCmp (float d) {
		if (Mathf.Abs (d) < EPS) {
			return 0;
		}
		return (d > 0) ? 1 : -1;
	}
	 
	private float Det (float x1, float z1, float x2, float z2)
	{
		return x1 * z2 - x2 * z1;
	}
	 
	private float Cross (Vector3 a, Vector3 b, Vector3 c)
	{
		return Det (b.x - a.x, b.z - a.z, c.x - a.x, c.z - a.z);
	}
	 
	private float DotDet (float x1, float z1, float x2, float z2)
	{
		return x1 * x2 + z1 * z2;
	}
	 
	private float Dot (Vector3 a, Vector3 b, Vector3 c)
	{
		return DotDet (b.x - a.x, b.z - a.z, c.x - a.x, c.z - a.z);
	}

	private int BetweenCmp (Vector3 a, Vector3 b, Vector3 c) {
		return DblCmp (Dot (a, b, c));
	}

	private void CrossPoint (Vector3 a, Vector3 b, Vector3 c, Vector3 d, float y)
	{
		float s1, s2;
		int d1, d2, d3, d4;
		 
		d1 = DblCmp (s1 = Cross (a, b, c));
		d2 = DblCmp (s2 = Cross (a, b, d));
		d3 = DblCmp (Cross (c, d, a));
		d4 = DblCmp (Cross (c, d, b));

		if ((d1^d2) == -2 && 
			(d3^d4) == -2) {
			crossPoint.Add (new Vector3 ((c.x * s2 - d.x * s1) / (s2 - s1), y, (c.z * s2 - d.z * s1) / (s2 - s1)));
		}
	}

	private Vector3 GetCharacterDirectllyColliderVector (JSCharacterCollider collider, Vector3 start, Vector3 end, float radius) {
		crossPoint.Clear ();
		LineCircleCrossPoint (start, end, collider.ColliderCenterTransform.position, collider.SelfCollider.radius + radius);
		if (crossPoint.Count > 0) {
			return crossPoint [0] + Vector3.Normalize (start - end) * Mathf.Sqrt (Mathf.Pow (collider.SelfCollider.radius + radius, 2.0f) - Mathf.Pow (Vector3.Distance (collider.colliderCenterTransform.position, crossPoint [0]), 2.0f));
		}
		return end;
	}

	private void LineCircleCrossPoint (Vector3 a, Vector3 b, Vector3 c, float r) {
		float s1, s2;
		int d1, d2, d3, d4;
		Vector3 d = c + VerticalQuaternionUp * Vector3.Normalize (b - a) * r;

		d1 = DblCmp (s1 = Cross (a, b, c));
		d2 = DblCmp (s2 = Cross (a, b, d));
		d3 = DblCmp (Cross (c, d, a));
		d4 = DblCmp (Cross (c, d, b));

		if ((d1^d2) == -2 && 
			(d3^d4) == -2) {
			crossPoint.Add (new Vector3 ((c.x * s2 - d.x * s1) / (s2 - s1), c.y, (c.z * s2 - d.z * s1) / (s2 - s1)));
		}

		d = c + VerticalQuaternionDown * Vector3.Normalize (b - a) * r;

		d1 = DblCmp (s1 = Cross (a, b, c));
		d2 = DblCmp (s2 = Cross (a, b, d));
		d3 = DblCmp (Cross (c, d, a));
		d4 = DblCmp (Cross (c, d, b));

		if ((d1^d2) == -2 && 
			(d3^d4) == -2) {
			crossPoint.Add (new Vector3 ((c.x * s2 - d.x * s1) / (s2 - s1), c.y, (c.z * s2 - d.z * s1) / (s2 - s1)));
		}
	}

	private void DrawDirectllyLine (Vector3 start, Vector3 end) {
		JSHelper.DebugDrawLine (start, end, Color.blue, 5.0f);
	}

	#endregion
	
	// Update is called once per frame
//	void Update () {
//		if (Input.GetKeyDown (KeyCode.V)) {
//			Vector3[] v = GetVectorsInPath (A.GetComponent<JSColliderBase> (), A.GetComponent<JSColliderBase> ().ColliderCenterTransform.position, Target.position);
//			for (int i = 0; i < v.Length - 1; ++i) {
//				JSHelper.DebugDrawLine (v [i], v [i + 1], Color.red, 5.0f);
//			}
//		} else if (Input.GetKeyDown (KeyCode.N)) {
//			JSHelper.DebugDrawLine (A.GetComponent<JSColliderBase> ().ColliderCenterTransform.position, GetDirectllyVectorInPath (A.GetComponent<JSColliderBase> (), A.GetComponent<JSColliderBase> ().ColliderCenterTransform.position, Target.position), Color.blue, 5.0f);
//		}
//	}
}
