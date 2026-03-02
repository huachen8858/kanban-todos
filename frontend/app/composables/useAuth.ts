import axios from 'axios'
import { useAuthStore } from '~/stores/auth'
import type { ApiResponse, AuthResponse } from '~/types'

export const useAuth = () => {
  const authStore = useAuthStore()
  const api = useApi()
  const error = ref<string | null>(null)
  const loading = ref(false)

  async function login(email: string, password: string): Promise<boolean> {
    error.value = null
    loading.value = true
    try {
      const res = await api.post<ApiResponse<AuthResponse>>('/auth/login', { email, password })
      authStore.setAuth(res.data.data.token, res.data.data.user)
      return true
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data?.message) {
        error.value = err.response.data.message
      } else {
        error.value = 'Login failed. Please try again.'
      }
      return false
    } finally {
      loading.value = false
    }
  }

  async function register(name: string, email: string, password: string): Promise<boolean> {
    error.value = null
    loading.value = true
    try {
      const res = await api.post<ApiResponse<AuthResponse>>('/auth/register', { name, email, password })
      authStore.setAuth(res.data.data.token, res.data.data.user)
      return true
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data?.message) {
        error.value = err.response.data.message
      } else {
        error.value = 'Registration failed. Please try again.'
      }
      return false
    } finally {
      loading.value = false
    }
  }

  function logout() {
    authStore.logout()
    navigateTo('/login')
  }

  return { login, register, logout, error, loading }
}
