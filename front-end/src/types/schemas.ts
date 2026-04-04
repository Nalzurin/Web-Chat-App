import { z } from "zod"

export const userDtoSchema = z.object({
  id: z.string(),
  username: z.string(),
  email: z.string().email(),
  createdAt: z.string(),
})

export const authResultSchema = z.object({
  token: z.string(),
  user: userDtoSchema,
})

export const createUserSchema = z.object({
  username: z.string().min(1),
  email: z.string().email(),
  password: z.string().min(8),
})

export const loginSchema = z.object({
  username: z.string().min(1),
  password: z.string().min(1),
})

export type CreateUserSchema = z.infer<typeof createUserSchema>
export type LoginSchema = z.infer<typeof loginSchema>
export type UserDtoSchema = z.infer<typeof userDtoSchema>
export type AuthResultSchema = z.infer<typeof authResultSchema>
