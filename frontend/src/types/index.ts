export interface ApiResult<T> {
  success: boolean
  value?: T
  error?: { code: string; message: string }
}

export interface Project {
  id: string
  name: string
  description?: string
  createdById: string
  createdAt: string
  members: ProjectMember[]
}

export interface ProjectMember {
  projectId: string
  userId: string
  role: 'Owner' | 'Member' | 'Viewer'
}

export interface TaskItem {
  id: string
  projectId: string
  title: string
  description?: string
  status: TaskStatus
  priority: Priority
  assigneeId?: string
  dueDate?: string
  createdAt: string
}

export type TaskStatus = 'Todo' | 'InProgress' | 'Done'
export type Priority = 'Low' | 'Medium' | 'High'
