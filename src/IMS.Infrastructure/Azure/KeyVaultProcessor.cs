using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using IMS.Application.Abstractions;
using IMS.SharedKernel;
using Microsoft.Extensions.Configuration;

namespace IMS.Infrastructure.Azure;

public sealed class KeyVaultProcessor : IKeyVaultProcessor
{
    private readonly IConfiguration _configuration;
    private readonly string keyVaultURL;
    private readonly string clientId;
    private readonly string clientSecret;
    private readonly string directoryId;

    public KeyVaultProcessor(IConfiguration configuration)
    {
        _configuration = configuration;

        keyVaultURL = _configuration.GetSection("KeyVault:KeyVaultURL").Value!;
        clientId = _configuration.GetSection("KeyVault:ClientId").Value!;
        clientSecret = _configuration.GetSection("KeyVault:ClientSecret").Value!;
        directoryId = _configuration.GetSection("KeyVault:DirectoryID").Value!;
    }

    public async Task<Result> CreateNewSecretAsync(string secretName, string secretValue)
    {
        var client = new SecretClient(new Uri(keyVaultURL), new ClientSecretCredential(directoryId, clientId, clientSecret));

        try
        {
            await client.SetSecretAsync(secretName, secretValue);
            return Result.Success();
        }
        catch (Exception ex)
        {
            var error = new Error("KeyVaultProcessor.CreateNewSecretAsync", ex.Message);

            return Result.Failure(error);
        }
    }

    public async Task<Result<string>> GetSecretAsync(string secretName)
    {
        var client = new SecretClient(new Uri(keyVaultURL), new ClientSecretCredential(directoryId, clientId, clientSecret));

        try
        {
            var secret = await client.GetSecretAsync(secretName);

            return Result.Success(secret.Value.Value);
        }
        catch (Exception ex)
        {
            var error = new Error("KeyVaultProcessor.GetSecretAsync", ex.Message);

            return Result.Failure<string>(error);
        }
    }

    public async Task<Result> RemoveSecretAsync(string secretName)
    {
        var client = new SecretClient(new Uri(keyVaultURL), new ClientSecretCredential(directoryId, clientId, clientSecret));

        try
        {
            var operation = await client.StartDeleteSecretAsync(secretName);

            await operation.WaitForCompletionAsync();

            await client.PurgeDeletedSecretAsync(secretName);

            return Result.Success();
        }
        catch (Exception ex)
        {
            var error = new Error("KeyVaultProcessor.RemoveSecretAsync", ex.Message);

            return Result.Failure(error);
        }
    }
}
