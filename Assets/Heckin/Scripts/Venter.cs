using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Venter : MonoBehaviour
{
	public GameObject Prefab;

    void Start()
    {
		GameObject obj = Instantiate( Prefab, transform );
		obj.transform.localPosition = new Vector3( 0, 0, 1.3f );
		obj.transform.eulerAngles = new Vector3( 0, 0, 0 );
    }
}
