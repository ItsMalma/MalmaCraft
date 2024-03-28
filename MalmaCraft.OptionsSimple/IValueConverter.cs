namespace MalmaCraft.OptionsSimple
{
    public interface IValueConverter<V>
    {
        public V Convert(string value);
        public string Revert(V value)
        {
            return value?.ToString() ?? "";
        }
        public Type ValueType();
        public string ValuePattern();
    }
}
