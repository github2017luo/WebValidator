using Microsoft.Extensions.Configuration;

namespace WebValidator.Configuration
{
    public static class ConfigurationLoader
    {
        private static IConfiguration Config { get; set; }

        public static T GetOption<T>(string key)
        {
            if (Config == null)
            {
                Config = GetConfigBuilder();
            }

            return Config.GetSection(key).Get<T>();
        }

        public static T GetOption<T>()
        {
            return GetOption<T>(typeof(T).Name);
        }

        private static IConfiguration GetConfigBuilder()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        }
    }
}