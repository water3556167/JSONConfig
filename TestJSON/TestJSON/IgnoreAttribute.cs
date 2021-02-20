using System;

namespace TestJSON
{
	[System.AttributeUsage(AttributeTargets.All,Inherited =false,AllowMultiple =true)]
	internal class IgnoreAttribute : Attribute
	{
	}
}