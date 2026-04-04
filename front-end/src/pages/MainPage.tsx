import { Link } from "react-router-dom"

export default function MainPage() {
  return (
    <div className="min-h-svh grid place-items-center p-6">
      <div className="w-full max-w-3xl text-center">
        <h1 className="text-3xl font-heading mb-4">Welcome to ChatApp</h1>
        <p className="text-muted-foreground mb-6">
          ChatApp is a modern, privacy-focused chat platform built with a
          modular architecture. This demo uses a .NET 10 back end with a TypeScript
          front end, Vite, shadcn UI primitives, Zustand for state, TanStack
          React Query for data fetching, React Hook Form for forms, and Zod for
          validation.
        </p>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="rounded-none bg-card p-4">
            <h3 className="font-medium">Fast</h3>
            <p className="text-sm text-muted-foreground">Low-latency messaging powered by SignalR.</p>
          </div>
          <div className="rounded-none bg-card p-4">
            <h3 className="font-medium">Secure</h3>
            <p className="text-sm text-muted-foreground">JWT-based authentication and database encryption.</p>
          </div>
          <div className="rounded-none bg-card p-4">
            <h3 className="font-medium">Product-ready</h3>
            <p className="text-sm text-muted-foreground">Opinionated architecture and developer ergonomics.</p>
          </div>
        </div>
        <div className="mt-8">
          <Link to="/signup" className="mr-4">Get started</Link>
          <Link to="/login">Sign in</Link>
        </div>
      </div>
    </div>
  )
}
