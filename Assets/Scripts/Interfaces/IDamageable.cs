namespace BunnyGame.Interfaces
{
    /// <summary>
    /// Interfaz para entidades que pueden recibir daño y morir
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Llamado cuando la entidad debe morir
        /// </summary>
        void Die();

        /// <summary>
        /// Devuelve si la entidad está actualmente muerta
        /// </summary>
        bool IsDead { get; }
    }
}
