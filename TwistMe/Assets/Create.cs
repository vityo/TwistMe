using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Create : MonoBehaviour {
	public float sectionDistance = 0.2f;
	public Material[] materials;

	void Start () {
		Model.rowDistance = sectionDistance + 1.0f;
		Model.rowDistanceHalf = Model.rowDistance * 0.5f;
		Model.rowCount = materials.Length;

		ModelStart modelStart = GameObject.Find ("ModelStart").GetComponent<ModelStart> ();
		modelStart.objectStartSectionHole.transform.Translate(0.0f, -sectionDistance, 0.0f);
		modelStart.objectStartCapDown.transform.Translate(0.0f, 2 * sectionDistance, 0.0f);
		string sectionFirstName = modelStart.objectStartSection.name;
		modelStart.objectStartSection.name += "1";

		for (int i = 1; i <= Model.rowCount; i++) {
			if (i == 1) {
				continue;
			}

			GameObject sect = (GameObject)Instantiate (modelStart.objectStartSection, 
			                                           new Vector3(0.0f, modelStart.objectStartSection.transform.position.y + (i - 1) * (1.0f + sectionDistance), 0.0f ), Quaternion.identity);
			sect.transform.RotateAround(new Vector3(0.0f, 0.0f, 0.0f), Vector3.up, -18);
			sect.name = sectionFirstName + i.ToString();
		}

		modelStart.objectStartCapUp.transform.Translate(0.0f, Model.rowCount * Model.rowDistance, 0.0f);

		string ballFirstName = modelStart.objectStartBall.name;
		modelStart.objectStartBall.name += "1";
		List<Material> materialsToBalls = new List<Material>();
		
		for (int i = 0; i < Model.rowCount; i++) {
			materialsToBalls.AddRange (materials);
		}

		setMaterialToBall(modelStart.objectStartBall, ref materialsToBalls);
		Model.rowZeroY = modelStart.objectStartBall.transform.position.y;
		Model.rowHoleY = Model.rowZeroY - Model.rowDistance;
		Model.maxY = Model.rowZeroY + (Model.rowCount - 1) * Model.rowDistance;

		for (int j = 0; j < Model.rowCount; j++) {
			for (int i = 0; i < Model.rowCount; i++) {
				if (i == 0 && j == 0) {
					continue;
				}

				GameObject ball = (GameObject)Instantiate (modelStart.objectStartBall, 
				                                           new Vector3(0.0f, modelStart.objectStartBall.transform.position.y + i * (1.0f + sectionDistance), modelStart.objectStartBall.transform.position.z ), 
				                                          Quaternion.identity);
				ball.transform.RotateAround(Vector3.zero, Vector3.up, j * 360 / Model.rowCount);
				ball.name = ballFirstName + (j * Model.rowCount + i + 1).ToString();
				setMaterialToBall(ball, ref materialsToBalls);
			}
       	}

		Model.objects.AddRange (GameObject.FindGameObjectsWithTag ("Ball"));
		Model.objects.AddRange (GameObject.FindGameObjectsWithTag ("Section"));
	}

	void setMaterialToBall(GameObject ball, ref List<Material> materials) {
		int materialIndex = Random.Range(0, materials.Count - 1);
		ball.GetComponent<Ball>().ballMaterial = materials[materialIndex];
		materials.RemoveAt(materialIndex);
	}
}
