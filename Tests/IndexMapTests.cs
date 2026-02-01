using FluentAssertions;
using MonochromeBird.Map;
using MonochromeBird.Palette;


namespace Tests;


public class IndexMapTests {
	[Fact]
	public void MapIsCorrectlyInitialized() {
		var map = IndexMap.EnoughFor(8, 4);
		map.BitWidth.Should().Be(3);
		map.SlotCount.Should().Be(8);
		map.BitCount.Should().Be(8 * 3);
		map.WordCount.Should().Be(1);
	}

	[Fact]
	public void IndexAreCorrectlyStored() {
		var map = IndexMap.EnoughFor(8, 10);
		map[4] = 10;
		map[1] = 5;
		map[2] = 3;
		map[3] = 5;
		map[4].Should().Be(10);
		map[5].Should().Be(0);
		map[2].Should().Be(3);
		map[3].Should().Be(5);
		map[1].Should().Be(5);
	}

	[Fact]
	public void ReEncodeIsCorrect() {
		var map = new IndexMap(8, 8);
		map.BitWidth.Should().Be(8);
		map[7] = 4;
		map.ReEncode(3);
		map[7].Should().Be(4);
		map.SlotCount.Should().Be(8);
		map.BitWidth.Should().Be(3);
	}
}