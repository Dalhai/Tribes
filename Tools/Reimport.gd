# Save this as, say, `res://tools/fix_uids.gd`, then right-click ▶ Run
@tool
extends EditorScript

var files: Array[String] = []

func _run() -> void:
	files.clear()
	_collect_files("res://")
	for file in files:
		print("⟳ Fixing UID in:", file)
		var res = ResourceLoader.load(file)
		ResourceSaver.save(res)

func _collect_files(dir_path: String) -> void:
	var dir = DirAccess.open(dir_path)
	for file_name in dir.get_files():
		var ext = file_name.get_extension().to_lower()
		if ext in ["tscn", "tres", "material", "png"]:
			files.append(dir_path + "/" + file_name)
	for child in dir.get_directories():
		_collect_files(dir_path + "/" + child)
