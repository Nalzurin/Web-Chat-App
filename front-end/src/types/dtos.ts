export type UserDto = {
  id: string
  username: string
  email: string
  createdAt: string
}

export type AuthResult = {
  token: string
  user: UserDto
}

export type CreateUserDto = {
  username: string
  email: string
  password: string
}

export type LoginDto = {
  username: string
  password: string
}

export type KeyBundleResponseDto = {
  deviceId: string
  registrationId: number
  identityKey: string
  signedPreKey: string
  signedPreKeySig: string
  oneTimePreKey?: string | null
}

export type EncryptedMessageDto = {
  cipherText: string
  nonce: string
  ephemeralPublicKey?: string | null
  algorithm?: string | null
}

export type UsersListDto = UserDto[]

