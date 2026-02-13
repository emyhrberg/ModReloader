using System.Collections.Generic;

namespace ModReloader.Core.Features.LoadUnloadSingleMod;

/// <summary>
/// Defines how to create snapshots for manual snapshot system.
/// </summary>
internal interface ISnapshotDescriptor
{
	void CreateSnapshot(object value, ManualSnapshotRegistry registry, int depth);
	List<FieldDescriptor> NestedFields { get; }
	CollectionDescriptor CollectionElementInfo { get; }
	List<TupleElementDescriptor> TupleElementsInfo { get; }
}

/// <summary>
/// Describes how to snapshot collection elements.
/// </summary>
public class CollectionDescriptor : ISnapshotDescriptor
{
	public List<FieldDescriptor> NestedFields { get; }
	public CollectionDescriptor CollectionElementInfo { get; }
	public List<TupleElementDescriptor> TupleElementsInfo { get; }

	public CollectionDescriptor(
		List<FieldDescriptor> nestedElements = null,
		CollectionDescriptor collectionElementInfo = null,
		List<TupleElementDescriptor> tupleElementsInfo = null)
	{
		NestedFields = nestedElements;
		CollectionElementInfo = collectionElementInfo;
		TupleElementsInfo = tupleElementsInfo;
	}

	public void CreateSnapshot(object value, ManualSnapshotRegistry registry, int depth)
	{
		if (value == null)
			return;

		registry.CreateSnapshot(value, NestedFields, CollectionElementInfo, TupleElementsInfo, depth);
	}
}

/// <summary>
/// Describes how to snapshot a specific field by name.
/// </summary>
public class FieldDescriptor : CollectionDescriptor
{
	public string Name { get; }

	public FieldDescriptor(string fieldName,
		List<FieldDescriptor> nestedElements = null,
		CollectionDescriptor collectionElementInfo = null,
		List<TupleElementDescriptor> tupleElementsInfo = null) : base(nestedElements, collectionElementInfo, tupleElementsInfo)
	{
		Name = fieldName;
	}
}

/// <summary>
/// Describes how to snapshot a specific tuple element by index.
/// </summary>
public class TupleElementDescriptor : CollectionDescriptor
{
	public int Index { get; }

	public TupleElementDescriptor(int index,
		List<FieldDescriptor> nestedElements = null,
		CollectionDescriptor collectionElementInfo = null,
		List<TupleElementDescriptor> tupleElementsInfo = null) : base(nestedElements, collectionElementInfo, tupleElementsInfo)
	{
		Index = index;
	}
}
