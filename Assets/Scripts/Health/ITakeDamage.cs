namespace Health
{
    public interface ITakeDamage
    {
        /// <summary>
        /// Makes the object/entity take damage. Implemented internally
        /// by each entity.
        /// </summary>
        public bool TryTakeDamage(int quantity);

        /// <summary>
        /// Makes the object/entity take damage, if the entity is not avoiding.
        /// </summary>
        public void TryTakeAvoidableDamage(int damage);
    }
}
