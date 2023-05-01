extends NinePatchRect

func _ready():
	var root = get_tree().root
	root.size_changed.connect(func():
		var viewport_size = root.size
		var shader_material = material as ShaderMaterial
		
		if shader_material != null:
			shader_material.set_shader_parameter("vp_size", viewport_size)
	)
