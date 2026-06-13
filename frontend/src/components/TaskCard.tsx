import { Link } from 'react-router-dom'
import type { TaskItem, Priority } from '../types'

interface TaskCardProps {
  task: TaskItem
}

const priorityColors: Record<Priority, string> = {
  High: 'bg-red-100 text-red-700 dark:bg-red-900 dark:text-red-300',
  Medium: 'bg-yellow-100 text-yellow-700 dark:bg-yellow-900 dark:text-yellow-300',
  Low: 'bg-green-100 text-green-700 dark:bg-green-900 dark:text-green-300',
}

export default function TaskCard({ task }: TaskCardProps) {
  return (
    <Link
      to={`/tasks/${task.id}`}
      className="block bg-white dark:bg-gray-800 rounded-lg p-4 shadow-sm hover:shadow-md transition-shadow border border-gray-100 dark:border-gray-700"
    >
      <p className="text-sm font-medium text-gray-900 dark:text-white">{task.title}</p>
      <div className="flex items-center gap-2 mt-2">
        <span className={`text-xs px-2 py-0.5 rounded-full font-medium ${priorityColors[task.priority]}`}>
          {task.priority}
        </span>
        {task.dueDate && (
          <span className="text-xs text-gray-400">
            Due {new Date(task.dueDate).toLocaleDateString()}
          </span>
        )}
      </div>
    </Link>
  )
}
