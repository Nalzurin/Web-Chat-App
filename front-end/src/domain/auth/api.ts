import type { CreateUserDto, LoginDto, AuthResult } from "@/types/dtos"
import { authResultSchema, createUserSchema } from "@/types/schemas"

const API_BASE: string = (import.meta.env.VITE_API_URL as string) ?? ""

function buildUrl(path: string): string {
  const base = API_BASE.replace(/\/+$/u, "")
  if (!path.startsWith("/")) {
    path = `/${path}`
  }
  return `${base}${path}`
}

async function postJson<TReq, TRes>(url: string, body: TReq): Promise<TRes> {
  const fullUrl: string = buildUrl(url)
  const response: Response = await fetch(fullUrl, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  })

  if (!response.ok) {
    const errBody: unknown = await response.json().catch(() => ({}))
    throw errBody
  }

  const json: unknown = await response.json()
  return json as TRes
}

export async function login(dto: LoginDto): Promise<AuthResult> {
  const result: unknown = await postJson<LoginDto, unknown>("/login", dto)
  const parsed = authResultSchema.parse(result)
  return parsed as AuthResult
}

export async function signup(dto: CreateUserDto): Promise<void> {
  createUserSchema.parse(dto)
  await postJson<CreateUserDto, void>("/users", dto)
}
