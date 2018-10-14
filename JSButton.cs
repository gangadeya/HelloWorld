using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JSUI {
	
	[RequireComponent (typeof (Button))]
	public class JSButton : MonoBehaviour, IPointerEnterHandler {
		protected Button button;

		void Awake () {
			button = GetComponent<Button> ();
		}

		public void OnPointerEnter (PointerEventData eventData) {
			JSHelper.DebugLog ("Mouse Enter : " + name);
		}
	}

}
