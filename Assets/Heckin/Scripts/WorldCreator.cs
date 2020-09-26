using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WorldCreator : MonoBehaviour
{
	public static WorldCreator Instance;

	[Header( "References" )]
	public GameObject[] Layout;

	[Header( "Assets" )]
	public GameObject BasePrefab;
	public GameObject OtherPrefab;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		// Store initial grid chunk positions
		int x = 0;
		int y = 0;
		foreach ( var block in Layout )
		{
			block.GetComponent<Chunk>().Pos = new Vector2( x, y );
			if ( x == 1 && y == 1 )
			{
				Chunk.LastPlayerChunk = block.GetComponent<Chunk>();
			}

			x++;
			if ( x > 2 )
			{
				y++;
				x = 0;
			}

			block.GetComponent<Chunk>().UpdateText();
			Chunk.Chunks.Add( block.GetComponent<Chunk>() );
		}
	}

	void Update()
	{
		//Generate();
		if ( Input.GetKeyDown( KeyCode.DownArrow ) )
		{
			Chunk.MoveDir( new Vector2( 0, 1 ) );
		}
		if ( Input.GetKeyDown( KeyCode.UpArrow ) )
		{
			Chunk.MoveDir( new Vector2( 0, -1 ) );
		}
		if ( Input.GetKeyDown( KeyCode.LeftArrow ) )
		{
			Chunk.MoveDir( new Vector2( -1, 0 ) );
		}
		if ( Input.GetKeyDown( KeyCode.RightArrow ) )
		{
			Chunk.MoveDir( new Vector2( 1, 0 ) );
		}
	}

	void Generate()
    {
		foreach ( Transform child in transform )
		{
			DestroyImmediate( child.gameObject );
		}

		GameObject bas = Instantiate( BasePrefab, transform );
		bas.transform.localPosition = Vector3.zero;

		// Find anchors
		var ancs = bas.transform.Find( "Anchors" );
		int index = 0;
		foreach ( Transform anchor in ancs )
		{
			GameObject other = Instantiate( OtherPrefab, anchor );
			other.transform.localPosition = Vector3.zero;
			other.transform.parent = transform;

			if ( index < 2 )
			{
				var childancs = other.transform.Find( "Anchors" );
				int childindex = 0;
				foreach ( Transform childanchor in childancs )
				{
					if ( childindex >= 2 )
					{
						other = Instantiate( OtherPrefab, childanchor );
						other.transform.localPosition = Vector3.zero;
						other.transform.parent = transform;
					}

					childindex++;
				}
			}
			index++;
		}
    }
}
