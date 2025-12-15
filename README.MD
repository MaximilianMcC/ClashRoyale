# Clash Royale Clone
remaking clash royale (better than supercells version trust)

## How the Networker class works
Each packet sent carries a header that tells the server how to handle the packet. Headers are separated using the section symbol, `§`. Example : `"<HEADER>§<PACKET>"`
| Header | Meaning |
|---|---|
| `s` | Just send to server |
| `c<CLIENT ID>` | Just send to specific client |
| `a` | Send to all clients |
| `x` | Send to all clients, except the client who sent the packet |