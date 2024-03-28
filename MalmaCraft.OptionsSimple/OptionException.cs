using System.Text;

namespace MalmaCraft.OptionsSimple
{
    public abstract class OptionException : Exception
    {
        private readonly List<string> _options = [];
        public IList<string> Options => _options.AsReadOnly();

        protected OptionException(params string[] options)
        {
            _options.AddRange(options);
        }

        protected OptionException(ICollection<IOptionSpec<object>> options)
        {
            _options.AddRange(SpecsToStrings(options));
        }

        protected OptionException(ICollection<IOptionSpec<object>> options, string message) : base(message)
        {
            _options.AddRange(SpecsToStrings(options));
        }

        private List<string> SpecsToStrings(ICollection<IOptionSpec<object>> options)
        {
            List<string> strings = [];
            foreach (var option in options)
                strings.Add(SpecToString(option));
            return strings;
        }

        private static string SpecToString(IOptionSpec<object> option)
        {
            return string.Join("/", option.Options());
        }

        protected string SingleOptionString()
        {
            return SingleOptionString(_options[0]);
        }

        protected static string SingleOptionString(string option)
        {
            return option;
        }

        protected string MultipleOptionString()
        {
            StringBuilder buffer = new("[");

            var asSet = new HashSet<string>(_options);
            for (var iter = asSet.GetEnumerator(); iter.(); )
            {
                buffer
            }
        }
    }
}
