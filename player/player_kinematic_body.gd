extends KinematicBody2D

onready var playerLightScene = load("res://player/player_light/player_light.tscn")
onready var debugInfoScene = load("res://player/debug_info/debug_info.tscn")

var playerIndex = 0
var player = player_globals.players[self.playerIndex]

var PLAYER_SPRINT = "sprint_" + str(self.playerIndex)

func _ready():
	set_physics_process(true)
	create_light(self.playerIndex)
	create_debug_info()

func _physics_process(delta):
	get_input()
	move_and_slide(player.velocity * delta)

func _process(delta):
	player.timeElapsed = OS.get_unix_time() - player.timeStart
	if player.timeElapsed > 0:
		player.lightNode.update_size(player.timeElapsed)

func create_light(playerIndex):
	var playerLight = player_globals.players[playerIndex].lightNode
	if playerLight == null:
		playerLight = playerLightScene.instance()
		add_child(playerLight)
		player_globals.players[playerIndex].lightNode = playerLight
		print("Creating light: ", playerIndex)

func create_debug_info():
	var debugInfo = player_globals.players[0].debugInfo
	if debugInfo == null:
		debugInfo = debugInfoScene.instance()
		add_child(debugInfo)
		player_globals.players[0].debugInfo = debugInfo

func destroy():
	queue_free()

func get_input():
	if Input.is_action_just_pressed(PLAYER_SPRINT):
		player.isSprinting = true
	if Input.is_action_just_released(PLAYER_SPRINT):
		player.isSprinting = false
	var speed = player.sprintingSpeed if player.isSprinting else player.walkingSpeed
	
	if Input.is_action_pressed(player.up.inputName):
		player.velocity += player.up.vector * Input.get_action_strength(player.up.inputName) * speed
	if Input.is_action_pressed(player.down.inputName):
		player.velocity += player.down.vector * Input.get_action_strength(player.down.inputName) * speed
	if Input.is_action_pressed(player.left.inputName):
		player.velocity += player.left.vector * Input.get_action_strength(player.left.inputName) * speed
	if Input.is_action_pressed(player.right.inputName):
		player.velocity += player.right.vector * Input.get_action_strength(player.right.inputName) * speed

	player.velocity *= player.friction

func set_player_index(newPlayerIndex):
	var tempTimeStart = player.timeStart
	var tempLightNode = player.lightNode
	var tempDebugInfo = player.debugInfo
	var tempInstance = player.instance
	if tempTimeStart == null:
		tempTimeStart = OS.get_unix_time()

	self.playerIndex = newPlayerIndex
	player = player_globals.players[newPlayerIndex]
	PLAYER_SPRINT = "sprint_" + str(newPlayerIndex)
	player.timeStart = tempTimeStart
	player.lightNode = tempLightNode
	player.debugInfo = tempDebugInfo
	player.instance = tempInstance