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

const showEditModal = ref(false)
const editingProject = ref<Project | null>(null)
const editForm = reactive({ name: '', description: '' })
const editLoading = ref(false)
const editError = ref<string | null>(null)

const showDeleteConfirm = ref(false)
const deletingProject = ref<Project | null>(null)
const deleteLoading = ref(false)
const deleteError = ref<string | null>(null)

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

function openEdit(project: Project) {
  editingProject.value = project
  editForm.name = project.name
  editForm.description = project.description ?? ''
  editError.value = null
  showEditModal.value = true
}

async function handleEditProject() {
  if (!editingProject.value) return
  editError.value = null
  editLoading.value = true
  try {
    const res = await api.put<ApiResponse<Project>>(`/projects/${editingProject.value.id}`, {
      name: editForm.name,
      description: editForm.description || null
    })
    const updated = res.data.data
    const idx = projects.value.findIndex(p => p.id === updated.id)
    if (idx !== -1) projects.value[idx] = updated
    showEditModal.value = false
  } catch (err: unknown) {
    if (axios.isAxiosError(err) && err.response?.data?.message) {
      editError.value = err.response.data.message
    } else {
      editError.value = 'Failed to update project.'
    }
  } finally {
    editLoading.value = false
  }
}

function openDelete(project: Project) {
  deletingProject.value = project
  deleteError.value = null
  showDeleteConfirm.value = true
}

async function handleDeleteProject() {
  if (!deletingProject.value) return
  deleteError.value = null
  deleteLoading.value = true
  try {
    await api.delete(`/projects/${deletingProject.value.id}`)
    projects.value = projects.value.filter(p => p.id !== deletingProject.value!.id)
    showDeleteConfirm.value = false
  } catch (err: unknown) {
    if (axios.isAxiosError(err) && err.response?.data?.message) {
      deleteError.value = err.response.data.message
    } else {
      deleteError.value = 'Failed to delete project.'
    }
  } finally {
    deleteLoading.value = false
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
        <h2 class="text-2xl font-semibold text-gray-900 tracking-tight">Projects</h2>
        <button
          @click="showCreateModal = true"
          class="inline-flex items-center gap-1.5 bg-gray-900 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-gray-700 transition-colors"
        >
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" class="w-4 h-4">
            <path d="M10.75 4.75a.75.75 0 00-1.5 0v4.5h-4.5a.75.75 0 000 1.5h4.5v4.5a.75.75 0 001.5 0v-4.5h4.5a.75.75 0 000-1.5h-4.5v-4.5z" />
          </svg>
          New Project
        </button>
      </div>

      <div v-if="loading" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        <div v-for="i in 3" :key="i" class="bg-white rounded-xl border border-gray-200 p-5 animate-pulse">
          <div class="flex items-center gap-3 mb-4">
            <div class="w-8 h-8 bg-gray-100 rounded-lg" />
            <div class="h-4 bg-gray-200 rounded w-2/3" />
          </div>
          <div class="h-3 bg-gray-100 rounded w-1/2" />
        </div>
      </div>

      <p v-else-if="error" class="text-red-500 text-sm">{{ error }}</p>

      <div v-else-if="projects.length === 0" class="flex flex-col items-center justify-center py-20 text-center">
        <div class="w-12 h-12 rounded-xl bg-gray-100 flex items-center justify-center mb-4">
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" class="w-6 h-6 text-gray-400">
            <path d="M2 6a2 2 0 012-2h5l2 2h5a2 2 0 012 2v6a2 2 0 01-2 2H4a2 2 0 01-2-2V6z" />
          </svg>
        </div>
        <p class="text-gray-500 text-sm mb-1 font-medium">No projects yet</p>
        <p class="text-gray-400 text-sm">Create your first project to get started.</p>
      </div>

      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        <div
          v-for="project in projects"
          :key="project.id"
          class="bg-white rounded-xl border border-gray-200 hover:border-gray-300 transition-all flex flex-col"
        >
          <NuxtLink :to="`/projects/${project.id}`" class="flex-1 block p-5 group">
            <div class="flex items-start justify-between gap-2 mb-3">
              <div class="flex items-center gap-3 min-w-0">
                <div class="w-8 h-8 rounded-lg bg-blue-50 flex items-center justify-center flex-shrink-0">
                  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" class="w-4 h-4 text-blue-500">
                    <path d="M2 6a2 2 0 012-2h5l2 2h5a2 2 0 012 2v6a2 2 0 01-2 2H4a2 2 0 01-2-2V6z" />
                  </svg>
                </div>
                <h3 class="font-semibold text-gray-900 group-hover:text-blue-600 transition-colors truncate">{{ project.name }}</h3>
              </div>
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" class="w-4 h-4 text-gray-300 group-hover:text-blue-400 transition-colors flex-shrink-0 mt-0.5">
                <path fill-rule="evenodd" d="M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z" clip-rule="evenodd" />
              </svg>
            </div>
            <p v-if="project.description" class="text-sm text-gray-500 ml-11 mb-3 line-clamp-2">{{ project.description }}</p>
            <p class="text-xs text-gray-400 ml-11">{{ new Date(project.createdAt).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' }) }}</p>
          </NuxtLink>
          <div class="flex items-center justify-end gap-1 px-4 py-2.5 border-t border-gray-100">
            <button
              @click="openEdit(project)"
              title="Edit"
              class="p-1.5 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
            >
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" class="w-3.5 h-3.5">
                <path d="M2.695 14.763l-1.262 3.154a.5.5 0 00.65.65l3.155-1.262a4 4 0 001.343-.885L17.5 5.5a2.121 2.121 0 00-3-3L3.58 13.42a4 4 0 00-.885 1.343z" />
              </svg>
            </button>
            <button
              @click="openDelete(project)"
              title="Delete"
              class="p-1.5 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-colors"
            >
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" class="w-3.5 h-3.5">
                <path fill-rule="evenodd" d="M8.75 1A2.75 2.75 0 006 3.75v.443c-.795.077-1.584.176-2.365.298a.75.75 0 10.23 1.482l.149-.022.841 10.518A2.75 2.75 0 007.596 19h4.807a2.75 2.75 0 002.742-2.53l.841-10.52.149.023a.75.75 0 00.23-1.482A41.03 41.03 0 0014 4.193V3.75A2.75 2.75 0 0011.25 1h-2.5zM10 4c.84 0 1.673.025 2.5.075V3.75c0-.69-.56-1.25-1.25-1.25h-2.5c-.69 0-1.25.56-1.25 1.25v.325C8.327 4.025 9.16 4 10 4zM8.58 7.72a.75.75 0 00-1.5.06l.3 7.5a.75.75 0 101.5-.06l-.3-7.5zm4.34.06a.75.75 0 10-1.5-.06l-.3 7.5a.75.75 0 101.5.06l.3-7.5z" clip-rule="evenodd" />
              </svg>
            </button>
          </div>
        </div>
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

    <!-- Edit Project Modal -->
    <div
      v-if="showEditModal"
      class="fixed inset-0 bg-black/50 flex items-center justify-center z-50"
      @click.self="showEditModal = false"
    >
      <div class="bg-white rounded-xl shadow-xl p-6 w-full max-w-md mx-4">
        <h3 class="text-lg font-bold text-gray-800 mb-4">Edit Project</h3>
        <form @submit.prevent="handleEditProject" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Name</label>
            <input
              v-model="editForm.name"
              type="text"
              required
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Description</label>
            <textarea
              v-model="editForm.description"
              rows="3"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <p v-if="editError" class="text-red-500 text-sm">{{ editError }}</p>
          <div class="flex gap-3 justify-end">
            <button type="button" @click="showEditModal = false" class="px-4 py-2 text-sm text-gray-600 hover:text-gray-800">
              Cancel
            </button>
            <button
              type="submit"
              :disabled="editLoading"
              class="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50"
            >
              {{ editLoading ? 'Saving...' : 'Save' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Delete Confirm Modal -->
    <div
      v-if="showDeleteConfirm"
      class="fixed inset-0 bg-black/50 flex items-center justify-center z-50"
      @click.self="showDeleteConfirm = false"
    >
      <div class="bg-white rounded-xl shadow-xl p-6 w-full max-w-sm mx-4">
        <h3 class="text-lg font-bold text-gray-800 mb-2">Delete Project</h3>
        <p class="text-sm text-gray-600 mb-4">
          Delete <span class="font-semibold">{{ deletingProject?.name }}</span>? This cannot be undone.
        </p>
        <p v-if="deleteError" class="text-red-500 text-sm mb-4">{{ deleteError }}</p>
        <div class="flex gap-3 justify-end">
          <button @click="showDeleteConfirm = false" class="px-4 py-2 text-sm text-gray-600 hover:text-gray-800">
            Cancel
          </button>
          <button
            @click="handleDeleteProject"
            :disabled="deleteLoading"
            class="bg-red-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-red-700 disabled:opacity-50"
          >
            {{ deleteLoading ? 'Deleting...' : 'Delete' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
