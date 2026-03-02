<script setup lang="ts">
import type { ApiResponse, Project } from '~/types'

definePageMeta({ middleware: 'auth' })

const route = useRoute()
const projectId = Number(route.params.id)

const { logout } = useAuth()
const authStore = useAuthStore()
const api = useApi()

const { tasks, loading, error, fetchTasks, updateTaskStatus } = useTasks()

const project = ref<Project | null>(null)
const projectLoading = ref(true)
const showCreateModal = ref(false)
const updatingTaskId = ref<number | null>(null)

onMounted(async () => {
  const [projectRes] = await Promise.allSettled([
    api.get<ApiResponse<Project>>(`/projects/${projectId}`),
    fetchTasks(projectId),
  ])
  projectLoading.value = false

  if (projectRes.status === 'fulfilled') {
    project.value = projectRes.value.data.data
  }
})

async function handleStatusChange(taskId: number, newStatus: string) {
  updatingTaskId.value = taskId
  await updateTaskStatus(taskId, newStatus)
  updatingTaskId.value = null
}

function handleTaskCreated() {
  showCreateModal.value = false
  fetchTasks(projectId)
}
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <header class="bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
      <div class="flex items-center gap-3">
        <NuxtLink to="/projects" class="text-gray-400 hover:text-gray-600 text-sm">
          ← Projects
        </NuxtLink>
        <span class="text-gray-300">/</span>
        <span class="text-sm font-semibold text-gray-800">
          {{ projectLoading ? '...' : (project?.name ?? 'Project') }}
        </span>
      </div>
      <div class="flex items-center gap-4">
        <span class="text-sm text-gray-600">{{ authStore.user?.name }}</span>
        <button @click="logout" class="text-sm text-red-600 hover:underline">Logout</button>
      </div>
    </header>

    <main class="px-6 py-6">
      <div class="flex items-center justify-between mb-6">
        <div>
          <h2 class="text-2xl font-bold text-gray-800">
            <span v-if="projectLoading" class="inline-block w-40 h-7 bg-gray-200 animate-pulse rounded" />
            <template v-else>{{ project?.name }}</template>
          </h2>
          <p v-if="project?.description" class="text-sm text-gray-500 mt-1">{{ project.description }}</p>
        </div>
        <button
          @click="showCreateModal = true"
          class="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700"
        >
          + New Task
        </button>
      </div>

      <!-- Skeleton board -->
      <div v-if="loading" class="flex gap-4">
        <div v-for="i in 3" :key="i" class="flex-1 min-w-64 bg-gray-100 rounded-xl p-3 animate-pulse">
          <div class="h-4 bg-gray-200 rounded w-1/2 mb-4" />
          <div v-for="j in 2" :key="j" class="bg-white rounded-lg h-16 mb-2" />
        </div>
      </div>

      <p v-else-if="error" class="text-red-500">{{ error }}</p>

      <KanbanBoard
        v-else
        :tasks="tasks"
        :updating-task-id="updatingTaskId"
        @status-change="handleStatusChange"
      />
    </main>

    <CreateTaskModal
      v-if="showCreateModal"
      :project-id="projectId"
      @created="handleTaskCreated"
      @close="showCreateModal = false"
    />
  </div>
</template>
