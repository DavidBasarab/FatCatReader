using Microsoft.Win32;

namespace Common.Information
{
    public class DefaultRegistryReader : RegistryReader
    {
        public string GetValue(string registryLocation)
        {
            var key = Registry.LocalMachine;

            return null;
        }
    }
}