using System;
using ABServicios.BLL.Interfaces;

namespace ABServicios.BLL.Entities
{
    [Serializable]
    public abstract class AbstractEntity<IdT> : IReadOnlyEntity<IdT>
    {
        private IdT id;

        /// <summary>
        /// ID genérico de la entidad
        /// </summary>
        public virtual IdT ID
        {
            get { return id; }
            set { id = value; }
        }
    }

    /// <summary>
    /// Esta clase la agregue porque no tengo idea de cuantos problemas podemos tener si se implementa en AbstractEntity.. y encima hoy es viernes
    /// </summary>
    /// <typeparam name="IdT"></typeparam>
    public abstract class AbstractEquatableEntity<IdT> : AbstractEntity<IdT>
    {
        protected int? requestedHashCode;

        /// <summary>
        /// Compare equality through ID
        /// </summary>
        /// <param name="other">Entity to compare.</param>
        /// <returns>true if are equals</returns>
        /// <remarks>
        /// Two entities are equals if they are of the same hierarchy tree/sub-tree
        /// and has same id.
        /// </remarks>
        public virtual bool Equals(IReadOnlyEntity<IdT> other)
        {
            if (null == other || (!GetType().IsInstanceOfType(other) && !other.GetType().IsInstanceOfType(this)))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            bool otherIsTransient = Equals(other.ID, default(IdT));
            bool thisIsTransient = IsTransient();
            if (otherIsTransient && thisIsTransient)
            {
                return ReferenceEquals(other, this);
            }

            return other.ID.Equals(ID);
        }

        protected virtual bool IsTransient()
        {
            return Equals(ID, default(IdT));
        }

        public override bool Equals(object obj)
        {
            var that = obj as IReadOnlyEntity<IdT>;
            return Equals(that);
        }

        public override int GetHashCode()
        {
            if (!requestedHashCode.HasValue)
            {
                requestedHashCode = IsTransient() ? base.GetHashCode() : ID.GetHashCode();
            }
            return requestedHashCode.Value;
        }

    }
}