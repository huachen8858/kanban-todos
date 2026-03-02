import { defineStore } from 'pinia'
import type { User } from '~/types'

export const useAuthStore = defineStore('auth', () => {
  const token = useCookie<string | null>('auth_token', { default: () => null, maxAge: 60 * 60 * 24 })
  const user = useCookie<User | null>('auth_user', { default: () => null, maxAge: 60 * 60 * 24 })

  const isAuthenticated = computed(() => !!token.value)

  function setAuth(newToken: string, newUser: User) {
    token.value = newToken
    user.value = newUser
  }

  function logout() {
    token.value = null
    user.value = null
  }

  return { token, user, isAuthenticated, setAuth, logout }
})
