using Newtonsoft.Json.Linq;
using System.Globalization;
using cmedcc_idass.backend.Exceptions;
using Microsoft.Extensions.Configuration;
using cmedcc_idass.backend.Security;
using System.Security.Cryptography;
using System.Text;
namespace cmedcc_idass.backend.Utilize;

using System.Globalization;
using cmedcc_idass.backend.Constans;
public static class ConfigUtilize
{
    public static string LoadConfig(IConfiguration pvConfiguration)
    {
        var baseUrlConfig = pvConfiguration.GetValue<string>("AppInfo:BaseUrlConfig");
        var appContext = pvConfiguration.GetValue<string>("AppInfo:AppContext");
        var appEnv = pvConfiguration.GetValue<string>("AppInfo:AppEnv");
        // appEnv="Local" then application will load config from appsetting.json
        if (appEnv != null && appEnv.Equals("Local", StringComparison.OrdinalIgnoreCase))
        {
            var configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Local.json");
            if (!File.Exists(configFilePath))
                throw new FileNotFoundException("Local appsettings.Local.json file not found.", configFilePath);
            return File.ReadAllText(configFilePath);
        }

        // appEnv="Develop|Sandbox" then application will load config from config-service API
        var configServiceUrl = Path.Combine(
            baseUrlConfig ?? throw new AppException("BaseUrlConfig is missing from appsettings.json."),
            appContext!.ToLower() ??
            throw new AppException("AppContext is missing from appsettings.json."), appEnv!.ToLower() ??
            throw new AppException("AppEnv is missing from appsettings.json."));
        var httpClient = new HttpClient();
        var response = httpClient
            .GetStringAsync(configServiceUrl).Result;
        // read public key for encryption
        var privateKey = Path.Combine(Directory.GetCurrentDirectory(), "Certificates", "private_key.pem");
        return GetJsonDecryption(privateKey, response);
    }

    private static string GetJsonDecryption(string privateKey, string jStrProperties)
    {
        var token = JToken.Parse(jStrProperties);
        TraverseDecryptJson(privateKey, token);
        return token.ToString();
    }

    private static void TraverseDecryptJson(string privateKeyPath, JToken token)
    {
        switch (token)
        {
            case JValue value:
                value.Value =
                    RsaDecryption.Decrypt(privateKeyPath, value.ToString(CultureInfo.CurrentCulture));
                break;

            case JObject obj:
            {
                foreach (var property in obj.Properties())
                {
                    // Skip the "AppInfo" property and everything under it
                    if (property.Name == Strings.IgnoreJsonTag)
                    {
                        continue;
                    }

                    TraverseDecryptJson(privateKeyPath, property.Value);
                }

                break;
            }

            case JArray array:
            {
                foreach (var item in array)
                {
                    TraverseDecryptJson(privateKeyPath, item);
                }

                break;
            }
        }
    }
}