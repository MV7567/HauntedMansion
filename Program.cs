using HauntedMansion.Core;
using HauntedMansion.Data;
using HauntedMansion.Entities;
using HauntedMansion.GameLoop;
using HauntedMansion.UI;
using HauntedMansion.World;

// Initialise content loader
var loader = new JsonContentLoader();
loader.LoadAll("content");

// Build map - reads everything from JSON automatically
var roomFactory = new RoomFactory(loader);
var map = roomFactory.BuildMap("entrance_hall");

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
var startingState = new ExplorationGameState(gameManager, loader);

renderer.RenderMessage("Welcome to Haunted Mansion.");
renderer.RenderMessage($"You wake up in the entrance hall, {name}...");

gameManager.Run(startingState);
    