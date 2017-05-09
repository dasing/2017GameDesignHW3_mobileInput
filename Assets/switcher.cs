using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switcher : MonoBehaviour {

	public GameObject knight;
	public GameObject monster;

	public void swipeRight(){
		knight.SetActive (false);
		monster.SetActive (true);
	}

	public void swipeLeft(){
		knight.SetActive (true);
		monster.SetActive (false);
		
	}
}
