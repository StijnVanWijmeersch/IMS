namespace IMS.API.Requests.Stores;

public sealed record CreateStoreRequest(string Name, string Url, string Key, string Secret);
