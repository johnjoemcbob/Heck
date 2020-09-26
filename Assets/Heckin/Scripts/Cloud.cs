using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
	public float Speed = 1;
	public float Distance = 30;

	private Vector3 StartPos = Vector3.zero;

	void Update()
	{
		if ( StartPos == Vector3.zero )
		{
			StartPos = transform.localPosition;
		}

		transform.localPosition = StartPos +
			new Vector3( 1, 0, 0 ) * Mathf.Sin( Time.time * Speed ) * Distance +
			new Vector3( 0, 0, 1 ) * Mathf.Cos( Time.time * Speed ) * -Distance +
			new Vector3( 0, 1, 0 ) * Mathf.Sin( Time.time * Speed ) * Mathf.Cos( Time.time * Speed );
	}
}
