using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vent : MonoBehaviour
{
	public static float Speed = 1;

	private void OnTriggerStay( Collider collider )
	{
		var ply = collider.transform.GetComponentInParent<Player>();
		if ( LocalPlayer.Instance.Player != null && ply != null && ply == LocalPlayer.Instance.Player )
		{
			FindObjectOfType<NaughtyCharacter.Character>().AddUpVelocity( Speed );
			//StaticHelpers.GetOrCreateCachedAudioSource( "boing", LocalPlayer.Instance.Player.transform, Random.Range( 0.8f, 1.2f ), 1, 0, true );
		}
	}
}
