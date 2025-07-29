using System.Collections.Generic;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Throw
{
    /// <summary>
    /// Handles the assignment of throw spots to players, ensuring that each player
    /// receives a unique throw spot that is not currently assigned to another player.
    /// </summary>

    public class ThrowSpotHandler : MonoBehaviour
    {
        [SerializeField] private List<ThrowSpot> throwSpots;
    
        private readonly Dictionary<PlayerNumber, int> _assignedSpots = new();

        public ThrowSpot GetNextThrowSpot(PlayerNumber player)
        {
            _assignedSpots.Remove(player);
        
            List<int> availableIndices = new();
            for (int i = 0; i < throwSpots.Count; i++)
            {
                if (!_assignedSpots.ContainsValue(i))
                {
                    availableIndices.Add(i);
                }
            }
        
            int chosenIndex = availableIndices[Random.Range(0, availableIndices.Count)];
            _assignedSpots[player] = chosenIndex;
            return throwSpots[chosenIndex];
        }
    }
}
