using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDayCycle : MonoBehaviour
{
    public float speed = 10f;

	void Update ()
    {
	    transform.Rotate(Time.deltaTime * speed, 0, 0);
	}
}
