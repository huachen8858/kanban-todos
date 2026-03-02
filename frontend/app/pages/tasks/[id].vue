<script setup lang="ts">
import axios from 'axios'
import type { ApiResponse, Task } from '~/types'

definePageMeta({ middleware: 'auth' })

const route = useRoute()
const taskId = Number(route.params.id)

const { logout } = useAuth()
const authStore = useAuthStore()
const api = useApi()

const { comments, loading: commentsLoading, fetchComments, createComment, deleteComment } = useComments()

const task = ref<Task | null>(null)
const taskLoading = ref(true)
const taskError = ref<string | null>(null)

const newComment = ref('')
const commentSubmitting = ref(false)
const commentError = ref<string | null>(null)

const statusLabel: Record<string, string> = {
  Todo:       'To Do',
  InProgress: 'In Progress',
  Done:       'Done',
}
const statusStyle: Record<string, string> = {
  Todo:       'bg-gray-100 text-gray-700',
  InProgress: 'bg-blue-100 text-blue-700',
  Done:       'bg-green-100 text-green-700',
}
const priorityStyle: Record<string, string> = {
  Low:    'bg-gray-100 text-gray-600',
  Medium: 'bg-yellow-100 text-yellow-700',
  High:   'bg-red-100 text-red-700',
}

onMounted(async () => {
  try {
    const res = await api.get<ApiResponse<Task>>(`/tasks/${taskId}`)
    task.value = res.data.data
  } catch {
    taskError.value = 'Task not found.'
  } finally {
    taskLoading.value = false
  }
  await fetchComments(taskId)
})

async function submitComment() {
  if (!newComment.value.trim()) return
  commentError.value = null
  commentSubmitting.value = true
  const result = await createComment(taskId, newComment.value.trim())
  if (result) {
    newComment.value = ''
  } else {
    commentError.value = 'Failed to add comment.'
  }
  commentSubmitting.value = false
}

async function handleDeleteComment(commentId: number) {
  await deleteComment(commentId)
}
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <header class="bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
      <div class="flex items-center gap-3">
        <button @click="$router.back()" class="text-gray-400 hover:text-gray-600 text-sm">
          ← Back
        </button>
        <span class="text-gray-300">/</span>
        <span class="text-sm font-semibold text-gray-800 truncate max-w-xs">
          {{ taskLoading ? '...' : (task?.title ?? 'Task') }}
        </span>
      </div>
      <div class="flex items-center gap-4">
        <span class="text-sm text-gray-600">{{ authStore.user?.name }}</span>
        <button @click="logout" class="text-sm text-red-600 hover:underline">Logout</button>
      </div>
    </header>

    <main class="max-w-3xl mx-auto px-6 py-8">
      <!-- Skeleton -->
      <div v-if="taskLoading" class="bg-white rounded-xl border border-gray-200 p-6 animate-pulse">
        <div class="h-6 bg-gray-200 rounded w-2/3 mb-4" />
        <div class="h-4 bg-gray-100 rounded w-1/3 mb-2" />
        <div class="h-4 bg-gray-100 rounded w-full mb-2" />
        <div class="h-4 bg-gray-100 rounded w-3/4" />
      </div>

      <p v-else-if="taskError" class="text-red-500">{{ taskError }}</p>

      <template v-else-if="task">
        <!-- Task detail card -->
        <div class="bg-white rounded-xl border border-gray-200 p-6 mb-6">
          <div class="flex items-start gap-3 mb-4">
            <h1 class="text-xl font-bold text-gray-800 flex-1">{{ task.title }}</h1>
            <span
              class="text-xs px-2 py-1 rounded-full font-medium"
              :class="statusStyle[task.status]"
            >
              {{ statusLabel[task.status] }}
            </span>
            <span
              class="text-xs px-2 py-1 rounded-full font-medium"
              :class="priorityStyle[task.priority]"
            >
              {{ task.priority }}
            </span>
          </div>

          <p v-if="task.description" class="text-gray-600 text-sm mb-4 whitespace-pre-wrap">
            {{ task.description }}
          </p>
          <p v-else class="text-gray-400 text-sm italic mb-4">No description</p>

          <div class="flex gap-6 text-xs text-gray-500">
            <span v-if="task.dueDate">Due: {{ new Date(task.dueDate).toLocaleDateString() }}</span>
            <span>Created: {{ new Date(task.createdAt).toLocaleDateString() }}</span>
          </div>
        </div>

        <!-- Comments -->
        <div class="bg-white rounded-xl border border-gray-200 p-6">
          <h2 class="text-base font-semibold text-gray-800 mb-4">
            Comments ({{ comments.length }})
          </h2>

          <!-- Add comment -->
          <form @submit.prevent="submitComment" class="mb-5">
            <textarea
              v-model="newComment"
              placeholder="Write a comment..."
              rows="3"
              class="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 mb-2"
            />
            <p v-if="commentError" class="text-red-500 text-xs mb-2">{{ commentError }}</p>
            <div class="flex justify-end">
              <button
                type="submit"
                :disabled="commentSubmitting || !newComment.trim()"
                class="bg-blue-600 text-white px-4 py-1.5 rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50"
              >
                {{ commentSubmitting ? 'Posting...' : 'Post' }}
              </button>
            </div>
          </form>

          <!-- Loading skeleton -->
          <div v-if="commentsLoading" class="space-y-3">
            <div v-for="i in 2" :key="i" class="animate-pulse">
              <div class="h-3 bg-gray-200 rounded w-1/4 mb-2" />
              <div class="h-4 bg-gray-100 rounded w-full" />
            </div>
          </div>

          <!-- Empty state -->
          <p v-else-if="comments.length === 0" class="text-gray-400 text-sm text-center py-4">
            No comments yet.
          </p>

          <!-- Comment list -->
          <div v-else class="space-y-3">
            <div
              v-for="comment in comments"
              :key="comment.id"
              class="flex items-start gap-3 p-3 rounded-lg bg-gray-50"
            >
              <div class="flex-1">
                <div class="flex items-center gap-2 mb-1">
                  <span class="text-xs font-medium text-gray-700">User #{{ comment.authorId }}</span>
                  <span class="text-xs text-gray-400">
                    {{ new Date(comment.createdAt).toLocaleString() }}
                  </span>
                </div>
                <p class="text-sm text-gray-700 whitespace-pre-wrap">{{ comment.content }}</p>
              </div>
              <button
                v-if="comment.authorId === authStore.user?.id"
                @click="handleDeleteComment(comment.id)"
                class="text-gray-300 hover:text-red-500 text-xs shrink-0 mt-0.5"
              >
                Delete
              </button>
            </div>
          </div>
        </div>
      </template>
    </main>
  </div>
</template>
