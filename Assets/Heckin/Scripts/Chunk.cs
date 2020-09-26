﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chunk : MonoBehaviour
{
	public static Chunk LastPlayerChunk;
	public static List<Chunk> Chunks = new List<Chunk>();

	public Vector2 Pos;

	private static float timesx = 0;
	private static float timesy = 0;
	private static float tempcooldown = 0;
	private static Vector2 CachedMoveDir;

	private void OnTriggerEnter( Collider other )
	{
		if ( LastPlayerChunk == this ) return;
		if ( other.tag != "Player" ) return;
		if ( LastPlayerChunk == null )
		{
			LastPlayerChunk = this;
			return;
		}
		if ( tempcooldown > Time.time ) return;
		//MoveDir( Pos - LastPlayerChunk.Pos );
	}

	private void Update()
	{
		float distancex = Mathf.Abs( transform.position.x - Bord.Instance.transform.position.x );
		float distancey = Mathf.Abs( transform.position.z - Bord.Instance.transform.position.z );
		//Debug.Log( distancex + " " + distancey );
		
		float MOVE_TRIGGER = 40;
		float x = -Mathf.Sign( transform.position.x - Bord.Instance.transform.position.x );
		if ( distancex > MOVE_TRIGGER && -Mathf.Sign( Bord.Instance.Direction.x ) == x )
		{
			CachedMoveDir = new Vector2( x, CachedMoveDir.y );
		}
		float y = Mathf.Sign( transform.position.z - Bord.Instance.transform.position.z );
		if ( distancey > MOVE_TRIGGER && Mathf.Sign( Bord.Instance.Direction.z ) == y )
		{
			CachedMoveDir = new Vector2( CachedMoveDir.x, Mathf.Sign( transform.position.z - Bord.Instance.transform.position.z ) );
		}
	}

	private void FixedUpdate()
	{
		MoveDir( CachedMoveDir );
		CachedMoveDir = Vector2.zero;
	}

	public static void MoveDir( Vector2 dir )
	{
		Chunks[0].StartCoroutine( Co_MoveDir( dir ) );
	}

	static IEnumerator Co_MoveDir( Vector2 dir )
	{
		var startpos = LastPlayerChunk.Pos;
		if ( dir.x != 0 )
		{
			// Find those in the opposite direction of movement
			foreach ( var chunk in Chunks )
			{
				if ( chunk.Pos.x == startpos.x - dir.x )
				{
					// Move it to parent at the other end
					foreach ( var oppchunk in Chunks )
					{
						// Find the other by opposite x direction and same y value
						if ( oppchunk.Pos.x == startpos.x + dir.x && chunk.Pos.y == oppchunk.Pos.y )
						{
							var anchors = oppchunk.transform.Find( "Anchors" );
							var furthest = anchors.GetChild( 0 );
							//if ( ( dir.x == -1 && timesx >= 2 ) || ( dir.x == 1 && timesx < 2 ) )
							if ( dir.x == 1 )
							{
								var x = furthest.position.x;
								foreach ( Transform anchor in anchors )
								{
									if ( anchor.position.x > x )
									{
										furthest = anchor;
										x = anchor.position.x;
									}
								}
							}
							else
							{
								var x = furthest.position.x;
								foreach ( Transform anchor in anchors )
								{
									if ( anchor.position.x < x )
									{
										furthest = anchor;
										x = anchor.position.x;
									}
								}
							}

							// Move the chunk to local zero of the anchor and then unparent again after
							chunk.transform.SetParent( furthest, false );
							chunk.transform.localPosition = Vector3.zero;
							chunk.transform.localEulerAngles = Vector3.zero;
							chunk.transform.localScale = Vector3.one;
							chunk.Pos = oppchunk.Pos + new Vector2( dir.x, 0 );
							chunk.UpdateText();
							chunk.transform.parent = WorldCreator.Instance.transform;
						}
					}
				}
			}
			timesx++;
		}

		//yield return new WaitForSeconds( 2 );

		if ( dir.y != 0 )
		{
			// Find those in the opposite direction of movement
			foreach ( var chunk in Chunks )
			{
				if ( chunk.Pos.y == startpos.y - dir.y )
				{
					// Move it to parent at the other end
					foreach ( var oppchunk in Chunks )
					{
						// Find the other by opposite x direction and same y value
						if ( oppchunk.Pos.y == startpos.y + dir.y && chunk.Pos.x == oppchunk.Pos.x )
						{
							var anchors = oppchunk.transform.Find( "Anchors" );
							var furthest = anchors.GetChild( 0 );
							//if ( ( dir.y == -1 && times >= 2 ) || ( dir.y == 1 && times < 2 ) )
							//if ( dir.y == -1 )
							if ( dir.y == 1 )
							{
								var y = furthest.position.z;
								foreach ( Transform anchor in anchors )
								{
									if ( anchor.position.z < y )
									{
										furthest = anchor;
										y = anchor.position.z;
									}
								}
							}
							else
							{
								var y = furthest.position.z;
								foreach ( Transform anchor in anchors )
								{
									if ( anchor.position.z > y )
									{
										furthest = anchor;
										y = anchor.position.z;
									}
								}
							}

							// Move the chunk to local zero of the anchor and then unparent again after
							chunk.transform.SetParent( furthest, false );
							chunk.transform.localPosition = Vector3.zero;
							chunk.transform.localEulerAngles = Vector3.zero;
							chunk.transform.localScale = Vector3.one;
							chunk.Pos = oppchunk.Pos + new Vector2( 0, dir.y );
							chunk.UpdateText();
							chunk.transform.parent = WorldCreator.Instance.transform;
						}
					}
				}
			}
			timesy++;
		}

		// Store the new center
		foreach ( var chunk in Chunks )
		{
			if ( LastPlayerChunk.Pos + dir == chunk.Pos )
			{
				LastPlayerChunk = chunk;
				break;
			}
		}

		yield break;
	}

	public void UpdateText()
	{
		//GetComponentInChildren<Text>().text = Pos.ToString();
	}
}
