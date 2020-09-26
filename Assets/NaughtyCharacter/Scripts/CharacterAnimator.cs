using UnityEngine;

namespace NaughtyCharacter
{
	public static class CharacterAnimatorParamId
	{
		public static readonly int HorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
		public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
		public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
	}

	public class CharacterAnimator : MonoBehaviour
	{
		public Animator _animator;
		public CharacterController _controller;
		public Character _character;

		Vector3 m_LastPosition;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_controller = GetComponent<CharacterController>();
			//_character = GetComponent<Character>();
		}

		public void Update()
		{
			if ( _character != null )
			{
				// Local player
				float normHorizontalSpeed = _character.HorizontalVelocity.magnitude / _character.MovementSettings.MaxHorizontalSpeed;
				_animator.SetFloat( CharacterAnimatorParamId.HorizontalSpeed, normHorizontalSpeed );

				float jumpSpeed = _character.MovementSettings.JumpSpeed;
				float normVerticalSpeed = _character.VerticalVelocity.y.Remap(-jumpSpeed, jumpSpeed, -1.0f, 1.0f);
				_animator.SetFloat( CharacterAnimatorParamId.VerticalSpeed, normVerticalSpeed );

				_animator.SetBool( CharacterAnimatorParamId.IsGrounded, _character.IsGrounded );
			}
			else
			{
				// Networked player
				Vector3 movementVector = transform.position - m_LastPosition;

				float speed = Vector3.Dot( movementVector.normalized, transform.forward );

				_animator.SetFloat( CharacterAnimatorParamId.HorizontalSpeed, speed );
				_animator.SetFloat( CharacterAnimatorParamId.VerticalSpeed, 0 );

				_animator.SetBool( CharacterAnimatorParamId.IsGrounded, true );

				m_LastPosition = Vector3.Lerp( m_LastPosition, transform.position, Time.deltaTime * 10000 );
			}
		}
	}
}
