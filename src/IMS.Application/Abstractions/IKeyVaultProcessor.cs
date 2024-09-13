using IMS.SharedKernel;

namespace IMS.Application.Abstractions;

public interface IKeyVaultProcessor
{
    Task<Result> CreateNewSecretAsync(string secretName, string secretValue);
    Task<Result<string>> GetSecretAsync(string secretName);
    Task<Result> RemoveSecretAsync(string secretName);
}
