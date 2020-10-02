using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heck
{
	public class ChallengeChildren : BaseChallenge
	{
		public const float Goal = 5;

		public override bool Check()
		{
			var count = 0;
			if ( LocalPlayer.Instance != null && LocalPlayer.Instance.Player != null )
			{
				var child = LocalPlayer.Instance.Player.Followee;
				while ( child != null )
				{
					count++;
					child = child.Followee;
				}
			}
			return ( count >= Goal );
		}

		public override string GetText()
		{
			return "Have a just, like, a whole bunch of kids!";
		}
	}
}
