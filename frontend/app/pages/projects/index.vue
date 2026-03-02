<script setup lang="ts">
import axios from 'axios'
import type { ApiResponse, Project, CreateProjectRequest } from '~/types'

definePageMeta({ middleware: 'auth' })

const { logout } = useAuth()
const authStore = useAuthStore()
const api = useApi()

const projects = ref<Project[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

const showCreateModal = ref(false)
const createForm = reactive<CreateProjectRequest>({ name: '', description: '' })
const createLoading = ref(false)
const createError = ref<string | null>(null)

onMounted(async () => {
  try {
    const res = await api.get<ApiResponse<Project[]>>('/projects')
    projects.value = res.data.data
  } catch {
    error.value = 'Failed to load projects.'
  } finally {
    loading.value = false
  }
})

async function handleCreateProject() {
  createError.value = null
  createLoading.value = true
  try {
    const res = await api.post<ApiResponse<Project>>('/projects', {
      name: createForm.name,
      description: createForm.description || null
    })
    projects.value.push(res.data.data)
    showCreateModal.value = false
    createForm.name = ''
    createForm.description = ''
  } catch (err: unknown) {
    if (axios.isAxiosError(err) && err.response?.data?.message) {
      createError.value = err.response.data.message
    } else {
      createError.value = 'Failed to create project.'
    }
  } finally {
    createLoading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <header class="bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
      <h1 class="text-xl font-bold text-gray-800">TaskFlow</h1>
      <div class="flex items-center gap-4">
        <span class="text-sm text-gray-600">{{ authStore.user?.name }}</span>
        <button @click="logout" class="text-sm text-red-600 hover:underline">Logout</button>
      </div>
    </header>

    <main class="max-w-5xl mx-auto px-6 py-8">
      <div class="flex items-center justify-between mb-6">
        <h2 class="text-2xl font-bold text-gray-800">Projects</h2>
        <button
          @click="showCreateModal = true"
          class="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700"
        >
          + New Project
        </button>
      </div>

      <div v-if="loading" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        <div
          v-for="i in 3"
          :key="i"
          class="bg-white rounded-xl border border-gray-200 p-5 animate-pulse"
        >
          <div class="h-4 bg-gray-200 rounded w-3/4 mb-3" />
          <div class="h-3 bg-gray-100 rounded w-1/2" />
        </div>
      </div>

      <p v-else-if="error" class="text-red-500">{{ error }}</p>

      <div v-else-if="projects.length === 0" class="text-gray-500">
        No projects yet. Create your first one.
      </div>

      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        <NuxtLink
          v-for="project in projects"
          :key="project.id"
          :to="`/projects/${project.id}`"
          class="bg-white rounded-xl shadow-sm border border-gray-200 p-5 hover:shadow-md transition-shadow block"
        >
          <h3 class="font-semibold text-gray-800 mb-1">{{ project.name }}</h3>
          <p v-if="project.description" class="text-sm text-gray-500 mb-3">{{ project.description }}</p>
          <p class="text-xs text-gray-400">Created {{ new Date(project.createdAt).toLocaleDateString() }}</p>
        </NuxtLink>
      </div>
    </main>

    <!-- Create Project Modal -->
    <div
      v-if="showCreateModal"
      class="fixed inset-0 bg-black/50 flex items-center justify-center z-50"
      @click.self="showCreateModal = false"
    >
      <div class="bg-white rounded-xl shadow-xl p-6 w-full max-w-md mx-4">
        <h3 class="text-lg font-bold text-gray-800 mb-4">New Project</h3>
        <form @submit.prevent="handleCreateProject" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Name</label>
            <input
              v-model="createForm.name"
              type="text"
              required
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Description</label>
            <textarea
              v-model="createForm.description"
              rows="3"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <p v-if="createError" class="text-red-500 text-sm">{{ createError }}</p>
          <div class="flex gap-3 justify-end">
            <button
              type="button"
              @click="showCreateModal = false"
              class="px-4 py-2 text-sm text-gray-600 hover:text-gray-800"
            >
              Cancel
            </button>
            <button
              type="submit"
              :disabled="createLoading"
              class="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50"
            >
              {{ createLoading ? 'Creating...' : 'Create' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>
