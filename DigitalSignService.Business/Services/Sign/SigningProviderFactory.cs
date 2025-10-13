using DigitalSignService.Business.IServices.Sign;

namespace DigitalSignService.Business.Services.Sign
{
    public class SigningProviderFactory : ISigningProviderFactory
    {
        private readonly IEnumerable<ISigningProvider> _providers;

        public SigningProviderFactory(IEnumerable<ISigningProvider> providers)
        {
            _providers = providers;
        }

        public ISigningProvider GetProvider(string providerKey)
        {
            var provider = _providers.FirstOrDefault(p =>
                p.Name.Equals(providerKey, StringComparison.OrdinalIgnoreCase));

            if (provider == null)
                throw new InvalidOperationException($"Provider '{providerKey}' not supported.");

            return provider;
        }
    }
}
