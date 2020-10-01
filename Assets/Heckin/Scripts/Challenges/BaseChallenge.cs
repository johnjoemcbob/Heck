using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heck
{
	public class BaseChallenge
	{
		public virtual bool Check()
		{
			return false;
		}

		public virtual string GetText()
		{
			return "BASE CHALLENGE TEXT";
		}
	}
}
