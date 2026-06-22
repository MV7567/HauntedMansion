using HauntedMansion.Data;
using HauntedMansion.GameLoop;
using HauntedMansion.UI;

// Initialise content loader
var loader = new JsonContentLoader();
loader.LoadAll("content");

// Build map from JSON
var roomFactory = new RoomFactory(loader);
var map = roomFactory.BuildMap("entrance_hall");

// UI - main menu handles new game / load game
var renderer = new TextRenderer();
var intro = new GameIntro(renderer);
var (player, startRoomId) = intro.ShowMainMenu(loader);

// Move to starting room (or loaded room)
map.SetStartingRoom(startRoomId);

// Start game
var gameManager = new GameManager(player, map, renderer);
var startingState = new ExplorationGameState(gameManager, loader);
gameManager.Run(startingState);
    