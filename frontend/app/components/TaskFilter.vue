<script setup lang="ts">
interface FilterState {
  search: string
  priority: string
  showOverdue: boolean
}

const props = defineProps<{ modelValue: FilterState }>()
const emit = defineEmits<{ 'update:modelValue': [value: FilterState] }>()

function update(patch: Partial<FilterState>) {
  emit('update:modelValue', { ...props.modelValue, ...patch })
}
</script>

<template>
  <div class="flex items-center justify-between gap-3 mb-4">
    <input
      type="text"
      placeholder="Search tasks..."
      :value="modelValue.search"
      @input="update({ search: ($event.target as HTMLInputElement).value })"
      class="w-1/3 px-3 py-1.5 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-300 bg-white"
    />
    <div class="flex items-center gap-3">
      <select
        :value="modelValue.priority"
        @change="update({ priority: ($event.target as HTMLSelectElement).value })"
        class="px-3 py-1.5 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-300 bg-white"
      >
        <option value="">All priorities</option>
        <option value="Low">Low</option>
        <option value="Medium">Medium</option>
        <option value="High">High</option>
      </select>
      <label class="flex items-center gap-2 text-sm text-gray-600 cursor-pointer select-none">
        <input
          type="checkbox"
          :checked="modelValue.showOverdue"
          @change="update({ showOverdue: ($event.target as HTMLInputElement).checked })"
          class="rounded"
        />
        Overdue only
      </label>
    </div>
  </div>
</template>
