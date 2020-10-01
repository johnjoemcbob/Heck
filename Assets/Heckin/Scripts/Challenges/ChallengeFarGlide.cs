using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heck
{
	public class ChallengeFarGlide : BaseChallenge
	{
		public const float Distance = 100;

		private Vector3 LastPos = Vector3.zero;
		private float CurrentDistance = 0;

		public override bool Check()
		{
			if ( LocalPlayer.Instance != null && LocalPlayer.Instance.Player != null )
			{
				// If grounded then reset
				if ( NaughtyCharacter.Character.Instance.IsGrounded )
				{
					CurrentDistance = 0;
				}
				else
				{
					// Check distance from last frame and add to total
					LastPos.y = LocalPlayer.Instance.Player.transform.position.y; // Don't include y distance travelled
					CurrentDistance += Vector3.Distance( LocalPlayer.Instance.Player.transform.position, LastPos );

					if ( CurrentDistance >= Distance )
					{
						return true;
					}
				}
				LastPos = LocalPlayer.Instance.Player.transform.position;
			}
			return false;
		}

		public override string GetText()
		{
			return "Glide 100 meters in one flight!";
		}
	}
}
