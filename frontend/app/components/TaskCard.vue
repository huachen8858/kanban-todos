<script setup lang="ts">
import type { Task } from '~/types'

const props = defineProps<{ task: Task }>()

const priorityStyle: Record<string, string> = {
  Low:    'bg-gray-100 text-gray-600',
  Medium: 'bg-yellow-100 text-yellow-700',
  High:   'bg-red-100 text-red-700',
}

const formattedDueDate = computed(() => {
  if (!props.task.dueDate) return null
  return new Date(props.task.dueDate).toLocaleDateString()
})

const isOverdue = computed(() => {
  if (!props.task.dueDate || props.task.status === 'Done') return false
  return new Date(props.task.dueDate) < new Date()
})
</script>

<template>
  <div class="bg-white rounded-lg border border-gray-200 p-3 shadow-sm hover:shadow-md transition-shadow cursor-grab active:cursor-grabbing">
    <div class="flex items-start justify-between gap-2 mb-2">
      <NuxtLink
        :to="`/tasks/${task.id}`"
        class="text-sm font-medium text-gray-800 hover:text-blue-600 leading-snug"
        @click.stop
      >
        {{ task.title }}
      </NuxtLink>
      <span
        class="text-xs px-2 py-0.5 rounded-full font-medium shrink-0"
        :class="priorityStyle[task.priority] ?? 'bg-gray-100 text-gray-600'"
      >
        {{ task.priority }}
      </span>
    </div>
    <div v-if="formattedDueDate" class="flex items-center gap-1.5 mt-1">
      <p class="text-xs" :class="isOverdue ? 'text-red-500' : 'text-gray-400'">
        Due {{ formattedDueDate }}
      </p>
      <span
        v-if="isOverdue"
        class="text-xs font-medium px-1.5 py-0.5 rounded-full bg-red-100 text-red-700"
      >
        Overdue
      </span>
    </div>
  </div>
</template>
