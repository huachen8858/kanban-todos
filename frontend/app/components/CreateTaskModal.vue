<script setup lang="ts">
import axios from 'axios'
import type { ApiResponse, Task } from '~/types'

const props = defineProps<{ projectId: number }>()
const emit = defineEmits<{ created: [task: Task]; close: [] }>()

const api = useApi()
const form = reactive({ title: '', description: '', priority: 'Medium', dueDate: '' })
const loading = ref(false)
const error = ref<string | null>(null)

async function handleSubmit() {
  error.value = null
  loading.value = true
  try {
    const payload: Record<string, unknown> = {
      title: form.title,
      priority: form.priority,
    }
    if (form.description) payload.description = form.description
    if (form.dueDate) payload.dueDate = form.dueDate

    const res = await api.post<ApiResponse<Task>>(`/projects/${props.projectId}/tasks`, payload)
    emit('created', res.data.data)
    form.title = ''
    form.description = ''
    form.priority = 'Medium'
    form.dueDate = ''
  } catch (err: unknown) {
    if (axios.isAxiosError(err) && err.response?.data?.message) {
      error.value = err.response.data.message
    } else {
      error.value = 'Failed to create task.'
    }
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div
    class="fixed inset-0 bg-black/50 flex items-center justify-center z-50"
    @click.self="emit('close')"
  >
    <div class="bg-white rounded-xl shadow-xl p-6 w-full max-w-md mx-4">
      <h3 class="text-lg font-bold text-gray-800 mb-4">New Task</h3>

      <form @submit.prevent="handleSubmit" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Title</label>
          <input
            v-model="form.title"
            type="text"
            required
            class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Description</label>
          <textarea
            v-model="form.description"
            rows="3"
            class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <div class="grid grid-cols-2 gap-3">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Priority</label>
            <select
              v-model="form.priority"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Due Date</label>
            <input
              v-model="form.dueDate"
              type="date"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
        </div>

        <p v-if="error" class="text-red-500 text-sm">{{ error }}</p>

        <div class="flex gap-3 justify-end">
          <button
            type="button"
            @click="emit('close')"
            class="px-4 py-2 text-sm text-gray-600 hover:text-gray-800"
          >
            Cancel
          </button>
          <button
            type="submit"
            :disabled="loading"
            class="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50"
          >
            {{ loading ? 'Creating...' : 'Create Task' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
