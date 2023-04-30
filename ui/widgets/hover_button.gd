extends Label

signal pressed

@onready var button_text: Label = self
@onready var button_border: NinePatchRect = $Border as NinePatchRect

func _ready():
	connect("mouse_entered", on_mouse_entered)
	connect("mouse_exited", on_mouse_exited)
	connect("gui_input", on_gui_input)
	
func on_mouse_entered():
	button_text.add_theme_color_override("font_color", Color.BLUE)
	
func on_mouse_exited():
	button_text.add_theme_color_override("font_color", Color.BLACK)
	
func on_gui_input(event: InputEvent):
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT:
			pressed.emit()
