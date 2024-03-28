namespace MalmaCraft.OptionsSimple
{
    public interface IOptionSpec<V>
    {
        public List<V> Values(OptionSet detectedOptions);
        public V Value(OptionSet detectedOptions);
        public V? ValueOptional(OptionSet detectedOptions)
        {
            return Value(detectedOptions);
        }
        public ICollection<string> Options();
        public bool IsForHelp();
    }
}
