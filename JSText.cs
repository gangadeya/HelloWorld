using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JSUI {

	[RequireComponent (typeof(Text))]
	public class JSText : MonoBehaviour {
		protected Text text;
	
		void Awake () {
			text = GetComponent<Text> ();
		}
	
		public void SetText (string str) {
			if (text == null) {
				text = GetComponent<Text> ();
			}
			if (text.text != str) {
				text.text = str;
			}
		}

		public void SetText (int num) {
			if (text == null) {
				text = GetComponent<Text> ();
			}
			if (text.text != num.ToString ()) {
				text.text = num.ToString ();
			}
		}
	}

}
