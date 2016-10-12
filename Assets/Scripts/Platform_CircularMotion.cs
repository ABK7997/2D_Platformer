using UnityEngine;
using System.Collections;

public class Platform_CircularMotion : MonoBehaviour {

    private float time = 0, startX, startY;
    public float speed, radius;

	// Use this for initialization
	void Start () {
        startX = transform.localPosition.x;
        startY = transform.localPosition.y;
	}
	
	// Update is called once per frame
	void Update () {
        time += (Time.deltaTime * speed);

        Vector3 newPos = new Vector3(startX + (Mathf.Cos(time) * radius), startY + (Mathf.Sin(time) * radius), 0f);
        transform.localPosition = newPos;
	}
}
