tool
extends CanvasItem

const param_screen_size = 'screen_size';
const param_screen_scale = 'screen_scale';
const param_uv_scale = 'uv_scale'
const param_opacity = 'opacity'

export var u_scale = 1.0 setget _set_u_scale
export var v_scale = 1.0 setget _set_v_scale
export var screen_scale = 1.0 setget _set_screen_scale
export(float, 0, 1) var opacity = 1.0 setget _set_opacity

func _set_u_scale(new_u_scale): 
	u_scale = new_u_scale; 
	_update()

func _set_v_scale(new_v_scale): 
	v_scale = new_v_scale
	_update()

func _set_screen_scale(new_screen_scale):
	screen_scale = new_screen_scale
	_update()

func _set_opacity(new_opacity):
	opacity = new_opacity
	_update()

func _enter_tree(): _update()
func _init(): _update()

func _update():
	var viewport = get_viewport()

	if viewport != null:
		var size = viewport.size
		var scale = Vector2(u_scale, v_scale)

		material.set_shader_param(param_screen_size, size)
		material.set_shader_param(param_screen_scale, screen_scale)
		material.set_shader_param(param_uv_scale, scale)
		material.set_shader_param(param_opacity, opacity)
