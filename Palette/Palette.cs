using System.Collections;
using ProtoBuf;


namespace MonochromeBird.Palette;


public readonly record struct PaletteItem<T>(uint Id, T Item) where T : notnull;


[ProtoContract]
public sealed record Palette<T>(
	[property: ProtoMember(1)] Dictionary<uint, T> Items,
	[property: ProtoMember(2)] Dictionary<T, uint> IDs
) : IEnumerable<PaletteItem<T>> where T : notnull {
	public Palette() : this([], []) {
	}


	public byte BitWidth => (byte)(Math.Log(HighestID, 2) + 1);
	public uint Count => (uint)Items.Count;
	private uint HighestID;


	public T? this[uint id] => Items.GetValueOrDefault(id);


	public uint this[T? item] {
		get {
			if (item is null) return 0;
			if (IDs.TryGetValue(item, out var id)) return id;
			HighestID = Count + 1u;
			Items[HighestID] = item;
			IDs[item] = HighestID;
			return HighestID;
		}
	}


	public Dictionary<uint, uint> Remove(uint[] idsToRemove) {
		var map = new Dictionary<uint, uint>();
		foreach (var removeId in idsToRemove) {
			if (Items.Remove(removeId, out var item)) {
				IDs.Remove(item);
			}
		}

		var lookup = new HashSet<uint>(idsToRemove);
		var (oldItems, newId) = (new Dictionary<uint, T>(Items), 1u);
		Items.Clear();
		IDs.Clear();
		foreach (var (oldId, item) in oldItems) {
			if (lookup.Contains(oldId)) continue;
			map[oldId] = newId;
			Items.Add(newId, item);
			IDs.Add(item, newId);
			newId++;
		}

		HighestID = newId != 0 ? newId - 1 : 0;
		return map;
	}


	public IEnumerator<PaletteItem<T>> GetEnumerator() {
		foreach (var (id, item) in Items) {
			yield return new PaletteItem<T>(id, item);
		}
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
	
	
	public override string ToString() {
		return '[' + string.Join(' ', this.Select(static x => x.ToString())) + ']';
	}
}