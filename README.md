# Web-Chat-App

Overview
--------
This project is a full-stack, real-time chat application focused on privacy-first communication. It enables two users to exchange messages securely, with all content protected by end-to-end encryption using the Signal Protocol.

The system consists of:

- A backend built with ASP.NET Core that handles authentication, message routing, and encrypted data storage

- A frontend (React + TypeScript) responsible for encryption, key management, and user interaction

Messages are encrypted on the client before being sent and remain encrypted throughout their lifecycle. The backend never has access to plaintext messages or private keys, acting purely as a transport and persistence layer.

The application supports:

- Secure user authentication

- Real-time communication via SignalR

- Private one-to-one messaging

- Encrypted message storage

- Client-side key generation and management

This design ensures that user conversations remain confidential, even from the server itself.

Tech stack
----------
- Backend: .NET 10 (ASP.NET Core)
- Real-time: SignalR
- Authentication: ASP.NET Core Identity + JWT (Bearer)
- CQRS/Mediator: MediatR
- Data storage: Entity Framework Core (Postgres for production; InMemory used in tests)
- Logging: Serilog (console + file sinks)
- Frontend: React + TypeScript + shadcn/ui Component library + Zustand state management + TanStack Table and Query + React Hook Form + Zod + Recharts + SignalR Client 


Project Status
--------------
This project is actively under development.

Currently implemented:
- Authentication and JWT-based access control
- SignalR-based real-time messaging infrastructure
- Key exchange endpoints and persistence
- Core backend architecture (CQRS, logging, data access)

In progress:
- End-to-end message persistence and delivery guarantees
- Full Signal Protocol integration on the frontend (Double Ratchet)
- React frontend for user interaction and key management

Planned:
-------------------
- Persist delivered and undelivered messages (Message entity + repository) and deliver-on-connect
- Integrate a real Signal Protocol client implementation in the frontend for proper Double Ratchet semantics
- Add refresh tokens / session management for long-lived clients
- Add retention, archival and storage quotas (cleanup job)
- Harden security (validate prekey signatures, rate-limiting, size limits, KMS for secrets)
- Add a small React+TypeScript demo client showing key upload, key fetch, E2EE session setup and chatting


Repository layout & containers
----------------------------
- `back-end/` - ASP.NET Core backend project (this repo)
- `front-end/` - React + TypeScript application (planned) that will be added to this repo
- `docker-compose.yml` - development compose that can orchestrate Postgres, backend and frontend dev server or built frontend container

The intent is that developers can run everything locally with a single `docker compose up` and have Postgres, backend, and frontend come up together. The `back-end` service talks to Postgres, and the frontend dev or production container will communicate with the backend (CORS/proxy configured for dev).

Dev container notes
-------------------
- The compose file currently includes `postgres`, `pgadmin4`, `redis` (optional) and a `back-end` service. The frontend will be added as a service (for dev it can run Vite and proxy API requests to the backend).
- Environment variables (JWT key, DB connection) should be provided to the containers via `.env` or your CI environment — do not commit secrets.
- For SignalR in containers, client should connect to `http(s)://<backend-host>/hubs/chat` and pass the JWT as `access_token` query parameter (already supported by the backend JWT configuration).

Quickstart (development)
-------------------------
Prerequisites:
- .NET 10 SDK
- Docker (for Postgres via docker-compose) or a Postgres instance
- (optional) dotnet-ef tool for applying migrations

1. Start Postgres (docker-compose):

   docker-compose up -d postgres

2. Update configuration if needed (`back-end/appsettings.json`):
   - `ConnectionStrings:DefaultConnection` should point to your Postgres instance
   - `Jwt:Key` should be changed from the development default for production use

3. Apply EF Core migrations (one-time):

   dotnet tool install --global dotnet-ef   # if you don't have it
   dotnet ef database update --project back-end --startup-project back-end

4. Run the backend:

   dotnet run --project back-end

   The API + SignalR hub will be available at the configured URL (defaults to https://localhost:5001 in dev).

5. Run tests:

   dotnet test

Database Schema
---------------

The application uses PostgreSQL with Entity Framework Core. ASP.NET Core Identity provides the base user and authentication tables.

### Identity Tables (provided by ASP.NET Core Identity)

- `AspNetUsers`
- `AspNetRoles`
- `AspNetUserRoles`
- `AspNetUserClaims`
- `AspNetUserLogins`
- `AspNetUserTokens`

These handle user accounts, roles, and authentication.

---

### Core Application Tables

#### Users (extension of Identity)
- Id (PK)
- UserName
- Email
- CreatedAt

---

#### Conversations
Represents a one-to-one chat between two users.

- Id (PK)
- User1Id (FK → Users)
- User2Id (FK → Users)
- CreatedAt

---

#### Messages
Stores encrypted messages exchanged between users.

- Id (PK, monotonic)
- ConversationId (FK → Conversations)
- SenderId (FK → Users)
- CipherText (encrypted payload)
- CreatedAt (server timestamp)
- Status (Sent, Delivered, Read)

---

#### UserDevices
Supports multi-device architecture required by Signal Protocol.

- Id (PK)
- UserId (FK → Users)
- DeviceId (unique per user)
- CreatedAt

---

#### KeyBundles
Stores public key material required for initiating encrypted sessions.

- Id (PK)
- UserId (FK → Users)
- DeviceId (FK → UserDevices)
- IdentityPublicKey
- SignedPreKey
- SignedPreKeySignature
- OneTimePreKeys (collection or separate table)
- CreatedAt

---

#### OneTimePreKeys
Used for X3DH key exchange (consumed on use).

- Id (PK)
- KeyBundleId (FK → KeyBundles)
- PublicKey
- IsUsed (bool)

---

#### Friendships (optional / planned)
Represents user relationships.

- Id (PK)
- RequesterId (FK → Users)
- AddresseeId (FK → Users)
- Status (Pending, Accepted, Blocked)
- CreatedAt

---

### Notes

- All message content is stored encrypted; the server never has access to plaintext.
- Message ordering is ensured via a monotonic Id and server-side timestamps.
- Multi-device support is handled via device-specific key bundles.
- One-time prekeys are consumed upon session establishment to maintain forward secrecy.

How the demo client should work (frontend responsibilities)
-------------------------------------------------------
- Implement full Signal Protocol (recommended libs: libsignal-protocol-js for web) or a Web Crypto ECDH+AES-GCM prototype for the demo.
- After login: generate device identity keys and upload a key bundle to `POST /keys/upload`.
- To start a conversation: `GET /keys/{userId}` to obtain the recipient's bundle (and one-time prekey) and perform the X3DH/Signal setup client-side.
- Connect to SignalR hub using the JWT: `new signalR.HubConnectionBuilder().withUrl('/hubs/chat?access_token='+jwt)`
- Use `SendDirectMessage(recipientUserId, encryptedMessage)` to send ciphertext; server will persist and/or relay it.


Notes
-----
- The server intentionally never inspects or decrypts ciphertext; all encryption and key management logic (Signal) runs on clients.
- The repository includes unit tests for key handling and infrastructure to extend with message persistence and client demos.
