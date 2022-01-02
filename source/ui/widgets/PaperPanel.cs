using Godot;

using TribesOfDust.Utils;

namespace TribesOfDust.UI.Widgets
{
    [Tool]
    public class PaperPanel : NinePatchRect
    {
        public override void _Ready()
        {
            // Register with the viewport to update necessary shader parameters.
            GetTree().Root.Connect("size_changed", this, "OnSizeChanged");

            // Trigger signal handler immediately for initialization of variables.
            OnSizeChanged();
            
            base._Ready();
        }

        private void OnSizeChanged() 
        {
            var viewportSize = GetTree().Root.Size;
            var shaderMaterial = Material as ShaderMaterial;

            if (shaderMaterial is not null)
            {
                shaderMaterial.SetShaderParam(ShaderParams.ViewportSize, viewportSize);
            }
        }
    }
}