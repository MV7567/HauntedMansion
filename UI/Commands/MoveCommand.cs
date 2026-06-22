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
        private readonly IRenderer _renderer;

        public MoveCommand(string targetRoomId, Map map, Player player, IRenderer renderer)
        {
            _targetRoomId = targetRoomId;
            _map = map;
            _player = player;
            _renderer = renderer;
        }

        public void Execute(CombatContext? context = null)
        {
            var (success, message) = _map.MoveToRoom(_targetRoomId, _player);
            if (!success)
                _renderer.RenderPassageBlocked(message);
        }
    }
}