using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
	public float BODY_SPEED = 1;
	public float BODY_MOVEDIST = 0.5f;
	public float WHEEL_SPEED = 5;
	public float MoveSpeed = 0.05f;

	private Transform Body;
	private Transform[] Wheels;
	private Vector3 Direction;

	void Start()
    {
		Body = transform.GetChild( 0 );

		// Find wheels here
		Wheels = new Transform[4];
		for ( int i = 0; i < 4; i++ )
		{
			Wheels[i] = transform.GetChild( i + 1 );
		}

		Direction = transform.forward;
	}

	void Update()
	{
		// Move forward speed
		transform.position += Direction * MoveSpeed;

		// Find distance from parent
		var dist = 50;
		var dir = Vector2.zero;
		{
			if ( transform.localPosition.x >= dist )
			{
				dir.x = 1;
			}
			if ( transform.localPosition.x <= -dist )
			{
				dir.x = -1;
			}
			if ( transform.localPosition.z >= dist )
			{
				dir.y = 1;
			}
			if ( transform.localPosition.z <= -dist )
			{
				dir.y = -1;
			}
		}
		if ( dir != Vector2.zero )
		{
			// Get next chunk pos, or loop around
			var chunk = transform.parent.GetComponent<Chunk>();
			chunk = Chunk.GetNext( chunk.Pos, dir );

			// Parent to that, reset pos if looped around
			transform.parent = chunk.transform;
			if ( Mathf.Abs( transform.localPosition.x ) >= dist )
			{
				transform.localPosition = new Vector3( 0, transform.localPosition.y, transform.localPosition.z );
			}
			if ( Mathf.Abs( transform.localPosition.z ) >= dist )
			{
				transform.localPosition = new Vector3( transform.localPosition.x, transform.localPosition.y, 0 );
			}
		}

		// Find ground to parent
		//RaycastHit hit;
		//if ( Physics.Raycast( transform.position, -Vector3.up, out hit, Mathf.Infinity ) )
		//{
		//	transform.parent = hit.transform;
		//}
		//else if ( transform.parent != null && transform.parent.GetComponent<Chunk>() != null )
		//{
		//	// Loop around if no ground found
		//	//transform.position -= Direction * 30 * ( 5 - 1 ); // Chunk size * chunk count? maybe work
		//	if ( Direction.x != 0 )
		//	{
		//		transform.parent = Chunk.GetOpposite( transform.parent.GetComponent<Chunk>(), true ).transform;
		//		transform.localPosition = new Vector3( 0, transform.localPosition.y, transform.localPosition.z );
		//	}
		//	if ( Direction.z != 0 )
		//	{
		//		transform.parent = Chunk.GetOpposite( transform.parent.GetComponent<Chunk>(), false ).transform;
		//		transform.localPosition = new Vector3( transform.localPosition.x, transform.localPosition.y, 0 );
		//	}
		//}

		// Lerp wheels and body animate
		foreach ( var wheel in Wheels )
		{
			wheel.transform.localEulerAngles += new Vector3( Time.deltaTime, 0, 0 ) * WHEEL_SPEED;
		}
		Body.localPosition = new Vector3( 0, ( 1 + Mathf.Sin( Time.time * BODY_SPEED ) ) * BODY_MOVEDIST, 0 );
	}
}
