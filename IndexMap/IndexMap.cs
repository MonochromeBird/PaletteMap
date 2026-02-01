using System.Collections;
using ProtoBuf;


namespace MonochromeBird.Map;


public readonly record struct IndexMapItem(uint Position, uint Value);


[ProtoContract]
public record IndexMap : IEnumerable<IndexMapItem> {
	[ProtoMember(1)] public uint SlotCount { get; private set; }
	[ProtoMember(2)] public byte BitWidth { get; private set; }
	[ProtoMember(3)] private uint[] Words;


	public ulong BitCount => GetBitCount(SlotCount, BitWidth);
	public uint WordCount => GetWordCount(BitCount);


	public IndexMap(uint slotCount, byte bitWidth) {
		SlotCount = slotCount;
		BitWidth = bitWidth;
		Words = new uint[WordCount];
	}


	public IndexMap() {
		Words = [];
	}


	public static IndexMap EnoughFor(uint slotCount, uint maxIndex) {
		return new IndexMap(slotCount, (byte)(Math.Log(maxIndex + 1, 2) + 1));
	}


	public uint this[uint position] {
		get => Read(Words, BitWidth, position);
		set => Write(Words, BitWidth, position, value);
	}


	public void ReEncode(byte bitWidth, uint? slotCount = null) {
		var slots = slotCount ?? SlotCount;
		var newWords = new uint[GetWordCount(GetBitCount(slots, bitWidth))];
		var copyCount = Math.Min(SlotCount, slots);

		for (uint position = 0; position < copyCount; position++) {
			var value = Read(Words, BitWidth, position);
			Write(newWords, bitWidth, position, value);
		}

		Words = newWords;
		SlotCount = slots;
		BitWidth = bitWidth;
	}


	public void ReEncode(Dictionary<uint, uint> oldToNew, byte bitWidth, uint? slotCount = null) {
		var slots = slotCount ?? SlotCount;
		var newWords = new uint[GetWordCount(GetBitCount(slots, bitWidth))];
		var copyCount = Math.Min(SlotCount, slots);

		for (uint position = 0; position < copyCount; position++) {
			var value = Read(Words, BitWidth, position);
			Write(newWords, bitWidth, position, value == 0 ? 0 : oldToNew[value]);
		}

		Words = newWords;
		SlotCount = slots;
		BitWidth = bitWidth;
	}


	private static uint GetWordCount(ulong bitCount) {
		return (uint)((bitCount + 31) / 32);
	}

	private static ulong GetBitCount(uint slotCount, byte bitWidth) {
		return slotCount * bitWidth;
	}

	private static uint Read(uint[] words, byte bitWidth, uint position) {
		var bitPosition = (ulong)position * bitWidth;
		var wordIndex = (uint)(bitPosition / 32);
		var bitOffset = (int)(bitPosition % 32);

		var mask = (uint)((1UL << bitWidth) - 1);

		if (bitOffset + bitWidth <= 32) {
			return (words[wordIndex] >> bitOffset) & mask;
		}

		// Value spans two words
		var bitsInFirstWord = 32 - bitOffset;
		var lowerBits = words[wordIndex] >> bitOffset;
		var upperBits = words[wordIndex + 1] & ((1u << (bitWidth - bitsInFirstWord)) - 1);
		return lowerBits | (upperBits << bitsInFirstWord);
	}

	private static void Write(uint[] words, byte bitWidth, uint position, uint value) {
		var bitPosition = (ulong)position * bitWidth;
		var wordIndex = (uint)(bitPosition / 32);
		var bitOffset = (int)(bitPosition % 32);

		var mask = (uint)((1UL << bitWidth) - 1);
		value &= mask;

		if (bitOffset + bitWidth <= 32) {
			words[wordIndex] = (words[wordIndex] & ~(mask << bitOffset)) | (value << bitOffset);
			return;
		}

		// Value spans two words
		var bitsInFirstWord = 32 - bitOffset;
		var bitsInSecondWord = bitWidth - bitsInFirstWord;

		var lowerMask = (1u << bitsInFirstWord) - 1;
		words[wordIndex] = (words[wordIndex] & ~(lowerMask << bitOffset)) | ((value & lowerMask) << bitOffset);

		var upperMask = (1u << bitsInSecondWord) - 1;
		words[wordIndex + 1] = (words[wordIndex + 1] & ~upperMask) | ((value >> bitsInFirstWord) & upperMask);
	}

	public IEnumerator<IndexMapItem> GetEnumerator() {
		for (var position = 0u; position < SlotCount; position++) {
			yield return new(position, this[position]);
		}
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}


	public override string ToString() {
		if (SlotCount <= 256) {
			return '[' + string.Join(' ', this.Select(static x => x.Value.ToString())) + ']';
		}
		return base.ToString() ?? string.Empty;
	}
}