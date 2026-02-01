using System.Collections;
using MonochromeBird.Palette;
using ProtoBuf;


namespace MonochromeBird.Map;


public readonly record struct PaletteMapItem<T>(uint Position, T? Value) where T : notnull;


[ProtoContract(SkipConstructor = true)]
public record PaletteMap<T>(
	[property: ProtoMember(1)] Palette<T> Palette,
	[property: ProtoMember(2)] IndexMap Map
) : IEnumerable<PaletteMapItem<T>> where T : notnull {
	public uint ItemCount => Palette?.Count ?? 0;
	public uint SlotCount => Map?.SlotCount ?? 0;
	public byte BitWidth => Map?.BitWidth ?? 0;


	public PaletteMap(uint slotCount, byte bitWidth = 6)
		: this(new Palette<T>(), new IndexMap(slotCount, bitWidth)) {
	}


	public T? this[uint position] {
		set => Map[position] = Palette[value];
		get => Palette[Map[position]];
	}


	private uint[] GetUnusedPaletteIds() {
		var used = new HashSet<uint>();
		foreach (var (_, value) in Map) {
			used.Add(value);
		}
		var (unused, idx) = (new uint[Palette.Count - used.Count + 1], 0);
		foreach (var (id, _) in Palette) {
			if (!used.Contains(id)) {
				unused[idx++] = id;
			}
		}
		return unused;
	}


	public void Compact() {
		Map.ReEncode(Palette.Remove(GetUnusedPaletteIds()), Palette.BitWidth);
	}


	public IEnumerator<PaletteMapItem<T>> GetEnumerator() {
		foreach (var (position, id) in Map) {
			yield return new(position, Palette[id]);
		}
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}


	public override string ToString() {
		if (SlotCount <= 256) {
			return '[' + string.Join(' ', this.Select(static x => x.Value?.ToString() ?? default(T)?.ToString())) + ']';
		}
		return base.ToString() ?? string.Empty;
	}
}