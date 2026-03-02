import axios from 'axios'
import { useAuthStore } from '~/stores/auth'

export const useApi = () => {
  const config = useRuntimeConfig()
  const authStore = useAuthStore()

  const instance = axios.create({
    baseURL: config.public.apiBase as string
  })

  instance.interceptors.request.use((cfg) => {
    if (authStore.token) {
      cfg.headers.Authorization = `Bearer ${authStore.token}`
    }
    return cfg
  })

  instance.interceptors.response.use(
    (res) => res,
    async (error) => {
      if (error.response?.status === 401) {
        authStore.logout()
        await navigateTo('/login')
      }
      return Promise.reject(error)
    }
  )

  return instance
}
