<script setup lang="ts">
definePageMeta({ middleware: 'auth' })

const { login, error, loading } = useAuth()

const email = ref('')
const password = ref('')

async function handleSubmit() {
  const ok = await login(email.value, password.value)
  if (ok) await navigateTo('/projects')
}
</script>

<template>
  <div class="min-h-screen bg-gray-50 flex items-center justify-center">
    <div class="bg-white rounded-xl shadow p-8 w-full max-w-md">
      <h1 class="text-2xl font-bold text-gray-800 mb-6">Sign in to TaskFlow</h1>

      <form @submit.prevent="handleSubmit" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Email</label>
          <input
            v-model="email"
            type="email"
            required
            class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Password</label>
          <input
            v-model="password"
            type="password"
            required
            class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <p v-if="error" class="text-red-500 text-sm">{{ error }}</p>

        <button
          type="submit"
          :disabled="loading"
          class="w-full bg-blue-600 text-white rounded-lg py-2 font-medium hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {{ loading ? 'Signing in...' : 'Sign in' }}
        </button>
      </form>

      <p class="mt-4 text-sm text-center text-gray-600">
        Don't have an account?
        <NuxtLink to="/register" class="text-blue-600 hover:underline">Register</NuxtLink>
      </p>
    </div>
  </div>
</template>
