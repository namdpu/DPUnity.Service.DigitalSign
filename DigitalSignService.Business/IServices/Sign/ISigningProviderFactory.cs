namespace DigitalSignService.Business.IServices.Sign
{
    public interface ISigningProviderFactory
    {
        ISigningProvider GetProvider(string providerKey);
    }
}
