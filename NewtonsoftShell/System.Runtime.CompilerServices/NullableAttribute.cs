using System;
using System.Runtime.CompilerServices;
using HumanStoryteller.NewtonsoftShell.Microsoft.CodeAnalysis;

namespace HumanStoryteller.NewtonsoftShell.System.Runtime.CompilerServices;

	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
	internal sealed class NullableAttribute : Attribute
	{
		public readonly byte[] NullableFlags;

		public NullableAttribute(byte P_0)
		{
			NullableFlags = new byte[1]
			{
				P_0
			};
		}

		public NullableAttribute(byte[] P_0)
		{
			NullableFlags = P_0;
		}
	}
