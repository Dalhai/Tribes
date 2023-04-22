using Godot;

namespace TribesOfDust.UI.Widgets
{
    public partial class HoverButton : Label
    {
        [Signal]
        public delegate void PressedEventHandler();

        /// <summary>
        ///  The item to be modulated, if any.
        ///  If left unassigned, no item will be modulated.
        /// </summary>
        [Export] 
        public NodePath? ModulationColored;

        /// <summary>
        ///  The item to have its' font colored, if any.
        ///  If left unassigned, no item will have its' font color adjusted.
        ///  If assigend to anything but a label, no item will have its' font color adjusted.
        /// </summary>
        [Export]
        public NodePath? FontColored;

        [Export]
        public Color HoverColor
        {
            get => _hoverColor;
            set => _hoverColor = value;
        }

        public bool IsHovered => _isHovered;

        public override void _EnterTree()
        {   
            base._EnterTree();

            _modulationColorTarget = GetNodeOrNull<Control>(ModulationColored);
            _fontColorTarget = GetNodeOrNull<Label>(FontColored);

            if (_modulationColorTarget is not null)
                _modulationColor = _modulationColorTarget.Modulate;
        }

        public override void _GuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouse && mouse.IsPressed()) 
            {
                EmitSignal(SignalName.Pressed);
            }

            base._GuiInput(@event);
        }

        public void OnMouseEntered() 
        {
            if (_modulationColorTarget is not null)
                _modulationColorTarget.Modulate = HoverColor;

            if (_fontColorTarget is not null)
                _fontColorTarget.AddThemeColorOverride("font_color", HoverColor);

            _isHovered = true;
        }

        public void OnMouseExited()
        {
            if (_modulationColorTarget is not null)
                _modulationColorTarget.Modulate = _modulationColor;

            if (_fontColorTarget is not null)
                _fontColorTarget.AddThemeColorOverride("font_color", _fontColor);

            _isHovered = false;
        }

        private Color _hoverColor = Colors.Orange;
        private Color _modulationColor = Colors.White;
        private Color _fontColor = Colors.White;

        private Label? _fontColorTarget;
        private Control? _modulationColorTarget;

        private bool _isHovered = false;
    }
}