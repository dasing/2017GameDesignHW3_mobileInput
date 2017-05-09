using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobileInput : MonoBehaviour {

	public GameObject switcher;
	public float zoomSpeed = 0.5f;
	public float rotateSpeed = 100000.0f;


	private Animator animationController;
	private float accelerometerUpdateInterval = 1.0f / 60.0f;
	private float lowPassKernelWidthInSeconds = 1.0f;
	private float shakeDetectionThreshold = 3.0f;
	private float unShakeDetectionThreshold = 1.0f;

	private float lowPassFilterFactor; 
	private Vector3 lowPassValue;
	private float doubleTapTimer = 0f;
	private Vector2 touchStartPos;
	private TouchPhase preAction;
	private float swipeSpeedThreshold = 1f;


	void Start () {
		//Debug.Log ("delta time = " + Time.deltaTime);
		animationController = this.GetComponent<Animator> ();

		shakeDetectionThreshold *= shakeDetectionThreshold;
		swipeSpeedThreshold *= swipeSpeedThreshold;
		lowPassValue = Input.acceleration;
		lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
	}
		
	void Update () {

		Vector3 acceleration = Input.acceleration;
		lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
		Vector3 deltaAcceleration = acceleration - lowPassValue;

		//Debug.Log ("acceleration = " + acceleration);

		if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold) {
			//Shake
			animationController.SetTrigger ("walkTrigger");
		} 


		if (doubleTapTimer > 0 ) {
			
			doubleTapTimer += Time.deltaTime;

		}
		if ( doubleTapTimer > 0.3f ) {
			
			//Single tap
			//Debug.Log("single tap, damage");
			animationController.SetTrigger("damageTrigger");
			doubleTapTimer = 0f;
		}
			
		if (Input.touchCount == 1) {
			//Debug.Log ("tap count = "+ Input.GetTouch (i).tapCount );
			if (Input.GetTouch (0).phase == TouchPhase.Began) {
				
				if (Input.GetTouch (0).tapCount > 0) {
					doubleTapTimer += Time.deltaTime;
				}

				if (Input.GetTouch (0).tapCount == 2) {
					//Double tap
					//Debug.Log("double tap, dead");
					animationController.SetTrigger ("deadTrigger");
					doubleTapTimer = 0f;
				}

				touchStartPos = Input.GetTouch (0).position;
				preAction = TouchPhase.Began;
					
			}

			if (Input.GetTouch (0).phase == TouchPhase.Moved) {
				//Debug.Log ("move");
				doubleTapTimer = 0;

				Vector2 speed = Input.GetTouch (0).deltaPosition * Time.deltaTime;
				if (speed.sqrMagnitude < swipeSpeedThreshold) {
					//slide
					//Debug.Log("slide");

					//this.transform.RotateAround( this.transform.position, Vector3.up,  Input.GetTouch(0).deltaPosition.magnitude*rotateSpeed*Time.deltaTime );
					if( Input.GetTouch(0).deltaPosition.x > 0 )
						this.transform.Rotate ( Vector3.up * rotateSpeed * Input.GetTouch(0).deltaPosition.magnitude , Space.World );
					else
						this.transform.Rotate ( (-1)*Vector3.up * rotateSpeed * Input.GetTouch(0).deltaPosition.magnitude , Space.World );
					
				}

				preAction = TouchPhase.Moved;

			}

			if (Input.GetTouch (0).phase == TouchPhase.Ended) {
				//Debug.Log ("end");
				Vector2 speed = Input.GetTouch (0).deltaPosition * Time.deltaTime;
				if (preAction == TouchPhase.Moved && speed.sqrMagnitude >= swipeSpeedThreshold) {
					
					Vector2 currPos = Input.GetTouch (0).position;
					Vector2 direction = currPos - touchStartPos;

					if (direction.x > 0) {
						//Debug.Log ("move right");
						switcher.SendMessage ("swipeRight");

					} else {
						//Debug.Log ("move left");
						switcher.SendMessage ("swipeLeft");

					}
				} 

					
				preAction = TouchPhase.Ended;
			}

		} else if (Input.touchCount == 2) {
			
			Touch touchZero = Input.GetTouch (0);
			Touch touchOne = Input.GetTouch (1);

			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			Camera.main.fieldOfView += deltaMagnitudeDiff * zoomSpeed;
			Camera.main.fieldOfView = Mathf.Clamp (Camera.main.fieldOfView, 0.1f, 179.9f );
			
		}
	}
}
