import { LoginForm } from "@/components/login-form"
import type { LoginFormValues } from "@/components/login-form"
import { useMutation } from "@tanstack/react-query"
import useAuth from "@/store/useAuth"
import { login } from "@/domain/auth/api"
import type { AuthResult, LoginDto } from "@/types/dtos"

export default function LoginPage() {
  const setAuth = useAuth((s) => s.setAuth)

  const loginMutation = useMutation<AuthResult, unknown, LoginDto>({
    mutationFn: (body: LoginDto) => login(body),
    onSuccess: (data: AuthResult) => {
      if (data?.token && data?.user) {
        setAuth(data.token, data.user)
      }
    },
  })

  return (
    <div className="min-h-svh grid place-items-center p-6">
      <div className="w-full max-w-sm">
        <LoginForm onSubmit={(d: LoginFormValues) => loginMutation.mutate({ username: d.usernameOrEmail, password: d.password })} />
      </div>
    </div>
  )
}
