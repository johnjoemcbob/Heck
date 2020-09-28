using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BordFollower : MonoBehaviour
{
	public Transform ToFollow;
	public BordFollower Followee;

	private Animator _animator;

	private void Awake()
	{
		transform.localScale = Vector3.one * 20 / 100.0f;
	}

	void Update()
    {
		float speed = 0;

		// Walk towards followee, set by Bord.cs
		float dist = Vector3.Distance( ToFollow.position, transform.position );
		if ( dist >= 1.5f )
		{
			transform.position = Vector3.MoveTowards( transform.position, ToFollow.position, Time.deltaTime * 10 );
			transform.LookAt( ToFollow );
			transform.localEulerAngles = new Vector3( 0, transform.localEulerAngles.y, 0 );
			speed = 1;
		}

		_animator.SetFloat( "HorizontalSpeed", speed );
		_animator.SetFloat( "VerticalSpeed", ( transform.position.y > 0.1f ) ? 1 : 0 );
		_animator.SetBool( "IsGrounded", ( transform.position.y < 0.1f ) );
	}

	public void SetAnimal( Player.Animal anim )
	{
		foreach ( Transform child in transform )
		{
			if ( child.GetSiblingIndex() != (int) anim )
			{
				child.gameObject.SetActive( false );
				Destroy( child.gameObject );
			}
		}

		_animator = GetComponentInChildren<Animator>();
	}

	public void Chirp()
	{
		StartCoroutine( Co_Chirp() );
	}

	IEnumerator Co_Chirp()
	{
		yield return new WaitForSeconds( Random.Range( 0.1f, 0.2f ) / 2 );

		StaticHelpers.GetOrCreateCachedAudioSource( "chirp" + Random.Range( 3, 5 ), transform.position, Random.Range( 0.8f, 1.2f ), 0.5f );
		GetComponentInChildren<Punchable>().Punch();

		yield return new WaitForSeconds( Random.Range( 0.1f, 0.2f ) / 2 );

		if ( Followee != null )
		{
			Followee.Chirp();
		}
	}
}
