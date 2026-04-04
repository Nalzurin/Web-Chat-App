import { SignupForm } from "@/components/signup-form"
import { useMutation } from "@tanstack/react-query"
import { signup } from "@/domain/auth/api"
import type { CreateUserDto } from "@/types/dtos"
import type { SignupValues } from "@/components/signup-form"

export default function SignupPage() {
  const signupMutation = useMutation<void, unknown, CreateUserDto>({
    mutationFn: (body: CreateUserDto) => signup(body),
    onSuccess: () => {
      // maybe redirect to login
    },
  })

  return (
    <div className="min-h-svh grid place-items-center p-6">
      <div className="w-full max-w-sm">
        <SignupForm onSubmit={(d: Omit<SignupValues, 'onSubmit'>) => signupMutation.mutate({ username: d.name, email: d.email, password: d.password })} />
      </div>
    </div>
  )
}
