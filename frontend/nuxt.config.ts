export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  modules: ['@nuxtjs/tailwindcss', '@pinia/nuxt', '@vueuse/nuxt'],
  runtimeConfig: {
    public: {
      apiBase: '/api/v1'
    }
  },
  devServer: { port: 3000 },
  nitro: {
    devProxy: {
      '/api': {
        target: 'http://localhost:5000/api',
        changeOrigin: true
      }
    }
  }
})
