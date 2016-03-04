using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectsTransform : MonoBehaviour
{
	private Vector2 _positionLast = Vector2.zero;
	public float rotateSpeedX = 2.0f;
	public float rotateSpeedY = 2.0f;
	public float rotateSpeedXios = 0.2f;
	public float rotateSpeedYios = 0.2f;
	public float moveSpeed = 0.5f;
	public float moveSpeedIOS = 0.05f;
	public float rotateOrMoveSensitive = 30.0f;
	public float rotateOrMoveSensitiveIOS = 30.0f;

	private Vector3 _positionDown = Vector3.zero;
	private bool _isTouchAnalyzed = false;
	private List<GameObject> _selectObjects = new List<GameObject> ();
	private GameObject _selectObject = null;
	private List<GameObject> _selectObjectsUp = new List<GameObject> ();
	private List<GameObject> _selectObjectsDown = new List<GameObject> ();
	
	public enum TouchType {none, rotate, move};
	public TouchType touch = TouchType.none;
	private float moveYMinCurrent = 0.0f;

	void Update(){
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			if (Input.touchCount > 0) {
				switch (Input.GetTouch (0).phase) {
				case TouchPhase.Began:
					checkTouch (Input.GetTouch(0).position);
					break;
				case TouchPhase.Moved:
					if(touch == TouchType.rotate) {
						touchAnalyze(Input.GetTouch(0).position, rotateOrMoveSensitiveIOS);
					}
					break;
				case TouchPhase.Ended:
					resetTouchSettings();
					break;
				}
			}
		} else if (Application.platform == RuntimePlatform.WindowsEditor) {
			//по дабл клику эти строки пропускает и сразу вращает камеру, это не ошибка!

			if (Input.GetMouseButtonDown (0)) {
				checkTouch (Input.mousePosition);
			}

			if (Input.GetMouseButton(0)) {
				if(touch == TouchType.rotate) {
					touchAnalyze(Input.mousePosition, rotateOrMoveSensitive);
				}
			}

			if (Input.GetMouseButtonUp (0)) {
				resetTouchSettings();
			}
		}
	}

	void touchAnalyze(Vector3 positionMove, float sensitive) {
		if(Vector3.Distance(_positionDown, positionMove) > sensitive
			&& !_isTouchAnalyzed) {
			_isTouchAnalyzed = true;

			if(Mathf.Abs(_positionDown.x - positionMove.x) < Mathf.Abs(_positionDown.y - positionMove.y)) {
				List<GameObject> ballsColumn = new List<GameObject>(GameObject.FindGameObjectsWithTag("Ball"));
				float localAngle = Utils.ToAngleTwist(_selectObject.transform.localEulerAngles.y);

				for(int i = 0; i < ballsColumn.Count; i++) {
					if(Mathf.Abs(localAngle - Utils.ToAngleTwist(ballsColumn[i].transform.localEulerAngles.y)) > Model.angleSideHalf) {
						ballsColumn.RemoveAt(i);
						i--;
					}
				}
				
				int freeSpace = Model.rowCount - ballsColumn.Count;
				
				if(Mathf.Abs(localAngle - Utils.ToAngleTwist(GameObject.Find("SectionHole").transform.localEulerAngles.y - 180.0f)) < Model.angleSideHalf) {
					freeSpace++;
					moveYMinCurrent = Model.rowHoleY;
				}
				else {
					moveYMinCurrent = Model.rowZeroY;
				}
				
				if (freeSpace > 0) {
					unholdObjects (_selectObjects);
					_selectObjects.Clear();
					
					touch = TouchType.move;
					
					_selectObjects.AddRange(ballsColumn);
					
					for(int i = 0; i < _selectObjects.Count; i++) {
						if(_selectObjects[i].transform.position.y - _selectObject.transform.position.y > Model.EPS) {
							_selectObjectsUp.Add(_selectObjects[i]);
						}
						else if(_selectObjects[i].transform.position.y - _selectObject.transform.position.y < -Model.EPS) {
							_selectObjectsDown.Add(_selectObjects[i]);
						}
					}
					
					_selectObjectsUp = _selectObjectsUp.OrderBy(p => p.transform.position.y).ToList();
					_selectObjectsDown = _selectObjectsDown.OrderBy(p => -p.transform.position.y).ToList();
					
					for(int i = 0; i < _selectObjects.Count; i++) {
						MagnetismRow magRow = _selectObjects[i].GetComponent<MagnetismRow>();
						
						if(magRow) {
							List<GameObject> objectsCollision = new List<GameObject>(_selectObjects);
							objectsCollision.Remove(_selectObjects[i]);
							magRow.objectsCollision = objectsCollision;
							
							if(_selectObjects[i] == _selectObject) {
								continue;
							}
							
							magRow.isAlwaysCalculate = true;
						}
					}
				}
			}
		}
	}

	void LateUpdate() {
		//только в LateUpdate, потому что сначала магнетизм, потом толчок-поворот
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			if(Input.touchCount > 0) {
				switch(Input.GetTouch(0).phase) {
				case TouchPhase.Moved:
					if(touch == TouchType.rotate) {
						rotateObjects(-(Input.GetTouch(0).position.x - _positionLast.x) * rotateSpeedXios);
					}
					else if (touch == TouchType.move) {
						moveObjects((Input.GetTouch(0).position.y - _positionLast.y) * moveSpeedIOS);
					}
					
					_positionLast = Input.GetTouch(0).position;
					break;
				}
			}
		}
		else if(Application.platform == RuntimePlatform.WindowsEditor){
			if ( Input.GetButton("Fire1") ) {
				if(touch == TouchType.rotate) {
					rotateObjects(-Input.GetAxis("Mouse X") * rotateSpeedX);
				}
				else if (touch == TouchType.move) {
					moveObjects(Input.GetAxis("Mouse Y") * moveSpeed);
				}
			}
		}
	}

	void checkTouch(Vector3 position){
		Ray ray = Camera.main.ScreenPointToRay(position);
		RaycastHit hit;

		if(Physics.Raycast (ray, out hit)) {
			bool isSection = hit.transform.gameObject.tag.StartsWith("Section");
			bool isBall = hit.transform.gameObject.tag.StartsWith("Ball");

			if(!isSection && !isBall) {
				return;
			}

			if(Utils.IsCalculates(hit.transform.gameObject)) {
				return;
			}

			_selectObject = hit.transform.gameObject;
			_positionDown = position;

			if(isSection) {
				_isTouchAnalyzed = true;
				selectSectionAndBalls(_selectObject);
			}
			else if(isBall) {	
				_isTouchAnalyzed = false;
				selectSectionAndBalls(GameObject.FindGameObjectsWithTag("Section").First(section => Mathf.Abs(section.transform.position.y + 0.5f - _selectObject.transform.position.y) < Model.rowDistanceHalf));
			}
		}
	}

	void unholdObjects(List<GameObject> objects) {
		for(int i = 0; i < objects.Count; i++) {
			MagnetismColumn magCol = objects[i].GetComponent<MagnetismColumn>();
			
			if(magCol) {
				magCol.isCalculates = true;
			}
			
			MagnetismRow magRow = objects[i].GetComponent<MagnetismRow>();
			
			if(magRow) {
				magRow.isAlwaysCalculate = false;
				magRow.isCalculates = true;
			}
		}
	}

	void resetTouchSettings() {
		unholdObjects (_selectObjects);

		touch = TouchType.none;
		_selectObject = null;
		_selectObjects.Clear();
		_selectObjectsUp.Clear();
		_selectObjectsDown.Clear();
	}

	void rotateObjects(float angle) {
		for (int i = 0; i < _selectObjects.Count; i++) {
			_selectObjects[i].transform.RotateAround(Vector3.zero, Vector3.up, angle);
		}
	}
	
	void moveObjects(float move) {
		if(Mathf.Abs(move) > Model.EPS) {
			List<GameObject> objectsToMove = new List<GameObject>();
			objectsToMove.Add(_selectObject);
			List<float> spaceFree = new List<float>();
			if(move > 0.0f) {
				objectsToMove.AddRange(_selectObjectsUp);
			}	
			else {
				objectsToMove.AddRange(_selectObjectsDown);
			}

			for(int i = 0; i < objectsToMove.Count - 1; i++) {
					spaceFree.Add(Mathf.Clamp(move > 0.0f ? 
					                          objectsToMove[i + 1].transform.position.y - objectsToMove[i].transform.position.y - Model.rowDistance :
					                          objectsToMove[i].transform.position.y - objectsToMove[i + 1].transform.position.y - Model.rowDistance
					                          , 0.0f, Model.rowDistance));
			}
			spaceFree.Add(Mathf.Clamp(move > 0.0f ? 
			                          Model.maxY - objectsToMove[objectsToMove.Count - 1].transform.position.y :
			                          objectsToMove[objectsToMove.Count - 1].transform.position.y - moveYMinCurrent
			                          , 0.0f, Model.rowDistance));

			bool spaceFreeExist = spaceFree.Exists(p => Mathf.Abs(p) > Model.EPS);
			float moveAmount = move;

			while(Mathf.Abs(moveAmount) > Model.EPS && spaceFreeExist) {
				for(int i = 0; i < spaceFree.Count; i++) {
					if(Mathf.Abs(spaceFree[i]) > Model.EPS) {
						if(spaceFree[i] >= Mathf.Abs(moveAmount)) {
							for(int j = 0; j <= i; j++) {
								objectsToMove[j].transform.Translate(0.0f, moveAmount, 0.0f);
							}
							moveAmount = 0.0f;
							break;
						}
						else {
							float moveObject = spaceFree[i] * Mathf.Sign(moveAmount);
							objectsToMove[i].transform.Translate(0.0f, moveObject, 0.0f);
							moveAmount -= moveObject;
							spaceFree[i] = 0.0f;
							break;
						}
					}
				}

				spaceFreeExist = spaceFree.Exists(p => Mathf.Abs(p) > Model.EPS);
			}
		}
	}

	void selectSectionAndBalls(GameObject obj) {
		touch = TouchType.rotate;
		_selectObjects.AddRange(GameObject.FindGameObjectsWithTag("Ball"));
		
		for(int i = 0; i < _selectObjects.Count; i++) {
			if(Mathf.Abs(obj.transform.position.y + 0.5f - _selectObjects[i].transform.position.y) > Model.rowDistanceHalf) {
				_selectObjects.RemoveAt(i);
				i--;
			}
		}
		
		_selectObjects.Add(obj);
	}
} //End class