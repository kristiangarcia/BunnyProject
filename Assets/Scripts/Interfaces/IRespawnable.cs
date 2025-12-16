using UnityEngine;

namespace BunnyGame.Interfaces
{
    /// <summary>
    /// Interfaz para entidades que pueden reaparecer
    /// </summary>
    public interface IRespawnable
    {
        /// <summary>
        /// Reaparece la entidad en su posición original
        /// </summary>
        void Respawn();

        /// <summary>
        /// Obtiene la posición original de spawn de la entidad
        /// </summary>
        Vector3 SpawnPosition { get; }
    }
}
