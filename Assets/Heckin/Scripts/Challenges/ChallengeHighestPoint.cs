using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heck
{
	public class ChallengeHighestPoint : BaseChallenge
	{
		public const float Highest = 20;

		public override bool Check()
		{
			if ( LocalPlayer.Instance != null && LocalPlayer.Instance.Player != null )
			{
				if ( LocalPlayer.Instance.Player.transform.localPosition.y >= Highest )
				{
					return true;
				}
			}
			return false;
		}

		public override string GetText()
		{
			return "Fly to the highest point in the world!";
		}
	}
}
