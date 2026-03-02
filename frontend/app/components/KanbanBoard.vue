<script setup lang="ts">
import { VueDraggable } from 'vue-draggable-plus'
import type { Task } from '~/types'

const props = defineProps<{
  tasks: Task[]
  updatingTaskId: number | null
}>()

const emit = defineEmits<{
  statusChange: [taskId: number, newStatus: string]
}>()

const COLUMNS = [
  { status: 'Todo',       label: 'To Do',      color: 'bg-gray-100' },
  { status: 'InProgress', label: 'In Progress', color: 'bg-blue-50'  },
  { status: 'Done',       label: 'Done',        color: 'bg-green-50' },
] as const

// Each column holds its own reactive task list derived from props
const columnTasks = reactive<Record<string, Task[]>>({
  Todo:       [],
  InProgress: [],
  Done:       [],
})

// Sync from props whenever tasks change.
// Must mutate in-place (splice) — replacing the array reference breaks VueDraggable's
// SortableJS DOM sync (count updates but items don't re-render).
watch(
  () => props.tasks,
  (newTasks) => {
    const byStatus = (s: string) => newTasks.filter((t) => t.status === s)
    columnTasks.Todo.splice(0, columnTasks.Todo.length, ...byStatus('Todo'))
    columnTasks.InProgress.splice(0, columnTasks.InProgress.length, ...byStatus('InProgress'))
    columnTasks.Done.splice(0, columnTasks.Done.length, ...byStatus('Done'))
  },
  { immediate: true, deep: true }
)

function onEnd(event: { item: HTMLElement }, targetStatus: string) {
  const taskId = Number(event.item.dataset.taskId)
  if (!taskId) return
  emit('statusChange', taskId, targetStatus)
}
</script>

<template>
  <div class="flex gap-4 overflow-x-auto pb-4">
    <div
      v-for="col in COLUMNS"
      :key="col.status"
      class="flex-1 min-w-64 rounded-xl p-3"
      :class="col.color"
    >
      <div class="flex items-center justify-between mb-3 px-1">
        <h3 class="text-sm font-semibold text-gray-700">{{ col.label }}</h3>
        <span class="text-xs text-gray-500 bg-white px-2 py-0.5 rounded-full">
          {{ columnTasks[col.status].length }}
        </span>
      </div>

      <VueDraggable
        v-model="columnTasks[col.status]"
        group="tasks"
        class="min-h-12 space-y-2"
        @end="(e) => onEnd(e, col.status)"
      >
        <div
          v-for="task in columnTasks[col.status]"
          :key="task.id"
          :data-task-id="task.id"
          class="relative"
        >
          <TaskCard :task="task" />
          <div
            v-if="updatingTaskId === task.id"
            class="absolute inset-0 bg-white/70 rounded-lg flex items-center justify-center"
          >
            <div class="w-4 h-4 border-2 border-blue-500 border-t-transparent rounded-full animate-spin" />
          </div>
        </div>
      </VueDraggable>
    </div>
  </div>
</template>
