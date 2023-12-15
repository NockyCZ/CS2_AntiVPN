using Newtonsoft.Json.Linq;

namespace AntiVPN;
public static class Config
{
	public static JObject? JsonConfigData { get; private set; }
	public static List<string>? SteamidWhitelist { get; private set; }
	public static List<string>? BlockedCountries { get; private set; }
	public static void CreateOrLoadConfig(string filepath)
	{
		if (!File.Exists(filepath))
		{
			JObject exampleData = new JObject
			{
				["AntiVPN"] = new JObject
				{
                    			["steamid_whitelist"] = new JArray { "76561198429950772", "76561198808392634" },
                    			["blocked_countries"] = new JArray { "RU", "CN" }
				}
			};
			File.WriteAllText(filepath, exampleData.ToString());
			var jsonData = File.ReadAllText(filepath);
			JsonConfigData = JObject.Parse(jsonData);
		}
		else
		{
			var jsonData = File.ReadAllText(filepath);
			JsonConfigData = JObject.Parse(jsonData);
		}

		JArray steamidWhitelistArray = (JArray)JsonConfigData["AntiVPN"]!["steamid_whitelist"]!;
        	SteamidWhitelist = steamidWhitelistArray.ToObject<List<string>>();

		JArray blockedCountriesArray = (JArray)JsonConfigData["AntiVPN"]!["blocked_countries"]!;
        	BlockedCountries = blockedCountriesArray.ToObject<List<string>>();
	}
}
