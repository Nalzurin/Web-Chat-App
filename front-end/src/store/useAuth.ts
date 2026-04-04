import {create} from "zustand"

export type User = {
  id: string
  username: string
  email: string
}

type AuthState = {
  token?: string
  user?: User | null
  setAuth: (token: string, user: User) => void
  clear: () => void
}

const getInitial = (): { token?: string | undefined; user?: User | undefined } => {
  try {
    const token: string | null = localStorage.getItem("auth.token")
    const userRaw: string | null = localStorage.getItem("auth.user")
    const user: User | undefined = userRaw ? (JSON.parse(userRaw) as User) : undefined
    return { token: token ?? undefined, user }
  } catch {
    return { token: undefined, user: undefined }
  }
}

export const useAuth = create<AuthState>((set) => {
  const init = getInitial()
  return {
    token: init.token,
    user: init.user,
    setAuth: (token, user) => {
      try {
        localStorage.setItem("auth.token", token)
        localStorage.setItem("auth.user", JSON.stringify(user))
      } catch {}
      set(() => ({ token, user }))
    },
    clear: () => {
      try {
        localStorage.removeItem("auth.token")
        localStorage.removeItem("auth.user")
      } catch {}
      set(() => ({ token: undefined, user: undefined }))
    },
  }
})

export default useAuth
