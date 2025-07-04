extends AudioStreamPlayer

func _ready():
	# We want our audio stream player to play a random next song
	# as soon as it has finished playing the current song.
	connect("finished", func(): play() )

