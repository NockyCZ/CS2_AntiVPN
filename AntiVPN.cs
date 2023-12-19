using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using Microsoft.Extensions.Logging;

namespace AntiVPN;

public class AntiVPN : BasePlugin
{
    public override string ModuleName => "Anti VPN & Country Blocker";
    public override string ModuleAuthor => "Nocky";
    public override string ModuleVersion => "1.0";

    public override void Load(bool hotReload)
    {
        Config.CreateOrLoadConfig(ModuleDirectory + "/antivpn_config.json");
    }
    [GameEventHandler]
    private HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
   	CCSPlayerController player = @event.Userid;

        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV || player.AuthorizedSteamID == null) 
            return HookResult.Continue;

   	AddTimer(0.5f, () => CheckPlayerIP(player));
	return HookResult.Continue;
    }

    [ConsoleCommand("css_antivpn_reload")]
    [CommandHelper(whoCanExecute: CommandUsage.SERVER_ONLY)]
    public void OnReloadCommand(CCSPlayerController caller, CommandInfo command)
    {
    	Config.CreateOrLoadConfig(ModuleDirectory + "/antivpn_config.json");
        command.ReplyToCommand("The configuration file \"antivpn_config.json\" reloaded.");
    }
    public void CheckPlayerIP(CCSPlayerController player)
    {
        string ipAddress = player!.IpAddress!.Split(":")[0];
        string steamid = player!.AuthorizedSteamID!.SteamId64.ToString();

        var isVPN = IsIpVPN(ipAddress).GetAwaiter().GetResult();
        if (isVPN){
            if (!Config.SteamidWhitelist!.Contains(steamid))
            {
                Server.ExecuteCommand($"kickid {player.UserId} VPN usage is not allowed on this server");
                Logger.LogInformation($"Player {player.PlayerName} ({steamid}) ({ipAddress}) has been kicked. (VPN Usage)");
                return;
            }
            return;
        }
        var countryCode = GetCountryCode(ipAddress).GetAwaiter().GetResult();
        if (Config.BlockedCountries!.Contains(countryCode))
        {
            if (!Config.SteamidWhitelist!.Contains(steamid))
            {
                Server.ExecuteCommand($"kickid {player.UserId} Your country is not allowed on this server");
                Logger.LogInformation($"Player {player.PlayerName} ({steamid}) ({ipAddress} | {countryCode}) has been kicked. (Disabled Country)");
                return;
            }
            return;
        }
    }
    static async Task <bool>IsIpVPN(string ipAddress)
    {
        using (var client = new HttpClient())
        {
            string requestURL = $"https://blackbox.ipinfo.app/lookup/{ipAddress}";

            HttpResponseMessage response = await client.GetAsync(requestURL);
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                if(jsonResponse == "Y"){
                    return true;
                }
                else{
                    return false;
                }
            }
            return false;
        }
    }
    static async Task <string>GetCountryCode(string ipAddress)
    {   
        using (var client = new HttpClient())
        {
            string requestURL = $"https://ipinfo.io/{ipAddress}/json";

            HttpResponseMessage response = await client.GetAsync(requestURL);
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse)!;
                return $"{data.country}";
            }
            return "";
        }
    }
}
