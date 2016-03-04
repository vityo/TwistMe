using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraTransform : MonoBehaviour {
	//The target of the camera. The camera will always point to this object.
	public Vector3 _targetPosition = Vector3.zero;
	private Vector2 _positionLast = Vector2.zero;

	public float _distance = 10.0f;
	private Vector3 _distanceVector;

	public float _zoomStep = 1.0f;
	public float _zoomStepIOS = 0.3f;

	public float _zoomInMax = 3.0f;
	public float _zoomOutMax = 20.0f;

	private float _x = 0.0f;
	private float _y = 0.0f;

	public float _xSpeed = 2.0f;
	public float _ySpeed = 2.0f;

	public float _xSpeedIOS = 0.2f;
	public float _ySpeedIOS = 0.2f;

	void Start () {
		_distanceVector = new Vector3 (0.0f, 0.0f, -_distance);
		Vector2 angles = transform.localEulerAngles;
		_x = angles.x;
		_y = angles.y;
		cameraRotateXY(_x, _y);
	}

	void Update () {
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			if (Input.touchCount > 0) {
				switch (Input.GetTouch (0).phase) {
				case TouchPhase.Began:
					_positionLast = Input.GetTouch (0).position;
					break;
				}
			}
		}
	}

	void LateUpdate() {
		if (GetComponent<ObjectsTransform> ().touch == ObjectsTransform.TouchType.none) {
			cameraRotate ();
			cameraZoom ();
		}
	}

	void cameraRotate() {
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			if(Input.touchCount > 0) {
				switch(Input.GetTouch(0).phase) {
				case TouchPhase.Moved:
						Vector2 position = Input.GetTouch(0).position;
						_x += (position.x - _positionLast.x) * _xSpeedIOS;
						_y += -(position.y - _positionLast.y) * _ySpeedIOS;
						_positionLast = position;
						cameraRotateXY(_x, _y);
					break;
				}
			}
		}
		else if(Application.platform == RuntimePlatform.WindowsEditor) {
			if (Input.GetButton("Fire1")) {
				_x += Input.GetAxis("Mouse X") * _xSpeed;
				_y += -Input.GetAxis("Mouse Y") * _ySpeed;
				cameraRotateXY(_x,_y);
			}
		}
	}
	
	void cameraRotateXY(float x, float y ) {
		Quaternion rotation = Quaternion.Euler(y, x, 0.0f);
		Vector3 position = rotation * _distanceVector + _targetPosition;
		transform.rotation = rotation;
		transform.position = position;
	}

	void cameraZoom() {
		float scrollDelta = 0.0f;
		
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			if (Input.touchCount == 2) {
				Touch touchZero = Input.GetTouch(0);
				Touch touchOne = Input.GetTouch(1);
				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
				scrollDelta = touchDeltaMag - prevTouchDeltaMag;
			}
		} else if (Application.platform == RuntimePlatform.WindowsEditor) {
			scrollDelta = Input.GetAxis("Mouse ScrollWheel");
		}
		
		if ( scrollDelta < 0.0f ) {
			zoomOut();
		}
		else if ( scrollDelta > 0.0f ) {
			zoomIn();
		}
	}

	void zoomIn(){
		_distance -= _zoomStep;
		
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			_distance -= _zoomStep * _zoomStepIOS;
		} else if (Application.platform == RuntimePlatform.WindowsEditor) {
			_distance -= _zoomStep;
		}

		if (_distance < _zoomInMax) {
			_distance = _zoomInMax;
		}
		
		_distanceVector = new Vector3(0.0f,0.0f, -_distance);
		cameraRotateXY(_x,_y);
	}

	void zoomOut(){
		_distance += _zoomStep;
		
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			_distance += _zoomStep * _zoomStepIOS;
		} else if (Application.platform == RuntimePlatform.WindowsEditor) {
			_distance += _zoomStep;
		}
		
		if (_distance > _zoomOutMax) {
			_distance = _zoomOutMax;
		}
		
		_distanceVector = new Vector3(0.0f,0.0f,-_distance);
		cameraRotateXY(_x,_y);
	}
}
