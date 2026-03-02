import axios from 'axios'
import type { ApiResponse, Task, CreateTaskRequest } from '~/types'

export const useTasks = () => {
  const api = useApi()
  const tasks = ref<Task[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchTasks(projectId: number): Promise<void> {
    error.value = null
    loading.value = true
    try {
      const res = await api.get<ApiResponse<Task[]>>(`/projects/${projectId}/tasks`)
      tasks.value = res.data.data
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data?.message) {
        error.value = err.response.data.message
      } else {
        error.value = 'Failed to load tasks.'
      }
    } finally {
      loading.value = false
    }
  }

  async function createTask(projectId: number, data: CreateTaskRequest): Promise<Task | null> {
    try {
      const res = await api.post<ApiResponse<Task>>(`/projects/${projectId}/tasks`, data)
      const task = res.data.data
      tasks.value.push(task)
      return task
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data?.message) {
        error.value = err.response.data.message
      } else {
        error.value = 'Failed to create task.'
      }
      return null
    }
  }

  async function updateTaskStatus(taskId: number, status: string): Promise<void> {
    try {
      const res = await api.patch<ApiResponse<Task>>(`/tasks/${taskId}/status`, { status })
      const updated = res.data.data
      const idx = tasks.value.findIndex((t) => t.id === taskId)
      if (idx !== -1) tasks.value[idx] = updated
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data?.message) {
        error.value = err.response.data.message
      } else {
        error.value = 'Failed to update task status.'
      }
    }
  }

  async function deleteTask(taskId: number): Promise<boolean> {
    try {
      await api.delete(`/tasks/${taskId}`)
      tasks.value = tasks.value.filter((t) => t.id !== taskId)
      return true
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data?.message) {
        error.value = err.response.data.message
      } else {
        error.value = 'Failed to delete task.'
      }
      return false
    }
  }

  return { tasks, loading, error, fetchTasks, createTask, updateTaskStatus, deleteTask }
}
