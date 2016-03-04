using UnityEngine;
using System.Collections;

public static class Utils {
	// value ∈ [min, max)
	public static float Mod(float value, float min, float max) {
		float distance = max - min;

		while (value < min) {
			value += distance;
		}
		
		while (value >= max) {
			value -= distance;
		}

		return value;
	}

	public static float ToAngleTwist(float angle) {
		return Mod(angle, -Model.angleSideHalf, 360.0f - Model.angleSideHalf);
	}

	public static bool IsCalculates(GameObject obj) {
		MagnetismColumn magCol = obj.GetComponent<MagnetismColumn> ();

		if (magCol && magCol.isCalculates) {
			return true;
		}

		MagnetismRow magRow = obj.GetComponent<MagnetismRow> ();

		if(magRow && magRow.isCalculates) {
			return true;
		}

		return false;
	}
}
