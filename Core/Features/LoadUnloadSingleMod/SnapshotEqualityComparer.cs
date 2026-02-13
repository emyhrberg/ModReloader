using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ModReloader.Core.Features.LoadUnloadSingleMod;

/// <summary>
/// Equality comparer for snapshot systems that compares reference types by reference
/// and value types by structural equality.
/// </summary>
public class SnapshotEqualityComparer : IEqualityComparer<object>
{
	public static readonly SnapshotEqualityComparer Instance = new();

	public new bool Equals(object? x, object? y)
	{
		if (x is null && y is null) return true;
		if (x is null || y is null) return false;

		// Reference types — compare by reference
		if (!x.GetType().IsValueType)
			return ReferenceEquals(x, y);

		// Value types — structural equality
		return x.GetType() == y.GetType() && x.Equals(y);
	}

	public int GetHashCode(object obj)
	{
		if (obj.GetType().IsValueType)
			return obj.GetHashCode();

		return RuntimeHelpers.GetHashCode(obj);
	}
}
