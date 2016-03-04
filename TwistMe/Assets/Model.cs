using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Model {
	public static float rowZeroY = 0.0f; //1 ряд шаров
	public static float rowDistance = 0.0f;
	public static float rowDistanceHalf = 0.0f;
	public static int rowCount = 5;
	public static int rowHole = -1;
	public static float rowHoleY = 0.0f; //0 ряд шаров
	public static float maxY = 0.0f;
	public static float EPS = 0.0001f;
	public static List<GameObject> objects = new List<GameObject> ();
	public static float angleSideHalf = 36.0f;
	public static float angleSide = 72.0f;
}
