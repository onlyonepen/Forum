using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnModel : MonoBehaviour
{
	private Button theButton;

	[SerializeField]
    private painterScript painter;
	[SerializeField]
    private GameObject model;

	private GameObject clone;
	void Start () {
		Button btn = transform.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick(){
		clone = Instantiate(model);
		clone.tag = "TargetObject";
		painter.InitNewModel();
	}
}
