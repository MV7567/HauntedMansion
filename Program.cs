using HauntedMansion.Data;
using HauntedMansion.GameLoop;
using HauntedMansion.Inventory.Interfaces;
using HauntedMansion.UI;
using HauntedMansion.World;

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

var saveManager = new SaveManager();
if (saveManager.HasSaveFile() && startRoomId != "entrance_hall")
{
    var (data, msg) = saveManager.LoadGame();
    if (data != null)
    {
        player.TakeDamage(player.CurrentHP - data.CurrentHP);
        player.AddMoney(data.Money - player.Money);
        player.GainExperience(data.Experience - player.Experience);
        startRoomId = data.CurrentRoomId ?? "entrance_hall";
        
        if (data.ClearedRooms != null)
            foreach (var id in data.ClearedRooms) (map.GetRoom(id) as Room)?.ForceClearEnemies();
            
        if (data.LootedRooms != null)
            foreach (var id in data.LootedRooms) (map.GetRoom(id) as Room)?.ForceLootAll();

        var itemFactory = new ItemFactory(loader);
        
        if (data.InventoryIds != null)
            foreach (var itemId in data.InventoryIds)
            {
                var item = itemFactory.CreateItem(itemId);
                if (item != null) player.PlayerInventory.AddItem(item);
            }

        if (data.EquippedIds != null)
            foreach (var kvp in data.EquippedIds)
            {
                var item = itemFactory.CreateItem(kvp.Value) as IEquippable;
                if (item != null) player.Equipment.Equip(item, player);
            }
    }
}

// Move to starting room (or loaded room)
map.SetStartingRoom(startRoomId);

// display starting room
var startingRoom = map.GetCurrentRoom();
if (startingRoom != null)
{
    var startDesc = startingRoom.OnEnter(player);
    renderer.RenderMessage(startDesc);
}

// Start game
var gameManager = new GameManager(player, map, renderer);
var startingState = new ExplorationGameState(gameManager, loader);
gameManager.Run(startingState);
    