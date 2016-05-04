using System;

namespace SimpleGraph
{
    //CREATED BY ANDREY SOKOLOV
    //DATE: 15.03.2016

    /// <summary>
    /// Связь элементов
    /// </summary>
    /// <typeparam name="T">Тип элементов для которых устанавливается связь</typeparam>
    public class Link<T> : IEquatable<Link<T>>
    {
        /// <summary>
        /// Родительский элемент связи
        /// </summary>
        public T Parent;
        /// <summary>
        /// Дочерний элемент связи
        /// </summary>
        public T Child;
        /// <summary>
        /// Создать новую связь
        /// </summary>
        /// <param name="Parent">Родительский элемент связи</param>
        /// <param name="Child">Дочерний элемент связи</param>
        public Link(T Parent, T Child)
        {
            this.Parent = Parent;
            this.Child = Child;
        }

        public static bool operator !=(Link<T> a, Link<T> b)
        {
            return !(a == b);
        }

        public static bool operator ==(Link<T> a, Link<T> b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (((object)a == null) || ((object)b == null))
                return false;
            return a.Parent.Equals(b.Parent) && a.Child.Equals(b.Child);
        }

        public bool Equals(Link<T> other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (GetType() != other.GetType())
                return false;
            if (Parent.Equals(other.Parent) && Child.Equals(other.Child))
                return true;
            else
                return false;
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return false;
            if (GetType() != other.GetType())
                return false;
            return Equals(other as Link<T>);
        }

        public override int GetHashCode()
        {
            return Child.GetHashCode() ^ Parent.GetHashCode();
        }
    }
}
