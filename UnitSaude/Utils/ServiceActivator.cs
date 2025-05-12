namespace UnitSaude.Utils
{
    public static class ServiceActivator
    {
        internal static IServiceProvider _serviceProvider = null;

        public static void Configure(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static IServiceScope GetScope()
        {
            return _serviceProvider.CreateScope();
        }
    }
}
