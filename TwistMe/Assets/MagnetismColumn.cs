using UnityEngine;
using System.Collections;

public class MagnetismColumn : MonoBehaviour {
	public float angleShift = 0.0f;
	public float rotateSpeed = 1.0f;
	public bool isCalculates = false;

	void Update () {
		if (isCalculates) {
			float rotateNeedMax = -Utils.Mod(transform.localEulerAngles.y + angleShift, -Model.angleSideHalf, Model.angleSideHalf);
			float sideHalfRotationCurrent = Model.angleSideHalf - Mathf.Abs(rotateNeedMax);
			float pathCoeffCurrent = sideHalfRotationCurrent / Model.angleSideHalf;
			float timeCurrent = 1.0f - Mathf.Sqrt(Mathf.Clamp01(1.0f - pathCoeffCurrent)); //обратная формула параболы
			float timeNeed = Mathf.Clamp01(timeCurrent + Time.deltaTime * rotateSpeed);
			float pathCoeffNeed = 1.0f - (1.0f - timeNeed) * (1.0f - timeNeed); //формула параболы
			float sideHalfRotationNeed = pathCoeffNeed * Model.angleSideHalf;
			float rotationStep = Mathf.Sign(rotateNeedMax) * (sideHalfRotationNeed - sideHalfRotationCurrent);
			transform.RotateAround(Vector3.zero, Vector3.up, rotationStep);

			if(Mathf.Abs(rotationStep) < Model.EPS) {
				isCalculates = false;
			}
		}
	}
}
