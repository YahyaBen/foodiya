namespace Foodiya.Domain.Configuration;

public sealed class BlobStorageOptions
{
    public const string SectionName = "AzureStorage";

    public string ConnectionString { get; set; } = string.Empty;

    public string ContainerName { get; set; } = "foodiya-images";
}
