namespace Rxmvvm
{
    public struct DataErrorChanged
    {
        public DataErrorChanged(string propertyName, string error)
        {
            Error = error;
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        public string Error { get; }
    }
}