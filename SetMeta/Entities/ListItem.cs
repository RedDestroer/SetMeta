﻿namespace SetMeta.Entities
{
    /// <summary>
    /// Элемент списка, для настроек типа фиксированного списка
    /// </summary>
    public class ListItem
    {
        public object Value { get; set; }
        public string DisplayValue { get; set; }

        public static bool operator ==(ListItem left, ListItem right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (((object)left == null) || ((object)right == null))
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(ListItem left, ListItem right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            var other = obj as ListItem;
            if (other == null)
                return false;

            if (!Equals(Value, other.Value))
                return false;

            if (DisplayValue != other.DisplayValue)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash += (31 * hash) + (Value == null ? (31 * hash) : Value.GetHashCode());
                hash += (31 * hash) + (DisplayValue == null ? (31 * hash) : DisplayValue.GetHashCode());

                return hash;
            }
        }
    }
}