using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heck
{
	public class UnlockableCharacter : BaseUnlockable
	{
		public int Character = 0;
		public UnlockableCharacter( int character )
		{
			Character = character;
		}

		public override void OnUnlock()
		{
			Game.Instance.UnlockedCharacters[Character] = true;
		}
	}
}
