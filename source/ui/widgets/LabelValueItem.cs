using Godot;
using static System.Diagnostics.Debug;

namespace TribesOfDust.UI
{
    public class LabelValueItem : Control
    {
        [Export(PropertyHint.Range, "0,1,0.05")]
        public float ActiveVisibility;

        public override void _EnterTree()
        {
            _label = GetNode<Label>(LabelPath);
            _value = GetNode<Label>(ValuePath);

            Assert(_label is not null);
            Assert(_value is not null);

            base._EnterTree();
        }

        public override void _Ready()
        {
            // Initialize self modulate to be invisible

            _activeColor = new Color(SelfModulate, ActiveVisibility);
            _inactiveColor = new Color(SelfModulate, 0.0f);

            SelfModulate = _inactiveColor;

            base._Ready();
        }

        public string Label
        {
            get => _label.Text;
            set => _label.Text = value;
        }

        public string Value
        {
            get => _value.Text;
            set => _value.Text = value;
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                SelfModulate = _isActive
                    ? _activeColor
                    : _inactiveColor;
            }
        }

        private Label _label = null!;
        private Label _value = null!;

        private bool _isActive = false;

        private Color _activeColor;
        private Color _inactiveColor;

        private const string LabelPath = "Line/Label";
        private const string ValuePath = "Line/Value";
    }
}