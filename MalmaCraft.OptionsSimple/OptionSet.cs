namespace MalmaCraft.OptionsSimple
{
    public class OptionSet(Dictionary<string, AbstractOptionSpec<object>> recognizedSpecs)
    {
        private readonly List<IOptionSpec<object>> _detectedSpecs = [];
        private readonly Dictionary<string, AbstractOptionSpec<object>> _detectedOptions = [];
        private readonly Dictionary<AbstractOptionSpec<object>, List<string>> _optionsToArguments = [];
        private readonly Dictionary<string, List<object>> _defaultValues = DefaultValues(recognizedSpecs);
        private readonly Dictionary<string, AbstractOptionSpec<object>> _recognizedSpecs = recognizedSpecs;

        public object? ValueOf(string option)
        {
            var spec = _detectedOptions[option];
            if (spec == null)
            {
                var defaults = DefaultValuesFor<object>(option);
                return defaults.Count < 1 ? null : defaults[0];
            }
            return ValueOf(spec);
        }

        public V? ValueOf<V>(IOptionSpec<V> option)
        {
            var values = ValuesOf(option);
            switch (values.Count)
            {
                case 0:
                    return default(V);
                case 1:
                    return values[0];
                default:
                    throw new 
            }
        }

        public List<object> ValuesOf(string option)
        {
            var spec = _detectedOptions[option];
            return spec == null ? DefaultValuesFor<object>(option) : ValuesOf(spec);
        }

        public IList<V> ValuesOf<V>(IOptionSpec<V> option)
        {
            var values = _optionsToArguments[(AbstractOptionSpec<object>)option];
            if (values == null || values.Count < 1)
                return DefaultValuesFor(option);
            
            AbstractOptionSpec<V> spec = (AbstractOptionSpec<V>)option;
            List<V> convertedValues = [];
            foreach (var each in values)
                convertedValues.Add(spec.Convert(each));

            return convertedValues.AsReadOnly();
        }

        private IList<V> DefaultValuesFor<V>(string option)
        {
            if (_defaultValues.TryGetValue(option, out var defaultValue))
            {
                List<V> result = [];
                foreach (var each in defaultValue)
                    result.Add((V)each);
                return result;
            }
            return [];
        }

        private IList<V> DefaultValuesFor<V>(IOptionSpec<V> option)
        {
            option.Options().GetEnumerator().MoveNext();
            return DefaultValuesFor<V>(option.Options().GetEnumerator().Current);
        }

        private static Dictionary<string, List<object>> DefaultValues(Dictionary<string, AbstractOptionSpec<object>> recognizedSpecs)
        {
            Dictionary<string, List<object>> defaults = [];
            foreach (var each in recognizedSpecs)
            {
                defaults[each.Key, each.Value.DefaultValues()];
            }
            return defaults;
        }
    }
}
