export interface ApiResponse<T> {
  success: boolean
  data: T
  message: string
  errors: string[]
}

export interface User {
  id: number
  name: string
  email: string
}

export interface Project {
  id: number
  name: string
  description: string | null
  ownerId: number
  createdAt: string
  updatedAt: string
}

export interface Task {
  id: number
  title: string
  description: string | null
  status: 'Todo' | 'InProgress' | 'Done'
  priority: 'Low' | 'Medium' | 'High'
  projectId: number
  assigneeId: number | null
  dueDate: string | null
  createdAt: string
  updatedAt: string
}

export interface TaskComment {
  id: number
  content: string
  taskId: number
  authorId: number
  createdAt: string
}

export interface AuthResponse {
  token: string
  user: User
}

export interface CreateTaskRequest {
  title: string
  description?: string
  priority: string
  dueDate?: string
}

export interface CreateProjectRequest {
  name: string
  description?: string
}
