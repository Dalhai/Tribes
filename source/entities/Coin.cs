using System.Diagnostics;

using Godot;

using TribesOfDust.Utils.Godot;

namespace TribesOfDust.Entities
{
    public class Coin : Node2D
    {
        #region Constructors

        public Coin(Texture coinTexture, Texture symbolTexture)
            : this(Vector2.One, coinTexture, symbolTexture)
        {
        }

        public Coin(Vector2 scale, Texture coinTexture, Texture symbolTexture)
        {
            // Check that the scale is not zero.

            GodotAssertions.AssertScaleValid(scale);
            GodotAssertions.AssertTextureValid(coinTexture);
            GodotAssertions.AssertTextureValid(symbolTexture);

            // Setup the root node properties

            Scale = scale;

            // Setup the sprite properties

            Vector2 coinSpriteScale = scale / coinTexture.GetWidth();
            Vector2 symbolSpriteScale = scale / 2.0f / symbolTexture.GetWidth();

            // Setup the coin sprite properties and attach it to the coin root node.

            _coinSprite = new Sprite()
            {
                Texture  = coinTexture,
                Centered = true,
                Scale    = coinSpriteScale
            };

            AddChild(_coinSprite);

            // Setup the symbol sprite properties and attach it to the coin root node.
            // Note that the symbol sprite is set to half the size of the coin sprite, hoping that
            // like this it will fit within the coin sprite. This is not very robust, of course and
            // should eventually be adapted.

            _symbolSprite = new Sprite()
            {
                Texture  = symbolTexture,
                Centered = true,
                Scale    = symbolSpriteScale
            };

            AddChild(_symbolSprite);
        }

        #endregion

        private readonly Sprite _coinSprite;
        private readonly Sprite _symbolSprite;
    }
}