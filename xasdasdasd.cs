using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

class Program
{
    static async Task Main(string[] args)
    {
        // 🔹 CHANGE THESE
        string secretName = "my-secret-name";
        string region = "ap-southeast-1";

        try
        {
            var secretValue = await GetSecretAsync(secretName, region);
            Console.WriteLine("Secret value:");
            Console.WriteLine(secretValue);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error:");
            Console.WriteLine(ex.Message);
        }
    }

    static async Task<string> GetSecretAsync(string secretName, string region)
    {
        var client = new AmazonSecretsManagerClient(
            RegionEndpoint.GetBySystemName(region)
        );

        var request = new GetSecretValueRequest
        {
            SecretId = secretName
        };

        var response = await client.GetSecretValueAsync(request);

        // SecretString (most common)
        if (!string.IsNullOrEmpty(response.SecretString))
        {
            return response.SecretString;
        }

        // Binary secret (rare)
        return Convert.ToBase64String(response.SecretBinary.ToArray());
    }
}
