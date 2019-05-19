extends Node

onready var cryptGenerator = load("res://crypt/crypt_generator/crypt_generator.tscn")

###################
# Godot Functions #
###################

var shouldGenerate = false

func _process(delta):
	if Input.is_action_pressed("pause_0") or Input.is_action_pressed("pause_1"):
		quit_to_main_menu()
	if Input.is_action_pressed("reset_0") or Input.is_action_pressed("reset_1"):
		create_crypt()

func _ready():
	set_process(true)
	create_crypt()
	change_music()

####################
# Helper Functions #
####################

func change_music(streamPath = null, volume = -20):
	if streamPath == null:
		streamPath = "res://assets/audio/bgm/passive_crypt_" + str(randi() % 2) + ".ogg"
	$AudioStreamPlayer.stream = load(streamPath)
	$AudioStreamPlayer.volume_db = -20
	$AudioStreamPlayer.play()

func cleanup():
	var cryptGeneratorNodes = get_tree().get_nodes_in_group("crypt_generator")
	for cryptGeneratorNode in cryptGeneratorNodes:
		cryptGeneratorNode.destroy()

func create_crypt():
	var cryptGeneratorNode = null
	var cryptGeneratorNodes = get_tree().get_nodes_in_group("crypt_generator")
	if len(cryptGeneratorNodes) != 0:
		cryptGeneratorNode = cryptGeneratorNodes[0]
	else:
		cryptGeneratorNode = cryptGenerator.instance()
		add_child(cryptGeneratorNode)
	cryptGeneratorNode.generate_crypt()

func exit_to_village():
	cleanup()
	crypt_globals.cryptSeed = null
	get_tree().change_scene("res://village/village.tscn")

func quit_to_main_menu():
	cleanup()
	get_tree().change_scene("res://ui/main_menu/main_menu.tscn")