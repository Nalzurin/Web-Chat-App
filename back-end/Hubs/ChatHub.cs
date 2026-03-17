using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace back_end.Hubs;

// The hub acts as a relay for encrypted peer-to-peer messages. The server never
// inspects or decrypts message payloads — it simply routes ciphertext between
// authenticated users.
[Authorize]
public class ChatHub : Hub
{
    // In-memory mapping of userId -> connection ids. A user may have multiple connections.
    private static readonly ConcurrentDictionary<Guid, HashSet<string>> _connections = new();

    private static HashSet<string> GetOrCreate(Guid userId) => _connections.GetOrAdd(userId, _ => new HashSet<string>());

    public override Task OnConnectedAsync()
    {
        var userIdString = Context.UserIdentifier;
        if (Guid.TryParse(userIdString, out var userId))
        {
            var set = GetOrCreate(userId);
            lock (set)
            {
                set.Add(Context.ConnectionId);
            }
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdString = Context.UserIdentifier;
        if (Guid.TryParse(userIdString, out var userId))
        {
            if (_connections.TryGetValue(userId, out var set))
            {
                lock (set)
                {
                    set.Remove(Context.ConnectionId);
                    if (set.Count == 0)
                        _connections.TryRemove(userId, out _);
                }
            }
        }

        return base.OnDisconnectedAsync(exception);
    }

    // Relay an encrypted message to a single recipient. The message must be
    // encrypted by the sender and can only be decrypted by the recipient.
    public async Task SendDirectMessage(Guid recipientUserId, Hubs.Dtos.EncryptedMessageDto message)
    {
        var senderIdString = Context.UserIdentifier;
        if (!Guid.TryParse(senderIdString, out var senderId))
            throw new HubException("Invalid sender identifier");

        if (_connections.TryGetValue(recipientUserId, out var recipients))
        {
            var clients = recipients.ToList();
            await Clients.Clients(clients).SendAsync("ReceiveDirectMessage", senderId, message);
        }
        else
        {
            // Optionally, you could persist undelivered messages here.
        }
    }

    // Publish or forward a public key to another user so they can establish
    // a shared secret client-side. The server does not validate or store keys.
    public async Task PublishPublicKey(Guid recipientUserId, string publicKey)
    {
        var senderIdString = Context.UserIdentifier;
        if (!Guid.TryParse(senderIdString, out var senderId))
            throw new HubException("Invalid sender identifier");

        if (_connections.TryGetValue(recipientUserId, out var recipients))
        {
            var clients = recipients.ToList();
            await Clients.Clients(clients).SendAsync("ReceivePublicKey", senderId, publicKey);
        }
    }
}
