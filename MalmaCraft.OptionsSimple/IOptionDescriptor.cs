namespace MalmaCraft.OptionsSimple
{
    public interface IOptionDescriptor
    {
        public List<string> Options();
        public string Description();
        public List<object> DefaultValues();
        public bool IsRequired();
        public bool AcceptsArguments();
        public bool RequiresArguments();
        public string ArgumentDescription();
        public string ArgumentTypeIndicator();
        public bool RepresentsNonOptions();
        public IValueConverter<T>? ArgumentConverter<T>();
    }
}
