using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JSParticleTextEffect : MonoBehaviour {

	private ParticleSystem[] particles;
	private Text text;
	private Outline outLine;

	private Color originColor = Color.clear;
	private Color startColor = Color.clear;
	private float colorFadeInTime = 1.0f;
	private float colorStayTime = 0.5f;
	private float colorFadeOutTime = 0.5f;
	private float currentTime = 0;
	private float colorFadeSpeed = 1.0f;

	// Use this for initialization
	void Start () {
		particles = GetComponentsInChildren<ParticleSystem> (true);
		text = GetComponentInChildren<Text> ();
		outLine = GetComponentInChildren<Outline> ();
		originColor = text.color;
		startColor = text.color;
		startColor.a = 0;

		enabled = false;
	}

	public enum EffectStep
	{
		FadeIn,
		Stay,
		FadeOut,
		Null
	}
	private EffectStep step = EffectStep.Null;

	public void StartEffect () {
		enabled = true;
		step = EffectStep.FadeIn;

		foreach (ParticleSystem par in particles) {
			par.Stop ();
			par.Play ();
		}

		text.color = startColor;
		currentTime = 0;
		colorFadeSpeed = 1.0f / colorFadeInTime;
	}

	private void UpdateEffect () {
//		if (step == EffectStep.FadeIn) {
//			currentTime += JSTime.Instance.UITime;
//			if (text.color.a < 1.0f) {
//				text.color.a = colorFadeSpeed * currentTime;
//				if (text.color.a > 1.0f) {
//					text.color.a = 0;
//				}
//			}
//			if (currentTime >= colorFadeInTime) {
//				currentTime = 0;
//				step = EffectStep.Stay;
//			}
//		} else if (step == EffectStep.Stay) {
//			currentTime += JSTime.Instance.UITime;
//			if (currentTime >= colorStayTime) {
//				currentTime = 0;
//				step = EffectStep.FadeOut;
//			}
//		} else if (step == EffectStep.FadeOut) {
//			currentTime += JSTime.Instance.UITime;
//			if (text.color.a )
//			if (currentTime >= colorFadeOutTime) {
//				currentTime = 0;
//				step = EffectStep.Null;
//				enabled = false;
//			}
//		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
