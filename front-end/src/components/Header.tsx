import { Link } from "react-router-dom"
import useAuth from "@/store/useAuth"
import { Button } from "@/components/ui/button"

export default function Header() {
  const user = useAuth((s) => s.user)

  return (
    <header className="border-b p-4">
      <div className="container mx-auto flex items-center justify-between">
        <Link to="/" className="font-medium">
          ChatApp
        </Link>
        <nav className="flex items-center gap-4">
          {user ? (
            <div className="text-sm">{user.username}</div>
          ) : (
            <>
              <Link to="/login">
                <Button variant="ghost">Login</Button>
              </Link>
              <Link to="/signup">
                <Button>Sign up</Button>
              </Link>
            </>
          )}
        </nav>
      </div>
    </header>
  )
}
