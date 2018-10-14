using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSController;

public class JSAnimationEvent : MonoBehaviour {
	private GameObject senderObj;
	private Animator animator;

	void Start () {
		animator = GetComponent<Animator> ();
	}

	public GameObject SenderObj {
		get { 
			return senderObj;
		}
		set { 
			senderObj = value;
		}
	}

	[System.Serializable]
	public class AnimationEventInfo {
		public string eventName;
		public GameObject obj;
		public bool selfParticle = false;
		public List<ParticleSystem> particles = new List<ParticleSystem> ();
		public bool selfTrail = false;
		public List<TrailRenderer> trails = new List<TrailRenderer> ();
		public string[] sounds;
	}

	public List<AnimationEventInfo> animationEventInfos = new List<AnimationEventInfo> ();

	/// <summary>
	/// Play Effect Animation Event
	/// </summary>
	/// <param name="index">Index.</param>
	public void AnimationEvent (string index_pause) {
		string[] vars = index_pause.Split ('_'); 
		int index = int.Parse (vars [0]);
		float pause = float.Parse (vars [1]);
		if (animationEventInfos.Count == 0) {
			JSHelper.DebugLog ("No animation event !");
		} else if (animationEventInfos.Count <= index) {
			JSHelper.DebugLogError ("Animation event index out of limit : " + index);
		} else {
			if (!animationEventInfos [index].selfParticle) {
				ParticleSystem.MainModule main = animationEventInfos [index].particles [0].main;
				main.simulationSpeed = JSTime.Instance.MainGameSceneDeltaTime;
				foreach (ParticleSystem par in animationEventInfos [index].particles) {
					par.Stop ();
					par.Play ();
				}

				if (pause > 0) {
					animator.speed = 0;
					StartCoroutine (DelayContinueAnimator (pause));
				}
			}
		}
	}

	private IEnumerator DelayContinueAnimator (float pauseTime) {
		yield return new WaitForSeconds (pauseTime);

		animator.speed = JSTime.Instance.MainGameSceneTimeSpeed;
	}

	/// <summary>
	/// React animation
	/// </summary>
	public delegate void ReactDelegateHandler (string key);
	public event ReactDelegateHandler actEventHandler;
	public string reactKey = null;

	/// <summary>
	/// React condition delegate handler.
	/// </summary>
	public delegate void ReactConditionDelegateHandler ();
	public event ReactConditionDelegateHandler actConditionEventHandler;
	public int conditionIndex = 0;

	/// <summary>
	/// Other Characters or judgement affect when this event happen
	/// </summary>
	public void ActEvent (int index = 1) {
		if (actEventHandler != null) {
			actEventHandler (reactKey);
			actEventHandler = null;
			reactKey = null;
		}

		if (index == conditionIndex) {
			if (actConditionEventHandler != null) {
				actConditionEventHandler ();
				actConditionEventHandler = null;
				conditionIndex = 0;
			}
		}
	}

	public void CheckActEvent () {
		if (actEventHandler != null) {
			JSHelper.DebugLogError ("actEventHandler not empty before init !");
		}
	}

	public delegate void FinishDelegateHandler ();
	public event FinishDelegateHandler finishEventHandler;
	/// <summary>
	/// Animation finish play event
	/// </summary>
	public void FinishEvent (string animationName) {
		JSHelper.DebugLog (name + " " + animationName + " finish play");
		if (finishEventHandler != null) {
			finishEventHandler ();
			finishEventHandler = null;
		}
	}
}
