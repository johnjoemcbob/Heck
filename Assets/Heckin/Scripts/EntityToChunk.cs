using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityToChunk : MonoBehaviour
{
    void Update()
    {
		if ( Chunk.Chunks.Count > 0 )
		{
			float maxdist = -1;
			var closest = Chunk.Chunks[0];
			foreach ( var chunk in Chunk.Chunks )
			{
				var dist = Vector3.Distance( transform.position, chunk.transform.position );
				if ( maxdist == -1 || dist < maxdist )
				{
					closest = chunk;
					maxdist = dist;
				}
			}
			transform.parent = closest.transform;
			Destroy( this );
		}
    }
}
