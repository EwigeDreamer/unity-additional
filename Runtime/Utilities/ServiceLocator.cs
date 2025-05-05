namespace ED.Additional.Utilities
{
    public static class ServiceLocator
    {
        public static void RegisterService<T>() where T : new() => ServiceHandler<T>.Instance = new T();
        public static void RegisterService<T>(T instance) => ServiceHandler<T>.Instance = instance;
        public static T GetService<T>() => ServiceHandler<T>.Instance;

        private static class ServiceHandler<T>
        {
            public static T Instance { get; set; }
        }
    }
}