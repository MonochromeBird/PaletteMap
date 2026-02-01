using FluentAssertions;
using MonochromeBird.Palette;


namespace Tests;


public class PaletteTest {
	[Fact]
	public void IDsAreRegistered() {
		var palette = new Palette<Fruits>();
		var appleId = palette[Fruits.Apple];
		var cherryId = palette[Fruits.Cherry];
		appleId.Should().Be(palette[Fruits.Apple]);
		cherryId.Should().Be(palette[Fruits.Cherry]);
	}

	[Fact]
	public void ItemsHaveCorrectIDs() {
		var palette = new Palette<Fruits>();
		var appleId = palette[Fruits.Apple];
		var cherryId = palette[Fruits.Cherry];
		palette[Fruits.Apple].Should().Be(appleId);
		palette[Fruits.Cherry].Should().Be(cherryId);
	}

	[Fact]
	public void BitWidthIsCorrect() {
		var palette = new Palette<Fruits>();
		_ = palette[Fruits.Apple];
		palette.BitWidth.Should().Be(1);
		_ = palette[Fruits.Cherry];
		palette.BitWidth.Should().Be(2);
		_ = palette[Fruits.Banana];
		palette.BitWidth.Should().Be(2);
	}

	[Fact]
	public void RemovalIsCorrect() {
		var palette = new Palette<Fruits>();
		var appleId = palette[Fruits.Apple];
		var bananaId = palette[Fruits.Banana];
		var cherryId = palette[Fruits.Cherry];
		var map = palette.Remove([appleId]);
		map.Should().ContainKey(bananaId);
		map.Should().ContainKey(cherryId);
		palette[Fruits.Banana].Should().Be(1);
		palette[Fruits.Cherry].Should().Be(2);
	}
}