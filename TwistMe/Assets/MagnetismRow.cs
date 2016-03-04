using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagnetismRow : MonoBehaviour {
	public float moveSpeed = 1.0f;
	public bool isCalculates = false;
	public bool isAlwaysCalculate = false;
	public List<GameObject> objectsCollision = null;
	private float _moveStep = 0.0f;

	void Update () {
		if (isAlwaysCalculate || isCalculates) {
			float moveNeedMax = -Utils.Mod(transform.position.y - Model.rowZeroY, -Model.rowDistanceHalf, Model.rowDistanceHalf);
			float rowHalfMoveCurrent = Model.rowDistanceHalf - Mathf.Abs(moveNeedMax);
			float pathCoeffCurrent = rowHalfMoveCurrent / Model.rowDistanceHalf;
			float timeCurrent = 1.0f - Mathf.Sqrt(Mathf.Clamp01(1.0f - pathCoeffCurrent)); //обратная формула параболы
			float timeNeed = Mathf.Clamp01(timeCurrent + Time.deltaTime * moveSpeed);
			float pathCoeffNeed = 1.0f - (1.0f - timeNeed) * (1.0f - timeNeed); //формула параболы
			float rowHalfMoveNeed = pathCoeffNeed * Model.rowDistanceHalf;
			_moveStep = Mathf.Sign(moveNeedMax) * (rowHalfMoveNeed - rowHalfMoveCurrent);

			if(Mathf.Abs(_moveStep) > Model.EPS) {
				if(objectsCollision != null) {
					for(int i = 0; i < objectsCollision.Count; i++) {
						if(!objectsCollision[i].GetComponent<MagnetismRow>().isCalculates) {
							if(_moveStep > 0.0f) {
								if(transform.position.y > objectsCollision[i].transform.position.y) {
									continue;
								}

								_moveStep = Mathf.Clamp(_moveStep, 0.0f, objectsCollision[i].transform.position.y - transform.position.y - Model.rowDistance);
							}
							else {
								if(transform.position.y < objectsCollision[i].transform.position.y) {
									continue;
								}

								_moveStep = Mathf.Clamp(_moveStep, objectsCollision[i].transform.position.y - transform.position.y + Model.rowDistance, 0.0f);
							}
						}
					}
				}
			}

			transform.Translate (0.0f, _moveStep, 0.0f);

			if(Mathf.Abs(_moveStep) < Model.EPS) {
				isCalculates = false;
			}
		}
	}
}
