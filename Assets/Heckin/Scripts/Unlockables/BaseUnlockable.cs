using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heck
{
	public class BaseUnlockable
	{
		public bool GetIsUnlocked()
		{
			return UnlockManager.Instance.GetUnlocked( this.GetType() );
		}

		public virtual void OnUnlock()
		{

		}
	}
}
