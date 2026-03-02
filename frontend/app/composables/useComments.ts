import axios from 'axios'
import type { ApiResponse, TaskComment } from '~/types'

export const useComments = () => {
  const api = useApi()
  const comments = ref<TaskComment[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchComments(taskId: number): Promise<void> {
    error.value = null
    loading.value = true
    try {
      const res = await api.get<ApiResponse<TaskComment[]>>(`/tasks/${taskId}/comments`)
      comments.value = res.data.data
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data?.message) {
        error.value = err.response.data.message
      } else {
        error.value = 'Failed to load comments.'
      }
    } finally {
      loading.value = false
    }
  }

  async function createComment(taskId: number, content: string): Promise<TaskComment | null> {
    try {
      const res = await api.post<ApiResponse<TaskComment>>(`/tasks/${taskId}/comments`, { content })
      const comment = res.data.data
      comments.value.push(comment)
      return comment
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data?.message) {
        error.value = err.response.data.message
      } else {
        error.value = 'Failed to add comment.'
      }
      return null
    }
  }

  async function deleteComment(commentId: number): Promise<boolean> {
    try {
      await api.delete(`/comments/${commentId}`)
      comments.value = comments.value.filter((c) => c.id !== commentId)
      return true
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data?.message) {
        error.value = err.response.data.message
      } else {
        error.value = 'Failed to delete comment.'
      }
      return false
    }
  }

  return { comments, loading, error, fetchComments, createComment, deleteComment }
}
