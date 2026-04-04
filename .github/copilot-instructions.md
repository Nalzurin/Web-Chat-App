# Copilot Instructions

## Core Rules

- This is an end-to-end encrypted chat application using the Signal Protocol.
- The backend MUST NEVER decrypt messages or access plaintext.
- All cryptographic operations happen on the client.

## Backend Guidelines

- Use ASP.NET Core Minimal APIs (no controllers).
- Use MediatR for business logic (handlers, not inline logic).
- Keep endpoints thin and delegate to handlers.
- Use EF Core with PostgreSQL.

## Real-time Messaging

- SignalR is used for message delivery.
- Messages handled by the server are ALWAYS encrypted payloads.

## Architecture Principles

- Follow clean separation of concerns (API → Handler → Data).
- Do not introduce shortcuts that break encryption boundaries.
- Avoid mixing infrastructure and domain logic.

## Frontend Expectations

- React + TypeScript
- Client handles encryption (Signal Protocol)
- Use TanStack Query for server state, Zustand for client state

## General

- Prefer maintainable, production-style code over quick hacks.
- Do not introduce patterns inconsistent with the existing architecture.
- If you are adding something new to the code and using the same general pattern as something that is already existing in the code base but you know there is a better or improved way then use the better one than the existing one, while recommending to switch over to the better one to the user.