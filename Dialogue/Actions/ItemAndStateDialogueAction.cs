using HauntedMansion.Combat;
using HauntedMansion.Combat.Interfaces;
using HauntedMansion.Entities;
using System.Linq;

namespace HauntedMansion.Dialogue.Actions
{
    /// <summary>
    /// Chnages enemy state and takes away the appropriate item
    /// </summary>
    public class ItemAndStateDialogueAction : IDialogueAction
    {
        private readonly Enemy _enemy;
        private readonly ICombatState _newState;
        private readonly string _message;
        private readonly Player _player;
        private readonly string _keyId;
        
        public ItemAndStateDialogueAction(Enemy enemy, ICombatState newState, string keyId, Player player, string message = "")
        {
            _enemy = enemy;
            _newState = newState;
            _keyId = keyId;
            _player = player;
            _message = message;
        }
        
        public string Execute(CombatContext? context)
        {
            _enemy.SetState(_newState);
            
            var item = _player.PlayerInventory.GetKeyItem().FirstOrDefault(k => k.KeyID == _keyId);
            if (item != null)
            {
                _player.PlayerInventory.RemoveItem(item);
            }
            
            return _message;
        }

    }
}