using HauntedMansion.Data;
using HauntedMansion.GameLoop;
using HauntedMansion.UI;
using HauntedMansion.World;

// Initialise content loader
var loader = new JsonContentLoader();
loader.LoadAll("content");

// Build map from JSON
var roomFactory = new RoomFactory(loader);
var map = roomFactory.BuildMap("entrance_hall");
// lock door
map.LockPassage("entrance_hall", "basement_stairs", Map.PassageBlockReason.RequiresItem, "The heavy door to the basement is locked. It has an old, rusty keyhole.");

// UI & Input 
var renderer = new TextRenderer();
var input = new ConsoleInputProvider();
var intro = new GameIntro(renderer, input);
var (player, startRoomId) = intro.ShowMainMenu(loader);

// Save logic delegate to SaveManager
var saveManager = new SaveManager();
if (saveManager.HasSaveFile() && startRoomId != "entrance_hall")
{
    var (data, msg) = saveManager.LoadGame();
    if (data != null)
    {
        startRoomId = saveManager.ApplySaveData(player, map, loader, data);
    }
}

// Move to starting room
map.SetStartingRoom(startRoomId);
var startingRoom = map.GetCurrentRoom();
if (startingRoom != null)
{
    renderer.RenderMessage(startingRoom.OnEnter(player));
    input.WaitForContinue();
}

// Start game
var gameManager = new GameManager(player, map, renderer, input);
var startingState = new ExplorationGameState(gameManager, loader);
gameManager.Run(startingState);