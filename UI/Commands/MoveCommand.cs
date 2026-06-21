using HauntedMansion.Combat;
using HauntedMansion.Entities;
using HauntedMansion.World;

namespace HauntedMansion.UI.Commands
{
    /// <summary>
    /// player movement
    /// calls Map.MoveToRoom() on Execute()
    /// </summary>
    public class MoveCommand : ICommand
    {
        private readonly string _targetRoomId;
        private readonly Map _map;
        private readonly Player _player;

        public MoveCommand(string targetRoomId, Map map, Player player)
        {
            _targetRoomId = targetRoomId;
            _map = map;
            _player = player;
        }

        public void Execute(CombatContext context = null)
        {
            _map.MoveToRoom(_targetRoomId, _player);
        }
    }
}