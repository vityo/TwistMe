using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
	public Material ballMaterial = null;

	void Start () {
		Renderer rend = GetComponent<Renderer>();
		rend.material = ballMaterial;
	}
}
