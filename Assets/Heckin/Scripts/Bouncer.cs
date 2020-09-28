using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
	public static float Speed = 20;

	private void OnTriggerEnter( Collider collider )
	{
		var ply = collider.transform.GetComponentInParent<Player>();
		if ( LocalPlayer.Instance.Player != null && ply != null && ply == LocalPlayer.Instance.Player )
		{
			FindObjectOfType<NaughtyCharacter.Character>().SetUpVelocity( Speed );
			StaticHelpers.GetOrCreateCachedAudioSource( "boing", LocalPlayer.Instance.Player.transform, Random.Range( 0.8f, 1.2f ), 1, 0, true );

			GameObject par = StaticHelpers.EmitParticleDust( ply.transform.position );
			par.transform.localScale *= 2;
		}
	}
}
