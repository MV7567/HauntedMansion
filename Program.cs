using HauntedMansion.Core;
using HauntedMansion.Data;
using HauntedMansion.Entities;
using HauntedMansion.GameLoop;
using HauntedMansion.UI;
using HauntedMansion.World;

// Initialise content loader
var loader = new JsonContentLoader();
loader.LoadAll("content");

// build map
var roomFactory = new RoomFactory(loader);
var map = new Map();

var kitchen = roomFactory.CreateRoom("kitchen");
var childBedroom = roomFactory.CreateRoom("child_bedroom");
var library = roomFactory.CreateRoom("library");
var entranceHall = roomFactory.CreateRoom("entrance_hall");

map.AddRoom(kitchen);
map.AddRoom(childBedroom);
map.AddRoom(library);
map.AddRoom(entranceHall);

// Connect rooms
map.ConnectRooms("entrance_hall", "kitchen");
map.ConnectRooms("entrance_hall", "library");
map.ConnectRooms("kitchen", "child_bedroom");

// Set starting room
map.SetStartingRoom("entrance_hall");

//create player
Console.WriteLine("Enter your name: ");
string name = Console.ReadLine() ?? "Player";

var baseStats = new CharacterStats(
    attack: 15,
    defence: 10,
    magic: 10,
    speed: 12,
    accuracy: 80,
    maxHP: 100);
    
var player = new Player(name, baseStats);

// Start game
var renderer = new TextRenderer();
var gameManager = new GameManager(player, map, renderer);
var startingState = new ExplorationGameState(gameManager);

renderer.RenderMessage("Welcome to Haunted Mansion.");
renderer.RenderMessage($"You wake up in the entrance hall, {name}...");

gameManager.Run(startingState);
    