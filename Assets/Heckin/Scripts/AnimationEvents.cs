using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
	public bool Follower = false;

	public void Footstep()
	{
		StaticHelpers.GetOrCreateCachedAudioSource( "footstep", transform.position, Random.Range( 0.8f, 1.2f ), Follower ? 0.1f : 0.2f, 0, !Follower );
	}

	public void Flap()
	{
		if ( !Follower )
		{
			StaticHelpers.GetOrCreateCachedAudioSource( "flap" + Random.Range( 1, 4 ), transform.position, Random.Range( 0.8f, 1.2f ), 0.5f );
		}
	}
}
