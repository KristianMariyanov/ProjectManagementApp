import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '../lib/apiClient'
import type { ApiResult, TaskItem, TaskStatus, Priority } from '../types'

export function useTasks(projectId: string, status?: TaskStatus) {
  const params = status ? `?status=${status}` : ''
  return useQuery({
    queryKey: ['tasks', projectId, status],
    queryFn: () =>
      api
        .get<ApiResult<TaskItem[]>>(`/api/projects/${projectId}/tasks${params}`)
        .then((r) => r.value ?? []),
    enabled: !!projectId,
  })
}

export function useTask(id: string) {
  return useQuery({
    queryKey: ['task', id],
    queryFn: () => api.get<ApiResult<TaskItem>>(`/api/tasks/${id}`).then((r) => r.value!),
    enabled: !!id,
  })
}

export function useCreateTask(projectId: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (data: {
      title: string
      description?: string
      priority: Priority
      assigneeId?: string
      dueDate?: string
    }) =>
      api
        .post<ApiResult<TaskItem>>(`/api/projects/${projectId}/tasks`, data)
        .then((r) => r.value!),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['tasks', projectId] }),
  })
}

export function useUpdateTask(id: string, projectId: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (data: Partial<TaskItem>) =>
      api.put<ApiResult<TaskItem>>(`/api/tasks/${id}`, data).then((r) => r.value!),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['tasks', projectId] })
      qc.invalidateQueries({ queryKey: ['task', id] })
    },
  })
}

export function useDeleteTask(projectId: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (id: string) => api.delete(`/api/tasks/${id}`),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['tasks', projectId] }),
  })
}
