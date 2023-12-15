# CS2_AntiVPN
An Anti-VPN & Country Blocker plugin for [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp). It is also possible to set up a whitelist for specific steamid64, on which VPN or Country blocking will not take effect.

### Configuration in ```/plugins/AntiVPN/antivpn_config.json```
|   | What it does |
| ------------- | ------------- |
| `steamid_whitelist`  | Which steamids will be whitelisted (steamid64) |
| `blocked_countries`  | Which countries will be disabled, use Alpha2-codes: https://www.iban.com/country-codes |

### Admin commands (Only server console)
```css_antivpn_reload``` - Reload config file

### Installation
1. Unzip into your servers `csgo/addons/counterstrikesharp/plugins/AntiVPN/` dir
