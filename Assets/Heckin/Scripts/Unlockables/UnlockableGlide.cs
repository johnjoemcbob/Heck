using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heck
{
	public class UnlockableGlide : BaseUnlockable
	{
		public override void OnUnlock()
		{

		}

		public float Try( bool _jumpInput, float gravity )
		{
			if ( _jumpInput && GetIsUnlocked() )
			{
				gravity /= 30;
			}
			return gravity;
		}
	}
}
