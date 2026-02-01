using FluentAssertions;
using MonochromeBird.Map;
using MonochromeBird.Palette;


namespace Tests;


public class PaletteMapTests {
	[Fact]
	public void CanStoreAndRetrieveValues() {
		var palette = new Palette<Fruits>();
		var map = IndexMap.EnoughFor(16, 8);
		var paletteMap = new PaletteMap<Fruits>(palette, map) {
			[0] = Fruits.Apple,
			[1] = Fruits.Banana,
			[2] = Fruits.Apple,
		};

		paletteMap[0].Should().Be(Fruits.Apple);
		paletteMap[1].Should().Be(Fruits.Banana);
		paletteMap[2].Should().Be(Fruits.Apple);
	}

	[Fact]
	public void SameValuesSharePaletteId() {
		var palette = new Palette<Fruits>();
		var map = IndexMap.EnoughFor(16, 8);
		var paletteMap = new PaletteMap<Fruits>(palette, map);

		paletteMap[0] = Fruits.Apple;
		paletteMap[1] = Fruits.Apple;
		paletteMap[2] = Fruits.Banana;

		palette.Count.Should().Be(2);
		map[0].Should().Be(map[1]);
	}

	[Fact]
	public void UnsetPositionsReturnNull() {
		var palette = new Palette<Fruits>();
		var map = IndexMap.EnoughFor(16, 8);
		var paletteMap = new PaletteMap<Fruits>(palette, map);
		paletteMap[5].Should().Be(Fruits.None);
	}

	[Fact]
	public void CompactRemovesUnusedPaletteEntries() {
		var palette = new Palette<Fruits>();
		var map = IndexMap.EnoughFor(8, 8);
		var paletteMap = new PaletteMap<Fruits>(palette, map) {
			[0] = Fruits.Apple,
			[1] = Fruits.Banana,
			[2] = Fruits.Orange,
			[3] = Fruits.Mango,
		};

		palette.Count.Should().Be(4);

		// Overwrite with existing values, making some palette entries unused
		paletteMap[1] = Fruits.Apple;
		paletteMap[3] = Fruits.Apple;

		paletteMap.Compact();

		palette.Count.Should().Be(2); // Only Apple and Orange remain
		paletteMap[0].Should().Be(Fruits.Apple);
		paletteMap[1].Should().Be(Fruits.Apple);
		paletteMap[2].Should().Be(Fruits.Orange);
		paletteMap[3].Should().Be(Fruits.Apple);
	}
}