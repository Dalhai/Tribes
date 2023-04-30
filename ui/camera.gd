extends Camera2D

@export_range(0, 1, 0.005)           var zoom_center_influence = 0.25
@export_range(0, 1, 0.005)           var zoom_max = 10.0 
@export_range(10.0, 1000.0, 1.0)     var zoom_speed = 100.0
@export_range(100.0, 100000.0, 10.0) var movement_max = 1000.0

@onready var viewport = get_viewport()

# Called when the node enters the scene tree for the first time.
func _ready():
	anchor_mode = Camera2D.ANCHOR_MODE_FIXED_TOP_LEFT
	zoom = Vector2(
		min(zoom_max, zoom.x), 
		min(zoom_max, zoom.y)
	)
	
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if (Input.is_action_pressed("cam_up")):
		position = position.move_toward(position + Vector2.UP, delta * movement_max / zoom.y)
	if (Input.is_action_pressed("cam_down")):
		position = position.move_toward(position + Vector2.DOWN, delta * movement_max / zoom.y)
	if (Input.is_action_pressed("cam_left")):
		position = position.move_toward(position + Vector2.LEFT, delta * movement_max / zoom.x)
	if (Input.is_action_pressed("cam_right")):
		position = position.move_toward(position + Vector2.RIGHT, delta * movement_max / zoom.x)
		
	force_update_scroll()
		
	var screen_center = get_screen_center_position()
	var mouse_pos = get_global_mouse_position()
	
	if (Input.is_action_just_released("zoom_in")):
		zoom_stable(mouse_pos, -zoom_speed * delta)
	if (Input.is_action_pressed("zoom_in")):
		zoom_stable(screen_center, -zoom_speed * delta / 100.0)
		
	if (Input.is_action_just_released("zoom_out")):
		zoom_stable(screen_center, zoom_speed * delta)
	if (Input.is_action_pressed("zoom_out")):
		zoom_stable(screen_center, zoom_speed * delta / 100.0)
		
func zoom_stable(anchor: Vector2, amount: float):
		var old_zoom: Vector2 = zoom
		var new_zoom: Vector2 = old_zoom * (1.0 - amount)
		
		new_zoom.x = max(0.0, min(new_zoom.x, zoom_max))
		new_zoom.y = max(0.0, min(new_zoom.y, zoom_max))
		
		var canvas_anchor = get_canvas_transform() * anchor
		var window_anchor = canvas_anchor / Vector2(viewport.size)
		zoom = new_zoom
		
		# Convert the window anchor back to a global position
		canvas_anchor = window_anchor * Vector2(viewport.size)
		
		var target = get_canvas_transform().affine_inverse() * canvas_anchor
		var target_offset = target - anchor
		
		position -= target_offset
		force_update_scroll()
