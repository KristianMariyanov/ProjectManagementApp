import { useParams, Link } from 'react-router-dom'
import { useTask, useUpdateTask } from '../api/useTasks'
import type { TaskStatus, Priority } from '../types'

export default function TaskDetailPage() {
  const { id = '' } = useParams()
  const { data: task, isLoading } = useTask(id)
  const updateTask = useUpdateTask(id, task?.projectId ?? '')

  if (isLoading) return <div className="p-8 text-gray-500">Loading...</div>
  if (!task) return <div className="p-8 text-gray-500">Task not found</div>

  const handleStatusChange = (status: TaskStatus) =>
    updateTask.mutate({ status })

  const handlePriorityChange = (priority: Priority) =>
    updateTask.mutate({ priority })

  return (
    <div className="min-h-screen bg-white dark:bg-gray-900 p-8">
      <div className="max-w-2xl mx-auto">
        <Link
          to={`/projects/${task.projectId}`}
          className="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200 text-sm mb-4 block"
        >
          Back to board
        </Link>

        <h1 className="text-2xl font-bold text-gray-900 dark:text-white mb-2">{task.title}</h1>

        {task.description && (
          <p className="text-gray-600 dark:text-gray-400 mb-6">{task.description}</p>
        )}

        <div className="flex gap-4 mb-8">
          <div>
            <label className="block text-xs text-gray-500 mb-1">Status</label>
            <select
              value={task.status}
              onChange={(e) => handleStatusChange(e.target.value as TaskStatus)}
              className="border rounded-lg px-3 py-1.5 text-sm dark:bg-gray-800 dark:border-gray-700 dark:text-white"
            >
              <option value="Todo">To Do</option>
              <option value="InProgress">In Progress</option>
              <option value="Done">Done</option>
            </select>
          </div>
          <div>
            <label className="block text-xs text-gray-500 mb-1">Priority</label>
            <select
              value={task.priority}
              onChange={(e) => handlePriorityChange(e.target.value as Priority)}
              className="border rounded-lg px-3 py-1.5 text-sm dark:bg-gray-800 dark:border-gray-700 dark:text-white"
            >
              <option>High</option>
              <option>Medium</option>
              <option>Low</option>
            </select>
          </div>
          {task.dueDate && (
            <div>
              <label className="block text-xs text-gray-500 mb-1">Due date</label>
              <p className="text-sm text-gray-700 dark:text-gray-300 py-1.5">
                {new Date(task.dueDate).toLocaleDateString()}
              </p>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
