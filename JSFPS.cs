using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSFPS : Singleton <JSFPS> {

	private float updateInterval = 0.5f;
	private float lastInterval = 0;
	private int frames = 0;
	private float fps = 0;

	private bool isStart = false;

	public void JSStart () {
		if (!isStart) {
			isStart = true;

			lastInterval = Time.realtimeSinceStartup;
			frames = 0;
		}
	}

	void OnGUI () {
		GUI.Label (new Rect (0, 50, 200, 200), "FPS: " + fps.ToString ("f2"));
	}

	void Update () {
		++frames;
		if (Time.realtimeSinceStartup > lastInterval + updateInterval) {
			fps = frames / (Time.realtimeSinceStartup - lastInterval);
			frames = 0;
			lastInterval = Time.realtimeSinceStartup;
		}
	}
}
