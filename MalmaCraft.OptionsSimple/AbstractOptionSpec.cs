namespace MalmaCraft.OptionsSimple
{
    public abstract class AbstractOptionSpec<V> : IOptionSpec<V>, IOptionDescriptor
    {
        private readonly List<string> _options = [];
        private readonly string _description = string.Empty;
        private bool _forHelp;

        public AbstractOptionSpec(string description, params string[] options)
        {
            _description = description;
            ArrangeOptions(options);
        }

        public AbstractOptionSpec(string option) : this(string.Empty, option) { }

        public ICollection<string> Options()
        {
            return _options.AsReadOnly();
        }

        public List<V> Values(OptionSet detectedOptions)
        {
            return detectedOptions.
        }

        public V Value(OptionSet detectedOptions)
        {
            throw new NotImplementedException();
        }

        public bool IsForHelp()
        {
            throw new NotImplementedException();
        }

        private void ArrangeOptions(params string[] unarranged)
        {
            if (unarranged.Length == 1)
            {
                _options.AddRange(unarranged);
                return;
            }

            List<string> shortOptions = [];
            List<string> longOptions = [];

            foreach (var each in unarranged)
                if (each.Length == 1)
                    shortOptions.Add(each);
                else
                    longOptions.Add(each);

            shortOptions.Sort();
            longOptions.Sort();

            _options.AddRange(shortOptions);
            _options.AddRange(longOptions);
        }
    }
}
